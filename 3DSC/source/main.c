#include <3ds.h>
#include <stdio.h>
#include <stdlib.h>

#include <string.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <netdb.h>
#include <arpa/inet.h>
#include <fcntl.h>
#include <errno.h>

//The top screen has 30 rows and 50 columns
//The bottom screen has 30 rows and 40 columns

#define PORT 25566
#define BUFSIZE 32

int sock;
struct sockaddr_in in;
struct sockaddr_in out;
socklen_t addrlen = (int)sizeof(struct sockaddr_in);
short playerID = 0;
bool backlightOff = false;

struct Message {
	unsigned short ID;
	unsigned short pdx;
	unsigned short pdy;
	unsigned int btn;

	u16 touch_px; // Touchscreen pixel x-coord
	u16 touch_py; // Touchscreen pixel y-coord
} message;

#ifndef REG_LCDBACKLIGHTMAIN
#define REG_LCDBACKLIGHTMAIN (u32)(0x1ED02240 - 0x1EB00000)
#endif

#ifndef REG_LCDBACKLIGHTSUB
#define REG_LCDBACKLIGHTSUB (u32)(0x1ED02A40 - 0x1EB00000)
#endif

static u32 brightnessMain;
static u32 brightnessSub;

void disableBacklight() {
	u32 off = 0;

	GSPGPU_ReadHWRegs(REG_LCDBACKLIGHTMAIN, &brightnessMain, 4);
	GSPGPU_ReadHWRegs(REG_LCDBACKLIGHTSUB, &brightnessSub, 4);

	GSPGPU_WriteHWRegs(REG_LCDBACKLIGHTMAIN, &off, 4);
	GSPGPU_WriteHWRegs(REG_LCDBACKLIGHTSUB, &off, 4);
}

void enableBacklight() {
	GSPGPU_WriteHWRegs(REG_LCDBACKLIGHTMAIN, &brightnessMain, 4);
	GSPGPU_WriteHWRegs(REG_LCDBACKLIGHTSUB, &brightnessSub, 4);
}

int main(int argc, char **argv) {
	gfxInitDefault();
	socInit((u32 *)memalign(0x1000, 0x100000), 0x100000);

	// Check wifi status
	u32 wifiStatus = 0;
	ACU_GetWifiStatus(&wifiStatus);
	if (!wifiStatus) {
		printf("\x1b[1;1HNo WiFi! Is your wireless slider on?");
	}

	// Use printf on top screen
	consoleInit(GFX_TOP, NULL);

	// Stuff for network magic
	int recvlen;
	unsigned char buf[BUFSIZE];
	unsigned char IDBuf[BUFSIZE];

	// Try create socket
	int err = 0;
	if ((sock = socket(AF_INET, SOCK_DGRAM, 0)) < 0) {
		err = 1;
	}

	// Setup socket addresses
	out.sin_family = in.sin_family = AF_INET;
	out.sin_port = in.sin_port = htons(PORT); // Set port
	in.sin_addr.s_addr = INADDR_ANY;

	// Try to bind socket to port
	if (err == 0 && bind(sock, (struct sockaddr *)&in, sizeof(in)) < 0)
		err = 2;

	// If any errors
	if (err != 0)
		printf("\x1b[4;1HError opening connection: %i", err);

	// Set socket receive to non-blocking to be able to exit while listening for broadcast
	fcntl(sock, F_SETFL, O_NONBLOCK);


	bool connected = false;

	// Main loop
	while (aptMainLoop()) {

		if (err == 0) {
			// If not connected, listen for broadcast
			if (!connected) {
				printf("\x1b[1;1HListening for broadcast on port %d", PORT);

				// Listen for packets. Returns packet size
				recvlen = recvfrom(sock, buf, BUFSIZE, 0, (struct sockaddr *)&out, &addrlen);

				// Check if correct packet. recvlen < 0 -> Error
				if (recvlen > 0) {
					printf("\x1b[2;1Hr"); // Debug print. Writes r for any packet received
					buf[recvlen] = 0; // Don't remember what this is for. Oops

									  // Check if message is connection port (Broadcast sends out port number as broadcast)
					int i = atoi(buf);
					if (i == PORT) {
						// If broadcast message, assign send address to received address
						in.sin_addr.s_addr = out.sin_addr.s_addr;

						// Prepare program; Setting flag, turning of backlight, etc
						connected = true;
						consoleClear();
						printf("\x1b[1;1HConnected");
						if (backlightOff == false)
							disableBacklight();
						backlightOff = true;
					}
				}
			}
		}

		printf("\x1b[28;1HPlayer ID: %i", playerID);
		printf("\x1b[29;1HPress START and SELECT to exit");

		// Scan input
		hidScanInput();
		// Save keystate
		u32 kDown = hidKeysHeld();

		if ((kDown & KEY_START) && (kDown & KEY_SELECT)) break; // break in order to return to hbmenu

		// If no errors and connected. Contruct input message
		if (err == 0 && connected) {
			// Get circle pad state
			circlePosition pos;
			hidCircleRead(&pos);

			// Construct message
			message.ID = playerID;
			message.pdx = pos.dx;
			message.pdy = pos.dy;
			message.btn = kDown;

			// Query for touchscreen information
			touchPosition touch;
			hidTouchRead(&touch);

			message.touch_px = touch.px;
			message.touch_py = touch.py;

			// Send packet to address broadcast came from
			sendto(sock, &message, sizeof(message), 0, (struct sockaddr *)&in, sizeof(in));

			int recv = 0;
			int count = 0;
			do {
				count++;
				// Send packet to address broadcast came from
				sendto(sock, &message, sizeof(message), 0, (struct sockaddr *)&in, sizeof(in));
				// Listen for packet to get player ID and check if server is alive
				//int recv = read(sock, IDBuf, sizeof(IDBuf));
				recv = recvfrom(sock, IDBuf, BUFSIZE, 0, (struct sockaddr *)&out, &addrlen);
			} while (atoi(IDBuf) == PORT && count < 5);

			if (count < 5 && recv > 0)
				playerID = IDBuf[0] | IDBuf[1] << 1;
			else {
				connected = false;
				playerID = 0;
				consoleClear();
				printf("\x1b[2;1HDisconnected");

				if (backlightOff == true) {
					enableBacklight();
					backlightOff = false;
				}
			}
		}

		u64 sleepDuration = 16000000ULL;
		svcSleepThread(sleepDuration);

		// Draw stuff
		gfxFlushBuffers();
		gfxSwapBuffers();
		gspWaitForVBlank();
	}

	// On exit
	if (backlightOff == true)
		enableBacklight();
	socExit();
	gfxExit();
	return 0;
}

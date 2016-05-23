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
#define BUFSIZE 2048

int sock;
struct sockaddr_in in;
struct sockaddr_in out;
socklen_t addrlen = (int)sizeof(struct sockaddr_in);

struct Message {
	unsigned short pdx;
	unsigned short pdy;
	unsigned int btn;
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

	u32 wifiStatus = 0;
	ACU_GetWifiStatus(&wifiStatus);
	if (!wifiStatus) {
		printf("\x1b[1;1HNo WiFi! Is your wireless slider on?");
	}


	//Initialize console on top screen. Using NULL as the second argument tells the console library to use the internal console structure as current one
	consoleInit(GFX_TOP, NULL);

	int recvlen;
	unsigned char buf[BUFSIZE];

	int err = 0;
	if ((sock = socket(AF_INET, SOCK_DGRAM, 0)) < 0) {
		err = 1;
	}

	out.sin_family = in.sin_family = AF_INET;
	out.sin_port = in.sin_port = htons(PORT);
	in.sin_addr.s_addr = INADDR_ANY;

	if (err == 0 && bind(sock, (struct sockaddr *)&in, sizeof(in)) < 0) {
		err = 2;
	}
	
	if (err != 0)
		printf("\x1b[4;1HError opening connection: %i", err);

	fcntl(sock, F_SETFL, O_NONBLOCK);

	bool connected = false;
	int count = 0;

	// Main loop
	while (aptMainLoop()) {		
		count++;

		if (err == 0) {
			if (!connected) {
				printf("\x1b[1;1HListening for broadcast on port %d", PORT);
				recvlen = recvfrom(sock, buf, BUFSIZE, 0, (struct sockaddr *)&out, &addrlen);
				if (recvlen > 0) {
					printf("\x1b[2;1Hr");

					buf[recvlen] = 0; 
					int i = atoi(buf);
					if (i == PORT) {
						in.sin_addr.s_addr = out.sin_addr.s_addr;

						connected = true;
						gfxFlushBuffers();
						gfxSwapBuffers();
						gspWaitForVBlank();
						printf("\x1b[2;1HConnected");
						disableBacklight();
					}
				}
			}
		}
		
		printf("\x1b[29;1HPress START and SELECT to exit");
		
		hidScanInput();
		u32 kDown = hidKeysHeld();

		if ((kDown & KEY_START) && (kDown & KEY_SELECT)) break; // break in order to return to hbmenu
		
		if (err == 0 && connected) {
			circlePosition pos;
			hidCircleRead(&pos);
			
			message.pdx = pos.dx;
			message.pdy = pos.dy;
			message.btn = kDown;
			sendto(sock, &message, sizeof(message), 0, (struct sockaddr *)&in, sizeof(in));
		}

		gfxFlushBuffers();
		gfxSwapBuffers();
		gspWaitForVBlank();
	}
	enableBacklight();

	socExit();
	gfxExit();
	return 0;
}

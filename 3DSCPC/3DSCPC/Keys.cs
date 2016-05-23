using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DSCPC
{
    class Keys
    {
        public static UInt32 A                 = 1 << 0;
        public static UInt32 B                 = 1 << 1;
        public static UInt32 SELECT            = 1 << 2;
        public static UInt32 START             = 1 << 3;
        public static UInt32 DRIGHT            = 1 << 4;
        public static UInt32 DLEFT             = 1 << 5;
        public static UInt32 DUP               = 1 << 6;
        public static UInt32 DDOWN             = 1 << 7;
        public static UInt32 R                 = 1 << 8;
        public static UInt32 L                 = 1 << 9;
        public static UInt32 X                 = 1 << 10;
        public static UInt32 Y                 = 1 << 11;
        public static UInt32 ZL                = 1 << 14;
        public static UInt32 ZR                = 1 << 15;
        public static UInt32 TOUCH             = 1 << 20;
        public static UInt32 CSTICK_RIGHT      = 1 << 24;
        public static UInt32 CSTICK_LEFT       = 1 << 25;
        public static UInt32 CSTICK_UP         = 1 << 26;
        public static UInt32 CSTICK_DOWN       = 1 << 27;
        public static UInt32 CPAD_RIGHT        = 1 << 28;
        public static UInt32 CPAD_LEFT         = 1 << 29;
        public static UInt32 CPAD_UP           = 1 << 30;
        public static UInt32 CPAD_DOWN         = 2147483648;
        
        private static UInt32 _btn;

        public static bool isDown(UInt32 key) {
            if ((_btn & key) == key)
                return true;

            return false;
        }

        public static void update(UInt32 btn) {
            _btn = btn;
        }
    }
}

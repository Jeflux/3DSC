using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vJoyInterfaceWrap;

namespace _3DSCPC
{
    class vjoyhelper
    {
        public static string setup(vJoy joystick, uint id) {

            if (!joystick.vJoyEnabled()) {
                return "Not enabled";
            }

            UInt32 DllVer = 0, DrvVer = 0;
            bool match = joystick.DriverMatch(ref DllVer, ref DrvVer);
            if (!match)
                return "Version of Driver ({0:X}) does NOT match DLL Version ({1:X})";

            VjdStat status = joystick.GetVJDStatus(id);
            switch (status) {
                case VjdStat.VJD_STAT_OWN:
                case VjdStat.VJD_STAT_FREE:
                    break;
                case VjdStat.VJD_STAT_BUSY:
                    return "vJoy Device " + id + " is already owned by another feeder\nCannot continue";
                case VjdStat.VJD_STAT_MISS:
                    return "vJoy Device " + id + " is not installed or disabled\nCannot continue";
                default:
                    return "vJoy Device " + id + " general error. Cannot continue";
            };

            if ((status == VjdStat.VJD_STAT_OWN) || ((status == VjdStat.VJD_STAT_FREE) && (!joystick.AcquireVJD(id)))) {
                return "Failed to acquire vJoy device number " + id;
            }

            joystick.ResetVJD(id);

            return null;
        }

    }
}

using System;

namespace MissionControl.Definitions
{
    [Flags]
    public enum BoardMask : ushort
    {
        Main             = 1,
        Actuator         = 2,
        Inertia          = 4,
        Sensor           = 8,
        Power            = 16,
        Telemetry        = 32,
        TestStand        = 64,
        BlackBox         = 128,
        NotImplemented2  = 256,
        NotImplemented3  = 512,
        NotImplemented4  = 1024,
        NotImplemented5  = 2048,
        NotImplemented6  = 4096,
        NotImplemented7  = 8192,
        NotImplemented8  = 16384,
        Test             = 32768,
    }
}
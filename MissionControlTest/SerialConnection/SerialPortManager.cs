using System;
using System.IO.Ports;
using MissionControl.Definitions;
using Serilog;

namespace MissionControl.SerialConnection
{
    public static class SerialPortManager
    {
        private static readonly SerialPort GroundPort = new();
        private static readonly SerialPort TelemetryPort = new();
        
        public static SerialPort GetSerialPort(SerialPortSource source)
        {
            return source switch
            {
                SerialPortSource.Telemetry => TelemetryPort,
                SerialPortSource.Ground => GroundPort,
                _ => null
            };
        }
        
        
        
    }
}
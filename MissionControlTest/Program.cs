// See https://aka.ms/new-console-template for more information

using System.IO.Ports;
using System.Text;
using MissionControl.Definitions;
using MissionControl.SerialConnection;
using MissionControl.SerialConnection.Frame;

namespace MissionControlTest;

public static class Program
{
    private static readonly Action<Package, SerialPortSource> HandlePackage =
        delegate(Package package, SerialPortSource source)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(package);
            builder.Append(", ");
            builder.Append(source);
            Console.WriteLine(builder.ToString());
        };


    public static void Main(string[] args)
    {
        SerialPort serialPort = SerialPortManager.GetSerialPort(SerialPortSource.Telemetry);
        foreach (var name in SerialPort.GetPortNames())
        {
            Console.WriteLine("PortName: " + name);
        }

        string? portName = null;
        while (portName == null)
        {
            Console.WriteLine("Enter chosen portName");
            portName = Console.ReadLine();
            if (portName != null)
            {
                break;
            }
        }
        Console.WriteLine("Done");
        serialPort.BaudRate = (int)BaudRate.Baudrate115200;
        serialPort.PortName = portName;

        TelemetryConnectionManager manager = new TelemetryConnectionManager(HandlePackage);
        //GroundConnectionManager groundConnectionManager = new(HandlePackage);
        //Thread groundThread = new(groundConnectionManager.Run);
        Thread thread = new Thread(manager.Run);
        thread.Start();
        //groundThread.Start();

        
        
        //groundConnectionManager.Open();

        manager.Open();
    }
}
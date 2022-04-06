// See https://aka.ms/new-console-template for more information

using MissionControl.Definitions;
using MissionControl.SerialConnection;
using MissionControl.SerialConnection.Frame;

namespace MissionControlTest;

public static class Program
{
    private static readonly Action<Package, SerialPortSource> HandlePackage =
        delegate(Package package, SerialPortSource source) { Console.WriteLine(package.ToString()); };


    public static void Main(string[] args)
    {
        TelemetryConnectionManager manager = new TelemetryConnectionManager(HandlePackage);

        Thread thread = new Thread(manager.Run);
        thread.Start();
        
        manager.UseSimulation();
        manager.Open(true);
        


    }

}
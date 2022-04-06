using System.Collections.Generic;
using System.Threading;
using MissionControl.SerialConnection.Commands;
using MissionControl.SerialConnection.Frame;

namespace MissionControl.SerialConnection.Simulation
{
    public class SerialPortReaderSimulator
    {
        private readonly AutoResetEvent _resetEvent;
        private readonly Queue<Package> _packageQueue;
        
        private int _time;
        private short _temperature = 1000;


        private bool Running { get; set; }


        public SerialPortReaderSimulator(AutoResetEvent resetEvent,Queue<Package> packageQueue)
        {
            _resetEvent = resetEvent;
            _packageQueue = packageQueue;
        }

        public void Run()
        {
            //Open the thread in a suspended state.
            _resetEvent.WaitOne();
            AutoResetEvent resetEvent = new(false);

            //Main loop - Runs until the thread is shutdown. 
            while (Running)
            {
                while (Running && !_resetEvent.WaitOne(0)) //TODO Check functionality here. 
                {
                    DispatchPackage();
                    resetEvent.WaitOne(100);
                    
                }
                _resetEvent.WaitOne();
            }
        }
        

        private void DispatchPackage()
        {
            List<Command> commands = new List<Command>
            {
                new TimeCommand(_time++),
                new TemperatureCommand(_temperature++)
            };

            Package package = new Package(commands);
            
            lock (_packageQueue)
            {
                _packageQueue.Enqueue(package);

                Monitor.PulseAll(_packageQueue);
            }
        }

        public void Stop()
        {
            Running = false;
            _resetEvent.Set();
        }
    }
}
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using MissionControl.SerialConnection.Commands;

namespace MissionControl.SerialConnection
{
    public class SerialPortWriter
    {
        private readonly Queue<Command> _commandQueue;
        private readonly Queue<Command> _internalQueue = new();

        private readonly SerialPort _serialPort;
        private readonly AutoResetEvent _resetEvent;

        private bool Running { get; set; }


        public SerialPortWriter(SerialPort serialPort, AutoResetEvent resetEvent, Queue<Command> commandQueue)
        {
            _serialPort = serialPort;
            _resetEvent = resetEvent;
            _commandQueue = commandQueue;
        }


        public void Run()
        {
            //Open the thread in a suspended state.
            _resetEvent.WaitOne();

            //Main loop - Runs until the thread is shutdown. 
            while (Running)
            {
                while (Running & _serialPort.IsOpen)
                {
                    DispatchCommands();
                }

                //Wait until the the serialPort is open
                _resetEvent.WaitOne();
            }
        }


        private void DispatchCommands()
        {
            lock (_commandQueue)
            {
                while (_commandQueue.Count > 0)
                {
                    _internalQueue.Enqueue(_commandQueue.Dequeue());
                }

                Monitor.PulseAll(_commandQueue);
            }

            while (_internalQueue.Count > 0)
            {
                Command command = _internalQueue.Dequeue();
                _serialPort.Write(command.ToByteArray(), 0, command.ToByteArray().Length);
            }
        }


        public void Stop()
        {
            Running = false;
            _resetEvent.Set();
        }
    }
}
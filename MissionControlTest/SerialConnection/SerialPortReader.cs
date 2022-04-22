using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

namespace MissionControl.SerialConnection
{
    public class SerialPortReader
    {
        private readonly SerialPort _serialPort;
        private readonly AutoResetEvent _resetEvent;


        private static readonly Queue<byte[]> ExternalByteQueue = new();
        private readonly Queue<byte[]> _internalByteQueue = new();
        private readonly Parser _parser;


        private bool Running { get; set; } = true;


        public SerialPortReader(SerialPort serialPort, AutoResetEvent resetEvent, Parser parser)
        {
            _serialPort = serialPort;
            _resetEvent = resetEvent;
            _parser = parser;
            serialPort.DataReceived += DataReceived;
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
                    DispatchBytes();
                }

                //Wait until the the serialPort is open
                _resetEvent.WaitOne();
            }
        }


        private void DispatchBytes()
        {
            lock (ExternalByteQueue)
            {
                while (ExternalByteQueue.Count > 0)
                {
                    _internalByteQueue.Enqueue(ExternalByteQueue.Dequeue());
                }

                Monitor.PulseAll(ExternalByteQueue);
            }

            while (_internalByteQueue.Count > 0)
            {
                _parser.Add(_internalByteQueue.Dequeue());
            }
        }

        public void Stop()
        {
            Running = false;
            _resetEvent.Set();
        }


        private static void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            using SerialPort serialPort = (SerialPort)sender;
            if (!serialPort.IsOpen || serialPort.BytesToRead <= 0) return;

            lock (ExternalByteQueue)
            {
                int bytesToRead = serialPort.BytesToRead;
                byte[] buffer = new byte[bytesToRead];
                serialPort.Read(buffer, 0, bytesToRead);
                ExternalByteQueue.Enqueue(buffer);
                Monitor.PulseAll(ExternalByteQueue);
            }
        }
    }
}
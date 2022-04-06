using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using MissionControl.Definitions;
using MissionControl.SerialConnection.Commands;
using MissionControl.SerialConnection.Frame;
using MissionControl.SerialConnection.Simulation;
using Serilog;

namespace MissionControl.SerialConnection
{
    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
    public class GroundConnectionManager : IDisposable
    {
        public delegate void HandlePackage(Package package, SerialPortSource source);

        private HandlePackage _handlePackage;


        private readonly Queue<Package> _groundQueue = new Queue<Package>();
        private readonly Queue<Command> _commandQueue = new Queue<Command>();

        private readonly Queue<Tuple<Package, SerialPortSource>> _internalQueue = new();


        private SerialPortReader _groundReader;
        private SerialPortWriter _groundWriter;

        private SerialPortReaderSimulator _groundSimulator;


        private readonly AutoResetEvent _groundReaderEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent _groundWriterEvent = new AutoResetEvent(false);

        private readonly AutoResetEvent _groundReaderSimulatorEvent = new AutoResetEvent(false);


        private Thread _groundReaderThread;
        private Thread _groundWriterThread;
        private bool _disposed;
        private bool _shouldRun = true;


        public GroundConnectionManager(HandlePackage handlePackage)
        {
            _handlePackage = handlePackage;


            _groundReader = new SerialPortReader(SerialPortManager.GetSerialPort(SerialPortSource.Ground),
                _groundReaderEvent, new Parser(_groundQueue));
            _groundWriter = new SerialPortWriter(SerialPortManager.GetSerialPort(SerialPortSource.Ground),
                _groundWriterEvent, _commandQueue);


            //TODO Create Ack thread

            _groundReaderThread = new Thread(_groundReader.Run);
            _groundWriterThread = new Thread(_groundWriter.Run);
        }


        public void Run()
        {
            while (_shouldRun)
            {
                Dequeue(); //Makes the thread wait until we've received data from the SerialPortThread. 
                while (_internalQueue.Count > 0)
                {
                    (Package package, SerialPortSource source) = _internalQueue.Dequeue();


                    //TODO Add Ack and resend functionality here. 
                    if (package.Header.IsAck)
                    {
                        //Ack Manager to something
                    }

                    _handlePackage(package, source);
                }
            }
        }


        private void Dequeue()
        {
            //https://www.codeproject.com/Articles/28785/Thread-synchronization-Wait-and-Pulse-demystified
            lock (_groundQueue)
            {
                while (_shouldRun && _groundQueue.Count == 0) Monitor.Wait(_groundQueue);
                while (_groundQueue.Count > 0)
                {
                    _internalQueue.Enqueue(
                        new Tuple<Package, SerialPortSource>(_groundQueue.Dequeue(), SerialPortSource.Ground));
                }
            }

            Monitor.PulseAll(_groundQueue);
        }


        public void SendCommand(Command command)
        {
            if (SerialPortManager.GetSerialPort(SerialPortSource.Ground).IsOpen == false)
            {
                Log.Warning("Tried to send command: {@Command} but port was closed", command);
                return;
            }

            if (command.Info.IsAcknowledged)
            {
                //TODO Add to ack Queue
            }


            lock (_commandQueue)
            {
                _commandQueue.Enqueue(command);
            }

            Monitor.PulseAll(_commandQueue);
        }

        /// <summary>
        /// Opens the serial Port for the given source or starts the thread if simulation  
        /// </summary>
        /// <param name="isSimulation">Signals to stop simulation thread. Default false.</param>
        public void Open(bool isSimulation = false)
        {
            if (isSimulation)
            {
                SerialPortManager.GetSerialPort(SerialPortSource.Ground).Open();
                _groundReaderEvent.Set();
                _groundWriterEvent.Set();
            }
            else
            {
                _groundReaderSimulatorEvent.Set();
            }
        }


        /// <summary>
        /// Closes the serialPort for the given source. If simulation then the thread is paused. 
        /// 
        /// </summary>
        /// <param name="source">Ground Or Telemetry.</param>
        /// <param name="isSimulation">Signals to stop simulation thread. Default false.</param>
        public void Close(SerialPortSource source, bool isSimulation = false)
        {
            if (isSimulation)
            {
                SerialPortManager.GetSerialPort(source).Close();
                _groundReaderEvent.Set();
                _groundWriterEvent.Set();
            }
            else
            {
                _groundReaderSimulatorEvent.Set();
            }
        }


        /// <summary>
        /// Stops the thread(s) for the given source. 
        /// </summary>
        /// <param name="isSimulation">Signals to stop simulation thread. Default false.</param>
        public void Stop(bool isSimulation = false)
        {
            if (isSimulation)
            {
                _groundReader.Stop();
                _groundWriter.Stop();
            }
            else
            {
                _groundSimulator.Stop();
            }
        }


        public void StopThread()
        {
            _shouldRun = false;
        }


        public void StopAll()
        {
            _groundReader.Stop();
            _groundWriter.Stop();
            _groundSimulator.Stop();
        }


        private static bool CheckSerialPortInfo(SerialPortSource source)
        {
            if (!SerialPortManager.GetSerialPort(source).PortName.Equals(null)) return true;
            Log.Error("Serial port info not set for {@Source}", source);
            return false;
        }


        public void UseNormalReader()
        {
            _groundSimulator.Stop();
            _groundReaderThread = new Thread(_groundReader.Run);
            _groundReaderThread.Start();
        }

        public void UseSimulation()
        {
            _groundReader.Stop();
            _groundSimulator = new SerialPortReaderSimulator(_groundReaderSimulatorEvent, _groundQueue);
            _groundReaderThread = new Thread(_groundSimulator.Run);
            _groundReaderThread.Start();
        }


        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                StopAll();
            }

            _shouldRun = false;
            _groundReaderEvent?.Dispose();
            _groundWriterEvent?.Dispose();
            _groundReaderSimulatorEvent?.Dispose();

            _disposed = true;
        }
    }
}
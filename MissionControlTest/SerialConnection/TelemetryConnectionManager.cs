using System.Diagnostics.CodeAnalysis;
using MissionControl.Definitions;
using MissionControl.SerialConnection.Frame;
using MissionControl.SerialConnection.Simulation;
using Serilog;

namespace MissionControl.SerialConnection
{
    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
    public class TelemetryConnectionManager : IDisposable
    {
        private readonly Action<Package, SerialPortSource> _handlePackage;


        private readonly Queue<Package> _telemetryQueue = new Queue<Package>();

        private readonly Queue<Tuple<Package, SerialPortSource>> _internalQueue = new();


        private SerialPortReader _telemetryReader;
        private SerialPortReaderSimulator _telemetrySimulator;


        private readonly AutoResetEvent _telemetryReaderEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent _telemetryReaderSimulatorEvent = new AutoResetEvent(false);


        private Thread _telemetryReaderThread;
        private bool _disposed;
        private bool _shouldRun = true;


        public TelemetryConnectionManager(Action<Package, SerialPortSource> handlePackage)
        {
            _handlePackage = handlePackage;


            _telemetryReader = new SerialPortReader(SerialPortManager.GetSerialPort(SerialPortSource.Telemetry),
                _telemetryReaderEvent, new Parser(_telemetryQueue));
            _telemetrySimulator =
                new SerialPortReaderSimulator(_telemetryReaderSimulatorEvent, _telemetryQueue);

            _telemetryReaderThread = new Thread(_telemetryReader.Run);
            _telemetryReaderThread.Start();
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
                    // if (package.Header.IsAck)
                    // {
                    //     //Ack Manager to something
                    // }

                    _handlePackage(package, source);
                }
            }
        }


        private void Dequeue()
        {
            //https://www.codeproject.com/Articles/28785/Thread-synchronization-Wait-and-Pulse-demystified
            lock (_telemetryQueue)
            {
                while (_shouldRun && _telemetryQueue.Count == 0) Monitor.Wait(_telemetryQueue);
                while (_telemetryQueue.Count > 0)
                {
                    _internalQueue.Enqueue(new Tuple<Package, SerialPortSource>(_telemetryQueue.Dequeue(),
                        SerialPortSource.Telemetry));
                }

                Monitor.PulseAll(_telemetryQueue);
            }
        }

        /// <summary>
        /// Opens the serial Port for the given source or starts the thread if simulation  
        /// </summary>
        /// <param name="isSimulation">Signals to stop simulation thread. Default false.</param>
        public void Open(bool isSimulation = false)
        {
            if (isSimulation == false)
            {
                SerialPortManager.GetSerialPort(SerialPortSource.Telemetry).Open();
                _telemetryReaderEvent.Set();
            }
            else
            {
                _telemetryReaderSimulatorEvent.Set();
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
            if (isSimulation) SerialPortManager.GetSerialPort(SerialPortSource.Telemetry).Close();
            else _telemetryReaderSimulatorEvent.Set();
        }


        /// <summary>
        /// Stops the thread(s) for the given source. 
        /// </summary>
        /// <param name="source">Ground Or Telemetry.</param>
        /// <param name="isSimulation">Signals to stop simulation thread. Default false.</param>
        public void Stop(SerialPortSource source, bool isSimulation = false)
        {
            if (isSimulation) _telemetryReader.Stop();
            else _telemetrySimulator.Stop();
        }


        public void StopThread()
        {
            _shouldRun = false;
        }


        public void StopAll()
        {
            _telemetryReader.Stop();
            _telemetrySimulator.Stop();
        }


        private static bool CheckSerialPortInfo(SerialPortSource source)
        {
            if (!SerialPortManager.GetSerialPort(source).PortName.Equals(null)) return true;
            Log.Error("Serial port info not set for {@Source}", source);
            return false;
        }


        public void UseNormalReader()
        {
            _telemetrySimulator.Stop();
            _telemetryReader = new SerialPortReader(SerialPortManager.GetSerialPort(SerialPortSource.Telemetry),
                _telemetryReaderEvent, new Parser(_telemetryQueue));
            _telemetryReaderThread = new Thread(_telemetryReader.Run);
            _telemetryReaderThread.Start();
        }

        public void UseSimulation()
        {
            _telemetryReader.Stop();

            _telemetryReaderThread = new Thread(_telemetrySimulator.Run);
            _telemetryReaderThread.Start();
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
            _telemetryReaderEvent?.Dispose();
            _telemetryReaderSimulatorEvent?.Dispose();

            _disposed = true;
        }
    }
}
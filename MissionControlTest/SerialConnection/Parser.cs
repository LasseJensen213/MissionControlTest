using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MissionControl.SerialConnection.Frame;
using Serilog;

namespace MissionControl.SerialConnection
{
    public class Parser
    {
        private readonly Queue<Package> _queue;

        //Array used to check if sequence is equal to either the start- or end-tag
        private readonly byte[] _fence = new byte[5];
        private readonly List<byte> _buffer = new();

        private bool _reading;

        private Header _header;

        public Parser(Queue<Package> queue)
        {
            _queue = queue;
        }


        private void AddToQueue(Package package)
        {
            lock (_queue)
            {
                _queue.Enqueue(package);
                Monitor.PulseAll(_queue);
            }
        }


        private void ShiftFence(byte b)
        {
            Array.Copy(_fence, 1, _fence, 0, _fence.Length - 1);
            _fence[^1] = b;
        }

        private bool IsFence(IEnumerable<byte> b)
        {
            return _fence.SequenceEqual(b);
        }

        public void Add(IEnumerable<byte> data)
        {
            foreach (byte b in data)
            {
                //The fence is an array of the 5 newest bytes, shift the fence, such that it includes the newest byte
                ShiftFence(b);
                //Check if we're in the middle of a package. 
                if (_reading)
                {
                    //Add the new byte to the buffer containing all of the received bytes. 
                    _buffer.Add(b);

                    if (_buffer.Count == Header.HeaderLength)
                    {
                        _header = new Header(_buffer);
                    }


                    //When we've received the full package, check if we have received the end-tag as well. 
                    if (IsFence(Header.EndTag) && _buffer.Count == _header.PackageSize)
                    {
                        //We now have a complete and correct package. 

                        //Ensure that we actually need to split the buffer
                        if (_buffer.Count - Header.EndTagLength <= 0) continue;

                        //Remove the last 5 bytes that represent the end-tag from the buffer
                        _buffer.RemoveRange(_buffer.Count - Header.EndTagLength, Header.EndTagLength);

                        //We now have a complete header and payload - convert it to a package object. 
                        Package package = new(_buffer);

                        //Add package to Queue
                        AddToQueue(package);


                        ResetForNewPackage();
                    }
                    else if (_buffer.Count > _header.PackageSize)
                    {
                        //We now have more bytes that we expected. Something went wrong.
                        ResetForNewPackage();
                        Log.Debug("EndTag was never found - dumbing buffer");
                    }
                }
                else
                {
                    if (IsFence(Header.StartTag))
                    {
                        _reading = true;
                    }
                }
            }
        }

        private void ResetForNewPackage()
        {
            _buffer.Clear();
            _reading = false;
            _header = null;
        }
    }
}
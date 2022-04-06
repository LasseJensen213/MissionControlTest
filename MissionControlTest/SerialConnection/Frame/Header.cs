using System;
using System.Collections.Generic;
using MissionControl.Definitions;
using Serilog;

namespace MissionControl.SerialConnection.Frame
{
    public class Header
    {
        
        public static readonly byte[] StartTag = {0xFD, 0xFF, 0xFF, 0xFF, 0xFF};
        public static readonly byte[] EndTag = {0xFE, 0xFF, 0xFF, 0xFF, 0xFF};
        public const int StartTagLength = 5;
        public const int EndTagLength = 5;
        
        

        //Header Info
        private const int CrcPosition = 0;
        private const int IsAckPosition = 2;
        private const int CommandIdPosition = 3;
        private const int PayloadLengthPosition = 5;

        public const int HeaderLength = 7;
        public const int CrcSize = 2;

        public ushort Crc;
        public bool IsAck;
        public CommandId CommandId;
        public ushort PayloadLength;
        public int PackageSize { get; set; }

        public Header(List<byte> payload)
        {
            if (payload.Count < HeaderLength)
            {
                Log.Debug("Tried to create header with to few bytes: {@Bytes}", payload);
                return;
            }
            
            Crc = BitConverter.ToUInt16(payload.ToArray(), CrcPosition);
            IsAck = BitConverter.ToBoolean(payload.ToArray(), IsAckPosition);
            CommandId = (CommandId) BitConverter.ToUInt16(payload.ToArray(), CommandIdPosition);
            PayloadLength = BitConverter.ToUInt16(payload.ToArray(), PayloadLengthPosition);
            PackageSize = HeaderLength + PayloadLength + EndTag.Length;
        }

    }
}
using System;
using System.Collections.Generic;
using System.Text;
using MissionControl.Definitions;
using MissionControl.Utils;
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
        private const int PayloadLengthPosition = 4;

        public const int HeaderLength = 6;
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
            
            Crc = BitConverter.ToUInt16(payload.ToArray().SubArray(CrcPosition, sizeof(UInt16)), CrcPosition);
            payload.ToArray();
            IsAck = BitConverter.ToBoolean(payload.ToArray(), IsAckPosition);
            CommandId = (CommandId) payload.ToArray()[CommandIdPosition];
            PayloadLength = BitConverter.ToUInt16(payload.ToArray(), PayloadLengthPosition);
            PackageSize = HeaderLength + PayloadLength + EndTag.Length;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("Header: , ");
            builder.Append($"Crc: {Crc}, ");
            builder.Append($"IsAck: {IsAck}, ");
            builder.Append($"CommandId: {CommandId}, ");
            builder.Append($"PayloadLength: {PayloadLength}\n");

            return builder.ToString();
        }
        
       
    }
}
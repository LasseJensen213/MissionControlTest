using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MissionControl.Definitions;
using MissionControl.SerialConnection.Frame;

namespace MissionControl.SerialConnection.Commands
{
    public class Command : ICommand
    {
        public readonly CommandInfo Info;
        public List<byte> Payload;

        public Command(CommandId id, IEnumerable<byte> payload)
        {
            Payload = payload.ToList();
            //TODO Check if this is needed.  
            if (BitConverter.IsLittleEndian) Payload.Reverse();

            Info = CommandDefinitions.Commands[id];
        }


        private object GetValueFromPayload()
        {
            object value = Type.GetTypeCode(Info.Type) switch
            {
                TypeCode.Decimal => BitConverter.ToSingle(Payload.ToArray()),
                TypeCode.UInt32 => BitConverter.ToUInt32(Payload.ToArray()),
                TypeCode.UInt16 => BitConverter.ToUInt16(Payload.ToArray()),
                TypeCode.Int32 => BitConverter.ToInt32(Payload.ToArray()),
                TypeCode.Int16 => BitConverter.ToInt16(Payload.ToArray()),
                TypeCode.Byte => Payload[0],
                TypeCode.Boolean => BitConverter.ToBoolean(Payload.ToArray()),
                _ => throw new ArgumentOutOfRangeException()
            };
            return value;
        }
        
        
        
        

        public byte[] ToByteArray()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(Info);
            return builder.ToString();
        }
    }
}
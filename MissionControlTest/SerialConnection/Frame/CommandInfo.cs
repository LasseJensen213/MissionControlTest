using System;
using MissionControl.Definitions;

namespace MissionControl.SerialConnection.Frame
{
    public class CommandInfo
    {
        public readonly CommandId Id;
        public readonly string Name; 
        public readonly Type Type; // In the case of a custom command, the Type is Object. 
        public readonly int CommandLength;
        public readonly bool IsAcknowledged;

        
        public CommandInfo(CommandId id, Type type, bool isAcknowledged)
        {
            Id = id;
            Type = type;
            IsAcknowledged = isAcknowledged;
            CommandLength = GetSize(type);
            Name = Enum.GetName(typeof(CommandId), id) ?? string.Empty;
        }

        public CommandInfo(CommandId id, int commandLength, bool isAcknowledged)
        {
            Id = id;
            CommandLength = commandLength;
            IsAcknowledged = isAcknowledged;
            Type = typeof(object);
            Name = Enum.GetName(typeof(CommandId), id) ?? string.Empty;
        }

        
        public bool IsCustom()
        {
            return Type.GetTypeCode(Type) == TypeCode.Object;
        }


        
        /**
         * Gets predefined byte lengths for values
         */
        private static int GetSize(Type type)
        {
            int size = Type.GetTypeCode(type) switch
            {
                TypeCode.Decimal => 4,
                TypeCode.UInt32 => 4,
                TypeCode.UInt16 => 2,
                TypeCode.Int32 => 4,
                TypeCode.Int16 => 2,
                TypeCode.Byte => 1,
                TypeCode.Boolean => 1,
                _ => 0
            };

            return size;
        }


    }
}
using System;
using System.Collections.Generic;
using MissionControl.Definitions;
using MissionControl.SerialConnection.Commands;
using MissionControl.Utils;

namespace MissionControl.SerialConnection.Frame
{
    public class Package
    {
        public readonly Header Header;
        private readonly List<Command> _commands;

        public bool IsValid;


        public Package(List<byte> payload)
        {
            Header = new Header(payload);
            _commands = new List<Command>();
            HandlePayload(payload);
        }

        public Package(List<Command> commands)
        {
            _commands = commands;
            IsValid = true;

        }
        


        private void HandlePayload(List<byte> data)
        {
            //Check Crc
            IsValid = ValidateCrc(Header.Crc, data.GetRange(Header.CrcSize, data.Count - Header.CrcSize));
            if (IsValid == false) return;

            //Minus 1 to get the 0-index
            List<byte> payload = data.GetRange(Header.HeaderLength - 1, data.Count - Header.HeaderLength - 1);

            //Step through payload to get all commands
            int index = 0;

            while (index < payload.Count)
            {
                //Extract information from byte list
                CommandInfo info = GetCommandDefinition(payload, ref index);
                IEnumerable<byte> commandPayload = GetCommandPayload(payload, info.CommandLength, ref index);

                _commands.Add(new Command(info.Id, commandPayload));
            }
        }


        private static bool ValidateCrc(ushort crc, IEnumerable<byte> payload)
        {
            ushort calculatedCrc = Crc.Crc16(payload);
            return crc == calculatedCrc;
        }

        private static CommandInfo GetCommandDefinition(List<byte> payload, ref int index)
        {
            CommandId id = (CommandId) BitConverter.ToUInt16(payload.ToArray(), index);
            index += 2;
            return CommandDefinitions.Commands[id];
        }

        private static IEnumerable<byte> GetCommandPayload(List<byte> payload, int commandLength, ref int index)
        {
            List<byte> commandData = payload.GetRange(index, commandLength);
            index += commandLength;
            return commandData;
        }
    }
}
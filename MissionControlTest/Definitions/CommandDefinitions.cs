using System.Collections.Generic;
using System.Diagnostics;
using MissionControl.SerialConnection.Frame;
using static MissionControl.Definitions.CommandId;

namespace MissionControl.Definitions
{
    public static class CommandDefinitions
    {
        public static readonly Dictionary<CommandId, CommandInfo> Commands;


        /**
         * Contains all of our commands.
         * Update as necessary 
         */
        static CommandDefinitions()
        {
            Commands = new Dictionary<CommandId, CommandInfo>();
            InsertCommand(new CommandInfo(CommandId.RocketState, typeof(byte), false));
            InsertCommand(new CommandInfo(StackHealth, typeof(ushort), false));
            InsertCommand(new CommandInfo(BoardMessages, 11, false));
            InsertCommand(new CommandInfo(Time, typeof(int), false));
            InsertCommand(new CommandInfo(Temperature, typeof(int), false));
            Debug.Assert(Enum.GetNames(typeof(CommandId)).Length == Commands.Count, $"CommandDefinitions: Expected to have a command for each CommandID. Expected {Enum.GetNames(typeof(CommandId)).Length} commands but was {Commands.Count} ");
        }


        private static void InsertCommand(CommandInfo info)
        {
            Commands.Add(info.Id, info);
        }
    }
}
using System.Collections.Generic;
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
            InsertCommand(new CommandInfo(RocketState, typeof(byte), false));
            InsertCommand(new CommandInfo(Stackhealth, typeof(ushort), false));
            InsertCommand(new CommandInfo(BoardMessages, 11, false));
        }


        private static void InsertCommand(CommandInfo info)
        {
            Commands.Add(info.Id, info);
        }
    }
}
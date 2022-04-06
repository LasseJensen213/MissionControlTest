using System;
using MissionControl.Definitions;

namespace MissionControl.SerialConnection.Commands
{
    public class TimeCommand : Command
    {
        public int time = 0;
        
        public TimeCommand(int time) : base(CommandId.Time, BitConverter.GetBytes(time))
        {
            this.time = time;
        }
    }
}
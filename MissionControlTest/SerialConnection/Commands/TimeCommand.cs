using System;
using System.Text;
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
        
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(base.ToString());
            builder.Append($", Time: {time}");
            return builder.ToString();
        }
    }
}
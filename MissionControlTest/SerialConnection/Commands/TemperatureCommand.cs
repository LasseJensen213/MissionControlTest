using System;
using System.Text;
using MissionControl.Definitions;

namespace MissionControl.SerialConnection.Commands
{
    public class TemperatureCommand : Command
    {
        private int temperature;
        
        public TemperatureCommand(short temperature) : base(CommandId.Temperature, BitConverter.GetBytes(temperature))
        {
            this.temperature = temperature;
        }


        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(base.ToString());
            builder.Append($", Temperature: {temperature}\n");
            return builder.ToString();
        }
    }
}
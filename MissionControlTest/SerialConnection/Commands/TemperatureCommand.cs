using System;
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
    }
}
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using MissionControl.Definitions;

namespace MissionControl.SerialConnection.Commands
{
    public class RocketStateCommand : Command
    {
        public readonly RocketState State;
        

        public RocketStateCommand(IEnumerable<byte> payload) : base(CommandId.RocketState, payload)
        {
            Debug.Assert(Payload.Count == 1, $"RocketStateCommand: Expected Payload lenght of 1, but was {Payload.Count}");
            State = (RocketState)Payload[0];
        }
        
        
        public RocketStateCommand(RocketState state) : base(CommandId.RocketState, new byte[]{(byte)state})
        {
            State = state;
        }
        
        
        
        
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(base.ToString());
            builder.Append($", RocketState: {State}\n");
            return builder.ToString();
        }
    }
}
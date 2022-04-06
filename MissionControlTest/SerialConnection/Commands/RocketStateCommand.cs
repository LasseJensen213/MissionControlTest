using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MissionControl.Definitions;

namespace MissionControl.SerialConnection.Commands
{
    public class RocketStateCommand : Command
    {
        
        
        
        
        public RocketStateCommand(CommandId id, [NotNull] IEnumerable<byte> payload) : base(id, payload)
        {
        }
    }
}
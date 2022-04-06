using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using MissionControl.Definitions;

namespace MissionControl.SerialConnection.Commands
{
    public class HealthCommand : Command
    {
        public BoardMask Mask;


        public HealthCommand(CommandId id, [NotNull] IEnumerable<byte> payload) : base(id, payload)
        {
            HandlePayload();
        }


        private void HandlePayload()
        {
            Debug.Assert(Info.CommandLength == Payload.Count,
                "HealthCommand: Payload length not equal to Command Length");

            Mask = (BoardMask) BitConverter.ToUInt16(Payload.ToArray(), 0);
        }
    }
}
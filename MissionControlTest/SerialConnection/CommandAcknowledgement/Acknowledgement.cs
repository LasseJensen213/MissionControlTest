using System;
using System.Collections.Generic;
using System.Threading;
using MissionControl.SerialConnection.Commands;
using Serilog;
using Timer = System.Timers.Timer;

namespace MissionControl.SerialConnection.CommandAcknowledgement
{
    //TODO: Sebastian, pls rename to something that makes sense...... 
    public class Acknowledgement
    {   
        private const int ResendTime = 200;
        
        private readonly Queue<Command> _commandQueue;

        private readonly Dictionary<int, Timer> _timers = new();

        public Acknowledgement(Queue<Command> commandQueue)
        {
            _commandQueue = commandQueue;
        }
        

        public void AddCommand(Command command)
        {
            Timer timer = new(ResendTime);
            int counter = 0;
            timer.Elapsed += (sender, e) => ResendCommand((Timer)sender, e, _commandQueue, ref counter, command);
            timer.Start();
            _timers.Add((int)command.Info.Id, timer); //TODO: update
        }


        public void Ack(int id)
        {
            _timers[id].Stop();
            _timers[id].Dispose(); //TODO: Is this necessary?
            _timers.Remove(id);
        }


        private static void ResendCommand(Timer sender, EventArgs e,Queue<Command> commandQueue, ref int counter, Command command)
        {
            Timer timer = (Timer)sender;

            if (counter >= 5)
            {
                Log.Warning("Command was resent 5 times, but not acknowledged. Command {@Command}", command);
                timer.Stop();
            }
            lock (commandQueue)
            {
                commandQueue.Enqueue(command);
            }
            Monitor.PulseAll(commandQueue);
            counter++;
        }
        
    }
}
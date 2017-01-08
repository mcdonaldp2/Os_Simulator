using System;
using Os_Simulator.Enums;

namespace Os_Simulator.Components
{
    /// <summary>
    /// Settings for the os
    /// </summary>
    public class Settings
    {
        public bool DecrementAllIo = false;
        public int CpuDelayTime = 400;
        public int MemorySize = 256;
        public int Quantum = 10;
        public SchedulingAlgorithm SchedulingAlgorithm = SchedulingAlgorithm.RoundRobin;

        public readonly string HelpString = "For more info on a specific command, type HELP <command-name>" +
                      Environment.NewLine + "------------------------------------------------------------------------------" +
                      Environment.NewLine + "PROC \t shows all unfinished processes in the system and their info" + 
                      Environment.NewLine + "MEM \t shows the current usage of memory space" + 
                      Environment.NewLine + "LOAD \t loads a program or job file into the simulator" + 
                      Environment.NewLine + "EXE \t lets the simulation run on its own" +
                      Environment.NewLine + "RESET \t manually resets the simulator" +
                      Environment.NewLine + "SETTINGS \t change some general settings for the simulation" +
                      Environment.NewLine + "STOP \t stops the cpu if it's currently running" +
                      Environment.NewLine + "EXIT \t exits and ends the simulation" + 
                      Environment.NewLine + "HELP \t shows list of all commands in the system" +
                      Environment.NewLine + "GRAPH \t shows graphs of current memory usage and average wait time" + Environment.NewLine + "\t for processes" + 
                      Environment.NewLine;

        /// <summary>
        /// Stats to display on the ui
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "speed \t \t" + this.GetSpeedConversion() + Environment.NewLine + "memory \t \t" +
                   this.MemorySize + Environment.NewLine + "decrementAllIo \t" + this.GetDecIo() + Environment.NewLine +
                   "Scheduling \t" + this.getScheduling();
        }

        /// <summary>
        /// Gets the value of the decIO setting
        /// </summary>
        /// <returns></returns>
        private string GetDecIo()
        {
            if (this.DecrementAllIo)
                return "true";
            return "false";
        }

        /// <summary>
        /// Gets the scheduling algorithm name as a string
        /// </summary>
        /// <returns>name of the currently set scheduling algorithm</returns>
        private string getScheduling()
        {
            return this.SchedulingAlgorithm.ToString();
        }

        /// <summary>
        /// Normalizes speed to a value between 1 and 10
        /// </summary>
        /// <returns></returns>
        private string GetSpeedConversion()
        {
            switch (CpuDelayTime)
            {
                case 0:
                    return "10";
                case 50:
                    return "9";
                case 100:
                    return "8";
                case 150:
                    return "7";
                case 200:
                    return "6";
                case 250:
                    return "5";
                case 300:
                    return "4";
                case 350:
                    return "3";
                case 400:
                    return "2";
                case 500:
                    return "1";
                default:
                    return CpuDelayTime > 500 ? "1" : "500";
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Os_Simulator.Enums;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.ViewManagement;

namespace Os_Simulator.Components
{
    /// <summary>
    /// Simulates the command interface in the os. Commands are passed from the gui to the ci for execution
    /// </summary>
    public class CommandInterface
    {
        private readonly OperatingSystem _os;
        private readonly FileLoader _loader = new FileLoader();

        /// <summary>
        /// Initializes the command interface.
        /// </summary>
        /// <param name="os">A parent os must be given so that the ci has access to any component of the os it needs.</param>
        public CommandInterface(OperatingSystem os)
        {
            this._os = os;
        }

        /// <summary>
        /// Lets the simulation run on its own. The user can also specify the
        /// number of cycles to run before pausing.If there are no processes in the
        /// ready queue that are waiting to be scheduled, EXE will return to the command interface.
        /// </summary>
        /// <param name="userCommand">Command passed from the gui</param>
        public void Exe(string[] userCommand)
        {
            int cycles = 0;
            if (userCommand.Length == 1)
                cycles = 10000;
            else if (userCommand.Length == 2)
                cycles = Int32.Parse(userCommand[1]);

            this._os.Advance(cycles);
        }

        /// <summary>
        /// Simulates the help function on a microsoft cmd line. Allows the user to see more detailed infor about commands, functions, etc...
        /// </summary>
        /// <param name="input">The users input from the command line</param>
        public void Help(string input)
        {
            string[] inputArray = input.Trim().Split(' ');

            if (inputArray.Length == 1)
            {
                this._os.Gui.LogMessageToConsole(this._os.Settings.HelpString);
                return;
            }
            else if (inputArray.Length == 2)
            {
                string toPrint = "-------------------------------------";
                switch (inputArray[1].ToLower().Trim())
                {
                    case "exit":
                        toPrint += "EXIT---------------------------------" + Environment.NewLine;
                        toPrint += "Exits and ends the simulation. The app will be closed and all data lost.";
                        break;
                    case "stop":
                        toPrint += "STOP---------------------------------" + Environment.NewLine;
                        toPrint += "Stops the simulations from running when the exe command is executing." + Environment.NewLine +
                                   "Simulation will remain in the state in which is was stopped in." + Environment.NewLine +
                                   "Processes can be resumed by running the exe command again.";
                        break;
                    case "reset":
                        toPrint += "RESET---------------------------------" + Environment.NewLine;
                        toPrint += "Resets the simulation by emptying all queues," + Environment.NewLine +
                                   "cpu, and resetting clock to 0. All programs in the system will be lost." + Environment.NewLine;
                        break;
                    case "exe":
                        toPrint += "EXE---------------------------------" + Environment.NewLine;
                        toPrint += "Lets the simulation run on its own. The number of cycles can be specified" + Environment.NewLine +
                                   "to run or if no value is given the simulation will run until all programs are" + Environment.NewLine +
                                   "finished";
                        break;
                    case "load":
                        toPrint += "LOAD---------------------------------" + Environment.NewLine;
                        toPrint += "Loads a program or job file into the simulator which allows the scheduler" + Environment.NewLine +
                                   "to allocate memory and place the cooresponding pcb into the" + Environment.NewLine +
                                   "appropriate queue";
                        break;
                    case "mem":
                        toPrint += "MEM---------------------------------" + Environment.NewLine;
                        toPrint += "Shows the current usage of memory space. This value can be changed" + Environment.NewLine +
                                   "with the settings command";
                        break;
                    case "proc":
                        toPrint += "PROC---------------------------------" + Environment.NewLine;
                        toPrint += "Prints all unfinished processes in the system. This includes current process" + Environment.NewLine +
                                   "state, amount of CPU time needed to complete, amount of CPU time" + Environment.NewLine +
                                   "already used, priority (if relevant), and number of I/O requests performed." + Environment.NewLine +
                                   "Note that these same processes can be seen in the queues on the log" + Environment.NewLine +
                                   "screen to the right";
                        break;
                    case "settings":
                        toPrint += "SETTINGS-----------------------------------" + Environment.NewLine;
                        toPrint += "SPEED - sets the speed of the simulation." + Environment.NewLine +
                                   "\t\t Inputs: 1-10, 1=slowest 10=fastest" + Environment.NewLine +
                                   "\t\t Default: 5" + Environment.NewLine +
                                   "MEMORY - sets the size of the memory used by the operating system." + Environment.NewLine + "\t if programs are loaded when this setting" + Environment.NewLine + "\t is changed the simulation will automatically reset" + Environment.NewLine +
                                   "\t\t Inputs: Any int > 0" + Environment.NewLine +
                                   "\t\t Default: 256" + Environment.NewLine +
                                   "DECREMENTALLIO - sets if the io queue decrements all of its items every" + Environment.NewLine +
                                   "\tcc or only decrement the top of the queue" + Environment.NewLine +
                                   "\t\t Inputs: true/false" + Environment.NewLine +
                                   "\t\t Default: false" + Environment.NewLine +
                                   "SCHEDULING - sets the algorithm used by the scheduler. When this" + Environment.NewLine +
                                   "\t setting is changed the simulator will reset" + Environment.NewLine +
                                   "\t\t Inputs: RoundRobin, FirstComeFirstServe," + Environment.NewLine + "\t\t\tShortestJobFirst" + Environment.NewLine +
                                   "\t\t Default: RoundRobin";

                        break;
                    case "graph":
                        toPrint += "GRAPH--------------------------------------" + Environment.NewLine;
                        toPrint += "Shows a bar chart of the current simulation's memory usage," + Environment.NewLine + "along with a line graph of " + Environment.NewLine + "The average waiting time for a process V. Total Cycles run";
                        break;
                    default:
                        this._os.Gui.LogMessageToConsole("Help command not recognized.");
                        return;
                }

                toPrint += Environment.NewLine + "----------------------------------------------------------------------------------";
                this._os.Gui.LogMessageToConsole(toPrint);

            }
            else
            {
                this._os.Gui.LogMessageToConsole("Help command not recognized.");
            }
        }

        /// <summary>
        /// Takes a program file name and creates a ProcessControlBlock from the program file
        /// </summary>
        /// <param name="user_command"> user command entered into the command line</param>
        /// <returns>
        /// A ProcessControlBlock created from the program file if a program file is found/
        /// If a program file is not found then returns null;
        /// </returns>
        public async Task<ProcessControlBlock> Load(string[] user_command)
        {
            if (user_command.Length < 2)
                return null;
            else
            {
                var newProcess = await _loader.LoadFile(user_command[1]);

                this._os.Gui.LogMessageToConsole("pcb created with pId: " + newProcess.PId);
                return newProcess;
            }
        }

        /// <summary>
        /// Shows the current usage of memory space.
        /// </summary>
        public void Mem()
        {
            var memUsage = this._os.Scheduler.GetCurrentMemoryUsage();

            this._os.Gui.LogMessageToConsole("");
            this._os.Gui.LogMessageToConsole("Currently using " + memUsage + "/" + this._os.Settings.MemorySize + " kB of memory");
        }

        /// <summary>
        /// Shows all unfinished processes in the system and their
        /// information.  The process information should include: current process
        /// state, amount of CPU time needed to complete, amount of CPU time
        /// already used, priority (if relevant), number of I/O requests performed.
        /// </summary>
        public void Proc()
        {
            var ready = this._os.Scheduler.ReadyQueue.GetCurrentQueueAsList();
            var waiting = this._os.Scheduler.WaitingQueue.GetCurrentQueueAsList();

            List<string> messages = new List<string>();

            foreach (var pcb in ready)
            {
                messages.Add(pcb.ToStringForProc());
            }

            foreach (var pcb in waiting)
            {
                messages.Add(pcb.ToStringForProc());
            }

            this._os.Gui.LogMessageToConsole("CURRENT PROCESSES");
            this._os.Gui.LogMessageToConsole("=================");

            foreach (var message in messages)
            {
                this._os.Gui.LogMessageToConsole(message);
            }

            this._os.Gui.LogMessageToConsole("=================");

        }

        /// <summary>
        /// Clears all queues and reset clock cycles. Does not change any settings
        /// </summary>
        public void Reset()
        {
            this._os.Reset();
        }

        /// <summary>
        /// Allows the user to change some basic settings in the simulation
        /// </summary>
        /// <param name="input">Command line input</param>
        public void Settings(string input)
        {
            string[] inputArray = input.Split(' ');

            if (inputArray.Length == 1)
            {
                this._os.Gui.LogMessageToConsole(this._os.Settings.ToString());
                return;
            }
            else if (inputArray.Length != 3)
            {
                this._os.Gui.LogMessageToConsole("Settings command not recognized");
                return;
            }

            switch (inputArray[1].ToLower())
            {
                case "speed":
                    int j;
                    if (Int32.TryParse(inputArray[2], out j))
                    {
                        switch (j)
                        {                           
                            case 10:
                                this._os.Settings.CpuDelayTime = 0;
                                break;
                            case 9:
                                this._os.Settings.CpuDelayTime = 50;
                                break;
                            case 8:
                                this._os.Settings.CpuDelayTime = 100;
                                break;
                            case 7:
                                this._os.Settings.CpuDelayTime = 150;
                                break;
                            case 6:
                                this._os.Settings.CpuDelayTime = 200;
                                break;
                            case 5:
                                this._os.Settings.CpuDelayTime = 250;
                                break;
                            case 4:
                                this._os.Settings.CpuDelayTime = 300;
                                break;
                            case 3:
                                this._os.Settings.CpuDelayTime = 350;
                                break;
                            case 2:
                                this._os.Settings.CpuDelayTime = 400;
                                break;
                            case 1:
                                this._os.Settings.CpuDelayTime = 500;
                                break;
                            default:
                                this._os.Settings.CpuDelayTime = j > 0 ? 50 : 500;
                                break;
                        }
                    }
                    else
                        this._os.Gui.LogMessageToConsole("Settings command not recognized");

                    break;
                case "memory":
                    int m;
                    if (Int32.TryParse(inputArray[2], out m))
                    {
                        this._os.UserInterrupt = true;
                        this._os.Settings.MemorySize = m;
                        this.Reset();
                    }

                    break;
                case "scheduling":
                    this._os.UserInterrupt = true;
                    if (inputArray[2] != null)
                    {
                        if (inputArray[2].ToLower().Equals("shortestjobfirst"))
                        {
                            this._os.Scheduler.ReadyQueue.IsShortestJobFirst = true;
                            this._os.Scheduler.WaitingQueue.IsShortestJobFirst = true;
                            this._os.Settings.SchedulingAlgorithm = SchedulingAlgorithm.ShortestJobFirst;                          
                        }
                        else if (inputArray[2].ToLower().Equals("roundrobin"))
                        {
                            this._os.Scheduler.ReadyQueue.IsShortestJobFirst = false;
                            this._os.Scheduler.WaitingQueue.IsShortestJobFirst = false;
                            this._os.Settings.SchedulingAlgorithm = SchedulingAlgorithm.RoundRobin;                          
                        }else if (inputArray[2].ToLower().Equals("firstcomefirstserve"))
                        {
                            this._os.Scheduler.ReadyQueue.IsShortestJobFirst = false;
                            this._os.Scheduler.WaitingQueue.IsShortestJobFirst = false;
                            this._os.Settings.SchedulingAlgorithm = SchedulingAlgorithm.FirstComeFirstServe;
                        }
                        else
                        {
                            this._os.Gui.LogMessageToConsole("Scheduling Algorithm not recognized");
                            return;
                        }

                        this.Reset();
                    }
                    else
                        this._os.Gui.LogMessageToConsole("Scheduling Algorithm not recognized");

                    break;
            }
        }


        public async void ShowGraph()
        {
            var newView = CoreApplication.CreateNewView();
            int newViewId = 0;

            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Frame frame = new Frame();
                frame.Navigate(typeof(Graph), _os);
                Window.Current.Content = frame;
                // You have to activate the window in order to show it later.
                Window.Current.Activate();

                newViewId = ApplicationView.GetForCurrentView().Id;
            });
            bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
            
        }
    }
}

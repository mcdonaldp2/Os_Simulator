using Os_Simulator.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.System;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Os_Simulator
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GUI : Page
    {
        private string _simDir = "C:\\Users\\sim_os>";
        private int _guiLineCounter = 25;
        private int _lineBreakOn = 25;

        private OperatingSystem _os;
        public int PreviousCommandIndex;
        public List<string> PreviousCommands = new List<string>();

        /// <summary>
        /// Init the component
        /// </summary>
        public GUI()
        {
            //since the gui serves as the entry point for the app, pass it's scope to the OS class and treat 
            // the gui as a subclass of OS
            this._os = new OperatingSystem(this);

            this.PreviousCommandIndex = 0;
            this.InitializeComponent();
            ApplicationView.GetForCurrentView().TryResizeView(new Size { Width = 1280, Height = 720 });
            this.UpdateMemoryLabel();

            for (int i = 0; i < this._lineBreakOn; i++)
            {
                TextBlock txtNumber = new TextBlock();
                txtNumber.Foreground = new SolidColorBrush(Colors.White);
                txtNumber.Text = "";

                this.gui_main_stack.Children.Insert(0, txtNumber);
            }
        }


        /// <summary>
        /// Exit the simulation
        /// </summary>
        private void Exit()
        {
            Application.Current.Exit();
        }

        /// <summary>
        /// Handles the load command
        /// </summary>
        /// <param name="user_input"></param>
        private async void HandleLoad(string user_input)
        {

            if (user_input.Split(' ').Length < 2)
            {
                LogMessageToConsole("No File name was included");
            }
            else
            {
                var newProcess = await this._os.UserInterface.Load(user_input.Split(' '));

                if (newProcess == null)
                {
                    LogMessageToConsole("Error Loading file");
                }
                else
                {
                    this._os.Scheduler.AddNewPCB(newProcess);
                    //todo call CPU memorymanager to determine if the process can be added to ready or waiting queue
                    //cpu.addNewPCB(newProcess);
                }
            }
        }

        /// <summary>
        /// Updates cc on ui
        /// </summary>
        /// <param name="cycle"></param>
        public void LogClockCycle(int cycle)
        {
            string label = "CPU Clock Cycle: ";
            this.cpuClockCycleLabel.Text = label + cycle;
        }

        /// <summary>
        /// Log last command on the ui
        /// </summary>
        private void LogEntryToConsole()
        {
            PreviousCommands.Add(gui_input.Text);
            PreviousCommandIndex = PreviousCommands.Count - 1;

            //create a copy of the command to log to console
            TextBlock txtNumber = new TextBlock();
            txtNumber.TextWrapping = TextWrapping.Wrap;
            txtNumber.Foreground = new SolidColorBrush(Colors.White);
            txtNumber.Text = this._simDir + gui_input.Text;

            //clear the input line
            gui_input.Text = "";

            if (this._guiLineCounter > _lineBreakOn)
            {
                gui_main_stack.Children.Remove(gui_main_stack.Children[1]);
                gui_main_stack.Children.Insert(_lineBreakOn, txtNumber);
            }
            else
            {
                gui_main_stack.Children.Insert(this._guiLineCounter, txtNumber);
            }

            this._guiLineCounter++;
        }

        /// <summary>
        /// Log a given message to the ui
        /// </summary>
        /// <param name="message"></param>
        public void LogMessageToConsole(string message)
        {
            //break the message down into seperate lines
            string[] messageArray = message.Split('\n');

            //TODO:(Matt) This is a hack to stop the scrolling at some point before it goes of the page. this should be changed to be dynamic or something        
            for (int i = 0; i < messageArray.Length; i++)
            {
                TextBlock txtNumber = new TextBlock();
                txtNumber.Foreground = new SolidColorBrush(Colors.White);
                txtNumber.Text = messageArray[i].Replace("\r", "");

                this.gui_main_stack.Children.RemoveAt(0);
                this.gui_main_stack.Children.Insert(_lineBreakOn, txtNumber);
            }
        }

        /// <summary>
        /// Parse the input string
        /// </summary>
        /// <param name="user_input"></param>
        private void ParseInput(string user_input)
        {
            string user_command = user_input.ToUpper().Split(' ')[0];

            string[] commands = new string[] { "PROC", "MEM", "LOAD", "EXE", "RESET", "EXIT", "STOP", "HELP", "SETTINGS", "GRAPH"};

            string found_command = commands.FirstOrDefault<string>(s => user_command.Equals(s));

            switch (found_command)
            {
                case "PROC":
                    LogEntryToConsole();
                    this._os.UserInterface.Proc();
                    //handleProc(messages);
                    break;
                case "MEM":
                    LogEntryToConsole();
                    this._os.UserInterface.Mem();
                    break;
                case "LOAD":
                    LogEntryToConsole();
                    HandleLoad(user_input);
                    break;
                case "EXE":
                    if (this._os.CpuIsRunning)
                        this.LogMessageToConsole(@"run stop command, wait until all programs are processed, 
                                                   or wait until inputed cycles have finished");
                    LogEntryToConsole();
                    this._os.UserInterface.Exe(user_input.Trim().Split(' '));
                    //this.os.advance(cycles);
                    break;
                case "RESET":
                    LogEntryToConsole();
                    this._os.UserInterface.Reset();
                    break;
                case "SETTINGS":
                    LogEntryToConsole();
                    this._os.UserInterface.Settings(user_input);
                    break;
                case "STOP":
                    this._os.UserInterrupt = true;
                    LogEntryToConsole();
                    break;
                case "HELP":
                    LogEntryToConsole();
                    this._os.UserInterface.Help(user_input);
                    //LogMessageToConsole(this.os.settings.helpString);
                    break;
                case "EXIT":
                    LogEntryToConsole();
                    Exit();
                    break;
                case "GRAPH":
                    LogEntryToConsole();
                    this._os.UserInterface.ShowGraph();
                    break;
                default:
                    LogEntryToConsole();
                    LogMessageToConsole(String.Format("'{0}' is not recognized as an internal or external command, operable program or batch file.", user_input));
                    break;
            }
        }

        /// <summary>
        /// Resets the ui by zeroing all data and clearing all field
        /// </summary>
        public void Reset()
        {
            this.UpdateCurrentPCBTable("");
            this.UpdateAllocatedMemoryLabel(0);

            this.UpdateMemoryLabel();
            this.UpdateReadyQueueLog();
            this.UpdateWaitingQueueLog();
            this.UpdateIoQueueLog();
            this.UpdateNumberPCBsInMemory();
            this.UpdateAvgWaitTime(0, 0);
            this.UpdateNumberPCBsInMemory();
            this.UpdateTotalJobs(0);
        }

        /// <summary>
        /// Detects actions on key. Used for keyboard shortcuts
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UiKeyUp(object sender, KeyRoutedEventArgs e)
        {
            //short cuts for up key
            if (e.Key == VirtualKey.Up)
            {
                //for testing only
                if (this.gui_input.Text.Equals("load program1.txt"))
                {
                    this.gui_input.Text = "exe 1000";
                }
                else
                {
                    this.gui_input.Text = "load program1.txt";
                }

                //previousCommandIndex++;
                //if (previousCommandIndex < previousCommands.Count)
                //{
                //    gui_input.Text = previousCommands.ElementAt(previousCommandIndex);
                //}
                //else
                //{
                //   previousCommandIndex--;
                //
                //   if (previousCommands.Count > 0)
                //   {
                //       gui_input.Text = previousCommands.ElementAt(previousCommandIndex);
                //  }
                // }
                // return;
            }

            if (e.Key == VirtualKey.Down)
            {
                PreviousCommandIndex--;
                if (PreviousCommandIndex >= 0)
                {
                    gui_input.Text = PreviousCommands.ElementAt(PreviousCommandIndex);
                }
                else
                {
                    PreviousCommandIndex = 0;
                    if (PreviousCommands.Count > 0)
                    {
                        gui_input.Text = PreviousCommands.ElementAt(PreviousCommandIndex);
                    }
                }
                return;
            }

            //if key makes it past here the user has entered data
            if (e.Key != VirtualKey.Enter)
                return;

            ParseInput(gui_input.Text);

            //LogEntryToConsole();     
        }     

        /******************Methods to update ui******************************/
        public void UpdateAvgWaitTime(double time, int jobs)
        {
            this.avgWaitTime.Text = "Avg. Wait of Completed Jobs:   " + time;
            this.jobsCompleted.Text = "Jobs Completed:    " + jobs;
        }

        public void UpdateAllocatedMemoryLabel(int mem)
        {
            this.allocatedMemoryLabel.Text = "Allocated Memory:     " + mem;
        }

        public void UpdateCurrentPCBTable(string messsage)
        {
            this.currentPCB.Text = messsage;
        }

        public void UpdateIoQueueCycles()
        {
            List<int> cyclesList = new List<int>();

            foreach (var x in this._os.IoScheduler.GetList())
            {
                cyclesList.Add(x.CurrentOperation.Cycles);
            }

            this.inIoQueueCycles.ItemsSource = cyclesList;


        }

        public void UpdateIoQueueLog()
        {
            List<int> idList = new List<int>();
            List<int> cyclesList = new List<int>();
            List<int> ioList = new List<int>();
            List<string> ioTypeList = new List<string>();

            foreach (var x in this._os.IoScheduler.GetList())
            {
                idList.Add(x.PId);
                cyclesList.Add(x.CurrentOperation.Cycles);
                ioList.Add(x.IoRequestsPerformed);
                ioTypeList.Add(x.CurrentOperation.IoType.ToString());
            }


            this.inIoQueueCycles.ItemsSource = cyclesList;
            this.inIoQueueList.ItemsSource = idList;
            this.inIoQueueIoType.ItemsSource = ioTypeList;
        }

        public void UpdateNumberPCBsInMemory()
        {
            int counter = this._os.Scheduler.ReadyQueue.Size + this._os.IoScheduler.Size();

            if (this._os.Cpu.CurrentPcb != null)
                counter += 1;

            this.numberOfProcessesInMemory.Text = "# PCBS in Memory:     " + counter;
        }

        public void UpdateMemoryLabel()
        {
            this.totalMemoryLabel.Text = "   Total Memory:     " + this._os.Settings.MemorySize;
        }

        public void UpdateReadyQueueLog() {
            List<int> idList = new List<int>();
            List<int> sizeList = new List<int>();
            List<int> ioList = new List<int>();
            List<int> cpuUsedList = new List<int>();
            List<string> operationList = new List<string>();
            List<int> cyclesLeftList = new List<int>();

            foreach (var x in this._os.Scheduler.ReadyQueue.Pcbs) {
                idList.Add(x.PId);
                sizeList.Add(x.MemoryRequirement);
                ioList.Add(x.IoRequestsPerformed);
                cpuUsedList.Add(x.TimeUsed);
                operationList.Add(x.CurrentOperation.Type.ToString());
                cyclesLeftList.Add(x.TotalCyclesNeeded);
            }

            this.inReadyQueueSize.ItemsSource = sizeList;
            this.inReadyQueueList.ItemsSource = idList;
            this.inReadyQueueIoRequests.ItemsSource = ioList;
            this.inReadyQueueCpuUsed.ItemsSource = cpuUsedList;
            this.inReadyQueueOperation.ItemsSource = operationList;
            this.inReadyQueueCyclesLeft.ItemsSource = cyclesLeftList;
        }

        public void UpdateTotalJobs(int jobs)
        {
            this.totalJobs.Text = "Total Jobs:     " + jobs;
        }

        public void UpdateWaitingQueueLog()
        {
            List<int> idList = new List<int>();
            List<int> sizeList = new List<int>();
            List<int> ioList = new List<int>();
            List<int> cyclesLeft = new List<int>();

            foreach (var x in this._os.Scheduler.WaitingQueue.Pcbs)
            {
                idList.Add(x.PId);
                sizeList.Add(x.MemoryRequirement);
                ioList.Add(x.IoRequestsPerformed);
                cyclesLeft.Add(x.TotalCyclesNeeded);
            }


            this.inWaitingQueueSize.ItemsSource = sizeList;
            this.inWaitingQueueList.ItemsSource = idList;
            this.inWaitingQueueIoRequests.ItemsSource = ioList;
            this.inWaitingQueueCyclesLeft.ItemsSource = cyclesLeft;
        }

        private void textBlock13_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }
    }
}

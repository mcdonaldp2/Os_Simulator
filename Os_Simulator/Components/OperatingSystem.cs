using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;
using Os_Simulator.Enums;

namespace Os_Simulator.Components
{
    /// <summary>
    /// Simulates an operating system
    /// </summary>
    public class OperatingSystem
    {
        /// <summary>
        /// Components used by os
        /// </summary>
        public CPU Cpu;
        public CommandInterface UserInterface;
        public Scheduler Scheduler;
        public GUI Gui;
        public IoScheduler IoScheduler;
        public Settings Settings = new Settings();
        public InterruptProcessor InterruptProcessor;
        public bool CpuIsRunning = false;
        public bool UserInterrupt = false;
        public int totalCycles;

        /// <summary>
        /// Init the os and make the gui a child of the os
        /// </summary>
        /// <param name="gui"></param>
        public OperatingSystem(GUI gui) {
            //create all the components of the os   
            this.Gui = gui;
            this.IoScheduler = new IoScheduler(this);
            this.Cpu = new CPU(this);
            this.UserInterface = new CommandInterface(this);
            this.Scheduler = new Scheduler(this);
            this.InterruptProcessor = new InterruptProcessor(this);
            totalCycles = 0;
        }

        /// <summary>
        /// Advances the os, checks for events and updates ui
        /// </summary>
        /// <param name="cycles"></param>
        public async void Advance(int cycles)
        {
            this.Gui.UpdateWaitingQueueLog();

            bool allCommandsFinished = false;
            while (cycles > 0 && !allCommandsFinished && !this.UserInterrupt)
            {
                this.CpuIsRunning = true;

                //delays cpu from executing almost instantly
                await Task.Delay(this.Settings.CpuDelayTime);

                // there is no process in the cpu but there are in the ready queue so 
                // create an context switch event

                this.CreateNeededEvents();

                //if there are ios waiting decrement their counters
                if (this.IoScheduler.HasIo())
                    this.IoScheduler.StartIo();

                //interrupt the cycle to handle any events
                if (this.InterruptProcessor.SignalInterrupt())
                {
                    while (this.InterruptProcessor.HasInterrupt)
                    {
                        this.HandleEvent(this.InterruptProcessor.GetEvent());
                    }
                }

                this.Cpu.AdvanceClock();
                totalCycles++;

                this.Scheduler.IncrementQuantumCounter();

                cycles--;

                if (this.Cpu.CurrentPcb == null && Scheduler.ReadyQueue.Size == 0 && IoScheduler.Size() == 0)
                    allCommandsFinished = true;

                this.Gui.UpdateNumberPCBsInMemory();

                this.IncrementWaitTimes();
            }

            if(this.Scheduler.WaitingQueue.Size == 0 && this.Scheduler.ReadyQueue.Size == 0 && !this.IoScheduler.HasIo())
                this.Gui.UpdateAllocatedMemoryLabel(0);

            this.Gui.UpdateAvgWaitTime(this.GetAvgWaitTimes(), this.Scheduler.FinishedPcbs.Count);

            this.CpuIsRunning = false;
            this.UserInterrupt = false;

            this.Gui.UpdateCurrentPCBTable("");
            //this.gui.UpdateAllocatedMemoryLabel();


        }

        /// <summary>
        /// Gets the total memory allocated in the os
        /// </summary>
        public int AllocatedMemory
        {
            get
            {
                var total = this.IoScheduler.TotalMemory + this.Scheduler.ReadyQueue.TotalMemory;

                return this.Cpu.CurrentPcb != null ? total += this.Cpu.CurrentPcb.MemoryRequirement : total;
            }
        }

        /// <summary>
        /// Creates any events if needed by the os
        /// </summary>
        public void CreateNeededEvents()
        {
            if (this.Cpu.CurrentPcb == null && this.Scheduler.ReadyQueue.Size == 0 && this.Scheduler.WaitingQueue.Size == 0 && !this.IoScheduler.HasIo())
                this.InterruptProcessor.CreateAndAddEvent(EventType.Done);
            else if (this.Cpu.CurrentPcb == null && this.Scheduler.ReadyQueue.Size > 0)
                this.InterruptProcessor.CreateAndAddEvent(EventType.ContextSwitch);
        }

        /// <summary>
        /// Get the average wait time of pcbs
        /// </summary>
        /// <returns></returns>
        public double GetAvgWaitTimes()
        {
           

            if (this.Scheduler.FinishedPcbs.Count == 0)
                return 0;

            int waitTime = 0;
            foreach (ProcessControlBlock pcb in this.Scheduler.FinishedPcbs)
            {
                waitTime += pcb.WaitTime;
            }

            return waitTime / this.Scheduler.FinishedPcbs.Count;


        }

        /// <summary>
        /// Handles events and makes takes the corresponding actions
        /// </summary>
        /// <param name="ecb"></param>
        public void HandleEvent(EventControlBlock ecb)
        {
            switch (ecb.Type)
            {
                case EventType.ReleaseIO:
                    //get the pcb from the ioscheduler
                    ProcessControlBlock pcb = this.IoScheduler.GetPcb(ecb.PcbId);
                    //move it back to the ready queue
                    this.Scheduler.ReadyQueue.EnQueue(pcb);
                    this.Gui.UpdateIoQueueLog();
                    this.Gui.UpdateReadyQueueLog();
                    //updateUI = true;
                    break;
                case EventType.ContextSwitch:
                    this.Scheduler.ContextSwitch();
                    this.Scheduler.UpdateReadyQueue();
                    //this.gui.UpdateCurrentPCBTable(this.cpu.currentPcb.ToString());
                    this.Gui.UpdateReadyQueueLog();
                    this.Gui.UpdateAllocatedMemoryLabel(this.AllocatedMemory);
                    this.Gui.UpdateAvgWaitTime(this.GetAvgWaitTimes(), this.Scheduler.FinishedPcbs.Count);
                    break;
                case EventType.NewShortestFirstPCB:
                    //check if anything in the waiting queue should be moved into the ready queue
                    if (this.Scheduler.WaitingQueue.Size > 0)
                    {
                        var toCheck = this.Scheduler.WaitingQueue.DeQueue();

                        while (this.UpdateQueues(toCheck) && this.Scheduler.WaitingQueue.Size > 0)
                            toCheck = this.Scheduler.WaitingQueue.DeQueue();
                    }

                    this.Gui.UpdateWaitingQueueLog();
                    break;
                case EventType.Done:
                    this.UserInterrupt = true;
                    break;
            }
        }

        private int Helper(List<ProcessControlBlock> list)
        {
            int counter = 0;
            foreach (ProcessControlBlock block in list)
            {
                counter += block.MemoryRequirement;
            }

            return counter;
        }

        /// <summary>
        /// Increments all pcbs wait time param except the pcb currently in the cpu
        /// </summary>
        public void IncrementWaitTimes()
        {
            this.Scheduler.ReadyQueue.IncrementWaitTimes();
            this.Scheduler.WaitingQueue.IncrementWaitTimes();
            this.IoScheduler.IncrementWaitTimes();
        }

        /// <summary>
        /// Clears everything from os and resets
        /// </summary>
        public void Reset()
        {
            this.Cpu.Reset();
            this.Scheduler.Reset();
            this.IoScheduler.Reset();
            this.InterruptProcessor.Reset();
            this.CpuIsRunning = false;
            this.UserInterrupt = false;
            this.Gui.Reset();
            this.totalCycles = 0;
        }

        /// <summary>
        /// Decides which queue a pcb goes in in shortest job first mode
        /// </summary>
        /// <param name="pcb"></param>
        /// <returns></returns>
        public bool UpdateQueues(ProcessControlBlock pcb)
        {
            // get a side list of all pcbs in memory
            List<ProcessControlBlock> allInMemory = new List<ProcessControlBlock>();

            allInMemory.Add(pcb);

            //add anything in cpu to temp list
            if (this.Cpu.CurrentPcb != null)
                allInMemory.Add(this.Cpu.CurrentPcb);

            //add all pcbs in ready queue to temp list
            foreach (ProcessControlBlock p in this.Scheduler.ReadyQueue.Pcbs)
                allInMemory.Add(p);

            //add all io to temp list
            foreach (ProcessControlBlock p in this.IoScheduler.GetAllPcBs())
                allInMemory.Add(p);

            //order everything in the list by number of cc's required
            allInMemory = allInMemory.OrderBy(p => p.TotalCyclesNeeded).ToList();

            //remove everything below the new pcb in the queue and see if it would fit in memory
            List<ProcessControlBlock> removed = new List<ProcessControlBlock>();

            while (pcb.PId != allInMemory[allInMemory.Count - 1].PId && this.Helper(allInMemory) > this.Settings.MemorySize)
            {
                removed.Add(allInMemory[allInMemory.Count - 1]);
                allInMemory.RemoveAt(allInMemory.Count - 1);
            }

            //add up the memory in the temp array
            int memory = 0;
            foreach (ProcessControlBlock block in allInMemory)
                memory += block.MemoryRequirement;

            //if the new pcb will fit in the queue actually do the stuff
            if (memory <= this.Settings.MemorySize && removed.Count > 0)
            {
                foreach (ProcessControlBlock block in removed)
                {
                    //if the pcb to be removed was in the io scheduler move it to the waiting queue
                    if (this.IoScheduler.IsInIoScheduler(block))
                    {
                        ProcessControlBlock pcbFromIo = this.IoScheduler.RemoveFromIoScheduler(block);
                        this.Scheduler.Memory.FreeMemory(pcbFromIo.MemoryRequirement);
                        this.Scheduler.WaitingQueue.EnQueue(pcbFromIo);
                    }
                    //if the pcb to be removed is in the cpu move it to the waiting queue and create an event to switch context
                    else if (this.Cpu.CurrentPcb != null && this.Cpu.CurrentPcb.PId == pcb.PId)
                    {
                        this.Scheduler.WaitingQueue.EnQueue(this.Cpu.CurrentPcb);
                        this.Scheduler.Memory.FreeMemory(this.Cpu.CurrentPcb.MemoryRequirement);
                        this.Cpu.CurrentPcb = null;

                        this.InterruptProcessor.CreateAndAddEvent(EventType.ContextSwitch);
                    }
                    // the pcb has to be in the ready queue so move it to the waiting queue
                    else
                    {
                        for (int j = 0; j < this.Scheduler.ReadyQueue.Pcbs.Count; j++)
                        {
                            if (this.Scheduler.ReadyQueue.Pcbs[j].PId == block.PId)
                            {

                                this.Scheduler.WaitingQueue.EnQueue(block);
                                this.Scheduler.ReadyQueue.Pcbs.RemoveAt(j);
                                this.Scheduler.Memory.FreeMemory(block.MemoryRequirement);
                            }
                        }
                    }
                }

                //space has been freed up for the new pcb to be added
                this.Scheduler.ReadyQueue.EnQueue(pcb);
                this.Scheduler.Memory.AssignMemory(pcb.MemoryRequirement);

                this.Gui.UpdateIoQueueLog();
                this.Gui.UpdateReadyQueueLog();
                this.Gui.UpdateWaitingQueueLog();

                return true;
            }
            else if (memory <= this.Settings.MemorySize && removed.Count == 0)
            {
                this.Scheduler.ReadyQueue.EnQueue(pcb);
                this.Scheduler.Memory.AssignMemory(pcb.MemoryRequirement);
                this.Gui.UpdateReadyQueueLog();
                
                return true;
            }
            else
            {
                this.Scheduler.WaitingQueue.EnQueue(pcb);
                this.Gui.UpdateWaitingQueueLog();

                return false;
            }
        }    
        
    }
}

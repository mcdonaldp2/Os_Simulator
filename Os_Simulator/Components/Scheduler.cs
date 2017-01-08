using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Os_Simulator.Enums;

namespace Os_Simulator.Components
{
    public class Scheduler
    {
        private OperatingSystem _os;
        public int QuantumCounter = 0;
        public ExecutionQueue ReadyQueue = new ExecutionQueue();
        public ExecutionQueue WaitingQueue = new ExecutionQueue();

        public List<ProcessControlBlock> FinishedPcbs = new List<ProcessControlBlock>();
        public int JobCount = 0;

        public MemoryManagement Memory;

        /// <summary>
        /// Init the component giving it permissions to the os
        /// </summary>
        /// <param name="os"></param>
        public Scheduler(OperatingSystem os) {
            this._os = os;
            this.Memory = new MemoryManagement(os);           
        }

        /// <summary>
        /// Add a new pcb to the os. Scheduler decides where to put it based on the currernt scheduling algorithm
        /// </summary>
        /// <param name="pcb"></param>
        public void AddNewPCB(ProcessControlBlock pcb)
        {
            JobCount++;
            this._os.Gui.UpdateTotalJobs(JobCount);

            if (this._os.Settings.SchedulingAlgorithm == SchedulingAlgorithm.RoundRobin || this._os.Settings.SchedulingAlgorithm == SchedulingAlgorithm.FirstComeFirstServe)
                this.AddPcbRoundRobin(pcb);
            else if (this._os.Settings.SchedulingAlgorithm == SchedulingAlgorithm.ShortestJobFirst)
                this.AddPcbShortestJobFirst(pcb);
        }

        /// <summary>
        /// Adds a pcb to the system when using round robin scheduling
        /// </summary>
        /// <param name="pcb"></param>
        public void AddPcbRoundRobin(ProcessControlBlock pcb)
        {
            var canAdd = this.Memory.CanAllocate(pcb.MemoryRequirement);

            //add PCB to ready queue
            if (canAdd)
            {
                pcb.State = State.NEW;
                this.ReadyQueue.EnQueue(pcb);
                this.Memory.AssignMemory(pcb.MemoryRequirement);
            }
            else
            { // add PCB to waiting queue
                pcb.State = State.NEW;
                WaitingQueue.EnQueue(pcb);
                this._os.Gui.UpdateWaitingQueueLog();
            }
        }

        /// <summary>
        /// Adds a pcb to the system when using sjf scheduling
        /// </summary>
        /// <param name="pcb"></param>
        public void AddPcbShortestJobFirst(ProcessControlBlock pcb)
        {

            //add the pcb to the waiting queue and create an event to check if it should be moved into the ready queue
            this.WaitingQueue.EnQueue(pcb);
            this._os.InterruptProcessor.CreateAndAddEvent(EventType.NewShortestFirstPCB);


        }

        /// <summary>
        /// Simulates a context switch in which the scheduler preempts a process and adds the next
        /// </summary>
        public void ContextSwitch()
        {
            if (this._os.Cpu.CurrentPcb == null && this.ReadyQueue.Size > 0)
                this._os.Cpu.CurrentPcb = this.ReadyQueue.DeQueue();

            if (this._os.Cpu.CurrentPcb != null && _os.Scheduler.ReadyQueue.Size > 0)
            {
                this.ReadyQueue.EnQueue(this._os.Cpu.CurrentPcb);
                this._os.Cpu.CurrentPcb = this._os.Scheduler.ReadyQueue.DeQueue();

            }
            this.UpdateReadyQueue();
        }

        /// <summary>
        /// Gets the total memory in the scheduler
        /// </summary>
        /// <returns></returns>
        public int GetCurrentMemoryUsage()
        {
            return this.Memory.GetCurrentMemoryUsage();
        }

        /// <summary>
        /// Increment the quantum. Creates event if context switch is needed
        /// </summary>
        public void IncrementQuantumCounter() {
            this.QuantumCounter++;

            if (this.QuantumCounter == this._os.Settings.Quantum && this._os.Settings.SchedulingAlgorithm == SchedulingAlgorithm.RoundRobin)
            {
                this._os.InterruptProcessor.CreateAndAddEvent(EventType.ContextSwitch);
                this.QuantumCounter = 0;
            }
        }

        /// <summary>
        /// Clear the queues and reset the scheduler
        /// </summary>
        public void Reset()
        {
            this.QuantumCounter = 0;
            this.ReadyQueue.Reset();
            this.WaitingQueue.Reset();
            this.FinishedPcbs = new List<ProcessControlBlock>();
            this.JobCount = 0;
            this.Memory.Reset();
        }

        /// <summary>
        /// Simulates the long term scheduler moving items from waiting queue to ready queue
        /// </summary>
        /// <returns>True if something was added to ready queue and false if not</returns>
        public bool UpdateReadyQueue() {
            //theres nothing in the waiting queue
            if (this.WaitingQueue.Size <= 0)
                return false;

            //theres something in the waiting queue and it can fit into memory
            if (this.Memory.CanAllocate(this.WaitingQueue.Peek().MemoryRequirement)){
                this.Memory.AssignMemory(this.WaitingQueue.Peek().MemoryRequirement);
                this.ReadyQueue.EnQueue(this.WaitingQueue.DeQueue());
                this._os.Gui.UpdateReadyQueueLog();
                
                this._os.Gui.UpdateWaitingQueueLog();
                return true;
            }

            return false;               
        }

        public void InsertPCB(ProcessControlBlock pcb)
        {
            throw new NotImplementedException();
        }

        public ProcessControlBlock RemovePCB()
        {
            throw new NotImplementedException();
        }

        public State GetState()
        {
            throw new NotImplementedException();
        }

        public void SetState(State state)
        {
            throw new NotImplementedException();
        }

        public void GetWait()
        {
            throw new NotImplementedException();
        }

        public void SetWait()
        {
            throw new NotImplementedException();
        }

        public void GetArrival()
        {
            throw new NotImplementedException();
        }

        public void SetArrival()
        {
            throw new NotImplementedException();
        }

        public void GetCpuTime()
        {
            throw new NotImplementedException();
        }

        public void SetCpuTime()
        {
            throw new NotImplementedException();
        }
    
    }
}

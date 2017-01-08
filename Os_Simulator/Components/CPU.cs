
using System;
using Os_Simulator.Enums;

namespace Os_Simulator.Components
{
    /// <summary>
    /// Simulates a single cpu in an operating system
    /// </summary>
    public class CPU
    {
        private readonly OperatingSystem _os;

        /// <summary>
        /// Represents the Os's clock
        /// </summary>
        public Clock Clock = new Clock();

        /// <summary>
        /// Represents the pcb currently in cpu being executed
        /// </summary>
        public ProcessControlBlock CurrentPcb;

        /// <summary>
        /// Init the CPU
        /// </summary>
        /// <param name="os">All components must be given the parent OS so it has access to everything</param>
        public CPU(OperatingSystem os) {
            _os = os;
        }
    
        /// <summary>
        /// Advances the CPU 1 clock cycle. The cpu will execute the current process in the current pcb and 
        /// take whatever action is neccessary
        /// </summary>
        public void AdvanceClock()
        {
            //log stuff
            this._os.Gui.UpdateCurrentPCBTable(this.CurrentPcb != null ? this.CurrentPcb.ToString() : "");
            this._os.Gui.LogClockCycle(this.Clock.GetClock());

            //move clock and execute any operations
            this.Clock.Execute();
            this.ExecuteOperation();
        }

        /// <summary>
        /// Executes current operation in the current pcb
        /// </summary>
        public void ExecuteOperation()
        {         
            if (CurrentPcb == null) {
                return;
            }

            

            //if the program is new flag it as running since its in the cpu and about to be executed
            if(this.CurrentPcb.State == State.NEW)
                this.CurrentPcb.State = State.RUN;

            int remainingCycles = this.CurrentPcb.DecrementOperationClock();

            switch (this.CurrentPcb.CurrentOperation.Type) {
                case OperationType.CALCULATE:
                    //this.gui.PrintToLog("cpu executing calc");
                    //this.gui.PrintToLog(remainingCycles + " cycles remaing for calc");
                    if (this.CurrentPcb.CurrentOperation.Cycles == 0)
                        this.CurrentPcb.SetNextCommand();
           
                    break;
                case OperationType.IO:
                    if (this.CurrentPcb.State == State.RUN)
                    {
                        this.CurrentPcb.State = State.WAIT;
                        this.CurrentPcb.CurrentOperation.Cycles++; //add the io cycle back since this cycle was wasted moving to io q
                        this._os.IoScheduler.ScheduleIo(this.CurrentPcb.CurrentOperation.IoType, CurrentPcb);
                        this.CurrentPcb = null;
                        this._os.Gui.UpdateCurrentPCBTable("");
                    }
                    else
                    {
                        this.CurrentPcb.SetNextCommand();
                        //put the pcb back in a run state
                        this.CurrentPcb.State = State.RUN;
                    }
                                     
                    break;
                case OperationType.YIELD:
                    this.CurrentPcb.SetNextCommand();
                    //if the yield is the last command discard it and create a cont switch event                   
                    this._os.InterruptProcessor.CreateAndAddEvent(EventType.ContextSwitch);                 
                    break;
                case OperationType.OUT:
                    this._os.Gui.LogMessageToConsole(this.CurrentPcb.ToString());
                    this.CurrentPcb.SetNextCommand();
                    break;        
            }

            //if the process is exiting create a context switch event to notify the scheduler to switch it out
            if (CurrentPcb != null && CurrentPcb.State == State.EXIT)
            {
                //remove from system and free memory
                this._os.Scheduler.Memory.FreeMemory(CurrentPcb.MemoryRequirement);
                this._os.Scheduler.FinishedPcbs.Add(CurrentPcb); //your journey is over little buddy. Live long and prosper...
                this.CurrentPcb = null; //discard currentpcb
                
                this._os.InterruptProcessor.CreateAndAddEvent(EventType.ContextSwitch);
            }


        }

        /// <summary>
        /// Resets the cpu
        /// </summary>
        public void Reset()
        {
            this.Clock.Reset();
            this.CurrentPcb = null;
        }

        /// <summary>
        /// Manually set the clock to a new cycle
        /// </summary>
        /// <param name="cycle"></param>
        public void SetClock(int cycle) {
            this.Clock.SetClock(cycle);
        }

        /// <summary>
        /// Detect Interrupts
        /// </summary>
        public void DetectInterrupt()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Detect Preemption
        /// </summary>
        public void DetectPreemption()
        {
            throw new NotImplementedException();
        }
    
    }
}

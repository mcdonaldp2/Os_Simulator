using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Os_Simulator.Enums;

namespace Os_Simulator.Components
{
    /// <summary>
    /// Simulate a block of code that contains all properies of a process
    /// </summary>
    public class ProcessControlBlock
    {
        public int PId;
        public int MemoryRequirement;
        public State State;
        public List<Operation> ProcessOperations;
        private Operation _currentOperation;
        public int WaitTime = 0;
        public int TimeUsed = 0;
        public int IoRequestsPerformed = 0;

        /// <summary>
        /// Represents the pcb currently being executed
        /// </summary>
        public Operation CurrentOperation {
            get {
                if (this._currentOperation == null) {
                    this._currentOperation = this.ProcessOperations[0];
                    this.ProcessOperations.RemoveAt(0);
                }

                return this._currentOperation;
            }
            set {
                this._currentOperation = value;
            }
        }

        /// <summary>
        /// Rough guess of how many ccs the pcb will take
        /// </summary>
        public int TotalCyclesNeeded
        {
            get
            {
                var counter = 0;
                foreach (Operation operation in this.ProcessOperations)
                {
                    if (operation.Type == OperationType.CALCULATE || operation.Type == OperationType.IO)
                        counter += operation.Cycles;
                    else
                        counter++;
                }
                return counter;
            }
        }

        /// <summary>
        /// Create a new pcb
        /// </summary>
        /// <param name="state"></param>
        /// <param name="memoryRequirement"></param>
        /// <param name="processOperations"></param>
        public ProcessControlBlock(State state, int memoryRequirement, List<Operation> processOperations)
        {
            this.MemoryRequirement = memoryRequirement;
            this.State = state;
            this.ProcessOperations = processOperations;
        }

        /// <summary>
        /// Decrements the remaining clock cycles for the current operation
        /// </summary>
        /// <returns></returns>
        public int DecrementOperationClock() {
            TimeUsed++;

            _currentOperation.Cycles--;
            return this._currentOperation.Cycles;
        }

        /// <summary>
        /// Gets the next event in the operation list and assignes it as current operation
        /// </summary>
        /// <returns>True: successfully added, False: no operations left to add</returns>
        public bool SetNextCommand() {
            if (this._currentOperation.Type == OperationType.IO)
            {
                IoRequestsPerformed++;
            }          

            //if there are no more commands to get return false
            if (this.ProcessOperations.Count == 0) {
                this.State = State.EXIT;
                return false;
            }

            //get the next command and check to make sure it was assigned, 
            this._currentOperation = this.ProcessOperations[0];
            this.ProcessOperations.RemoveAt(0);

            return this._currentOperation != null;
        }

        /// <summary>
        /// Represents the pcb on the gui
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {       
            return String.Format("pId: {4}" + Environment.NewLine + 
                "State: {0}" + Environment.NewLine + 
                "Current Operation: {5}" + Environment.NewLine + 
                "Operation cycles left: {1}" + Environment.NewLine + 
                "CPU time used: {2}" + Environment.NewLine + 
                "IO Requests Performed: {3}", State, this.CurrentOperation.Cycles > 0 ? this.CurrentOperation.Cycles : 0, this.TimeUsed, IoRequestsPerformed, this.PId, this.CurrentOperation.Type);
        }

        /// <summary>
        /// Message for the proc command
        /// </summary>
        /// <returns></returns>
        public string ToStringForProc()
        {
            return String.Format("-------------" + Environment.NewLine + 
                                "pId: {4} | " +
                                 "State: {0} | " +
                                 "Current Operation: {5} | " +
                                 "Operation cycles left: {1} " + Environment.NewLine + 
                                 "CPU time used: {2} | " +
                                 "IO Requests Performed: {3}" +  Environment.NewLine + 
                                 "-------------", State, this.CurrentOperation.Cycles > 0 ? this.CurrentOperation.Cycles : 0, this.TimeUsed, IoRequestsPerformed, this.PId, this.CurrentOperation.Type);
        }
        
    }
}

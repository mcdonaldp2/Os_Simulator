using System.Collections.Generic;
using System.Linq;


namespace Os_Simulator.Components
{
    /// <summary>
    /// Simulates exection queue in the os. Used for ready and waiting queue
    /// </summary>
    public class ExecutionQueue
    {
        /// <summary>
        /// List data for the queue
        /// </summary>
        public List<ProcessControlBlock> Pcbs = new List<ProcessControlBlock>();

        /// <summary>
        /// If true, queue will change to priority queue based on total cycles
        /// </summary>
        public bool IsShortestJobFirst = false;
        
        /// <summary>
        /// The number of pcbs in the queue
        /// </summary>
        public int Size {
            get {
                return Pcbs.Count;
            }
            
            }

        /// <summary>
        /// Remove and return the pcb on the top of the queue
        /// </summary>
        /// <returns>Pcb next in line</returns>
        public ProcessControlBlock DeQueue()
        {
            ProcessControlBlock process = this.Pcbs[0];
            this.Pcbs.RemoveAt(0);

            if (IsShortestJobFirst)
                this.Pcbs = this.Pcbs.OrderBy(p => p.TotalCyclesNeeded).ToList();

            return process;
        }

        /// <summary>
        /// Add a pcb to the queue. The order will depend on which scheduling algorithm is selected
        /// </summary>
        /// <param name="pcb">Pcb to be inserted</param>
        public void EnQueue(ProcessControlBlock pcb)
        {
            this.Pcbs.Add(pcb);

            if (IsShortestJobFirst)
                this.Pcbs = this.Pcbs.OrderBy(p => p.TotalCyclesNeeded).ToList(); //OrderBy(o => o.OrderDate).ToList();
        }

        /// <summary>
        /// Return the queue as a list object
        /// </summary>
        /// <returns></returns>
        public List<ProcessControlBlock> GetCurrentQueueAsList()
        {
            return Pcbs;
        }

        /// <summary>
        /// Increments all wait times for pcbs in the system excluding the one in the cpu
        /// </summary>
        public void IncrementWaitTimes()
        {
            foreach (ProcessControlBlock pcb in Pcbs)
                pcb.WaitTime++;
        }

        /// <summary>
        /// Checks if the queue is empty
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return this.Pcbs.Count == 0;
        }

        /// <summary>
        /// Return the pcb on the top of the queue. Pcb will not be removed from queue
        /// </summary>
        /// <returns></returns>
        public ProcessControlBlock Peek()
        {
            if (this.Pcbs.Count <= 0)
                return null;

            return this.Pcbs[0];
        }

        /// <summary>
        /// Clears and resets this queue
        /// </summary>
        public void Reset()
        {
            this.Pcbs = new List<ProcessControlBlock>();
        }

        /// <summary>
        /// Decrements timer on every stack (type of io) in the io scheduler
        /// </summary>
        public void SimIoAll()
        {
            foreach (ProcessControlBlock p in this.Pcbs)
            {
                p.DecrementOperationClock();
                //p.ioRequestsPerformed++;
            }
        }

        /// <summary>
        /// Decrements timer for the last incoming io only
        /// </summary>
        public void SimIoTop()
        {
            if (this.Pcbs.Count > 0)
            {
                this.Pcbs[0].DecrementOperationClock();
                //this.pcbs[0].ioRequestsPerformed++;
            }
        }

        /// <summary>
        /// The memory of all pcbs in the queue
        /// </summary>
        public int TotalMemory
        {
            get
            {
                int total = 0;
                foreach (ProcessControlBlock p in Pcbs)
                {
                    total += p.MemoryRequirement;
                }

                return total;
            } 

            
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Os_Simulator.Enums;

namespace Os_Simulator.Components
{
    /// <summary>
    /// Simulates an Io scheduler in an os
    /// </summary>
    public class IoScheduler
    {
        private readonly OperatingSystem _os;
        private Dictionary<string, ExecutionQueue> _ioDictionary = new Dictionary<string, ExecutionQueue>();

        /// <summary>
        /// Inits the component with access to the parent os
        /// </summary>
        /// <param name="os">Parent os</param>
        public IoScheduler(OperatingSystem os)
        {
            this._os = os;
        }

        /// <summary>
        /// Get all pcbs in the scheduler
        /// </summary>
        /// <returns></returns>
        public List<ProcessControlBlock> GetAllPcBs()
        {
            List<ProcessControlBlock> pcbList = new List<ProcessControlBlock>();
            foreach (KeyValuePair<string, ExecutionQueue> item in _ioDictionary)
            {
                foreach (ProcessControlBlock pcb in item.Value.Pcbs)
                {
                    pcbList.Add(pcb);
                }
            }

            return pcbList;
        }

        /// <summary>
        /// Get all pcbs in the scheduler in a flat list
        /// </summary>
        /// <returns></returns>
        public List<ProcessControlBlock> GetList()
        {
            List<ProcessControlBlock> list = new List<ProcessControlBlock>();
            foreach (KeyValuePair<string, ExecutionQueue> item in _ioDictionary)
            {
                ExecutionQueue q = item.Value;

                foreach (ProcessControlBlock pcb in q.Pcbs)
                {
                    list.Add(pcb);
                }

            }
            return list;
        }

        /// <summary>
        /// Get an individual pcb from the scheduler by id
        /// </summary>
        /// <param name="id">Id of the pcb</param>
        /// <returns></returns>
        public ProcessControlBlock GetPcb(int id)
        {
            foreach (KeyValuePair<string, ExecutionQueue> item in _ioDictionary)
            {
                foreach (ProcessControlBlock pcb in item.Value.Pcbs)
                {
                    if (pcb.PId == id)
                    {
                        //copy the pcb and remove it from the queue
                        ProcessControlBlock p = pcb;
                        item.Value.Pcbs.Remove(pcb);

                        //remove the queue from the list if it is empty
                        if (item.Value.IsEmpty())
                            _ioDictionary.Remove(item.Key);

                        return p;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Checks if the scheduler has any pcbs waiting on io
        /// </summary>
        /// <returns></returns>
        public bool HasIo()
        {
            return this._ioDictionary.Count > 0;
        }

        /// <summary>
        /// Increment the wait times of all pcbs in the scheduler
        /// </summary>
        public void IncrementWaitTimes()
        {
            foreach (KeyValuePair<string, ExecutionQueue> item in _ioDictionary)
            {
                foreach (ProcessControlBlock pcb in item.Value.Pcbs)
                {
                    pcb.WaitTime++;
                }
            }
        }

        /// <summary>
        /// Checks if an individual pc is in the scheduler
        /// </summary>
        /// <param name="pcb"></param>
        /// <returns></returns>
        public bool IsInIoScheduler(ProcessControlBlock pcb)
        {
            foreach (KeyValuePair<string, ExecutionQueue> item in _ioDictionary)
            {
                foreach (ProcessControlBlock p in item.Value.Pcbs)
                {
                    if (p.PId == pcb.PId)
                        return true;

                }
            }

            return false;
        }

        /// <summary>
        /// Removes a pcb from the scheduler
        /// </summary>
        /// <param name="pcb"></param>
        /// <returns></returns>
        public ProcessControlBlock RemoveFromIoScheduler(ProcessControlBlock pcb)
        {
            ProcessControlBlock x;
            foreach (KeyValuePair<string, ExecutionQueue> item in _ioDictionary)
            {
                bool found = false;
                for (int i = 0; i < item.Value.Pcbs.Count; i++)
                {
                    if (item.Value.Pcbs[i].PId == pcb.PId)
                    {
                        x = item.Value.Pcbs[i];
                        item.Value.Pcbs.RemoveAt(i);
                        return x;
                    }
                }
            }

            throw new NullReferenceException();
        }

        /// <summary>
        /// Clear the io dictionary and reset the component
        /// </summary>
        public void Reset()
        {
            this._ioDictionary = new Dictionary<string, ExecutionQueue>();
        }

        /// <summary>
        /// Adds pcb to the appropriate device queue. If a queue doesnt exist for it create one.
        /// </summary>
        /// <param name="ioDevice"></param>
        /// <param name="pcb"></param>
        public void ScheduleIo(string ioDevice, ProcessControlBlock pcb)
        {
            //log the io request
            pcb.IoRequestsPerformed++;

            //add the io to its queue if it exists, else create a new queue and it
            if (_ioDictionary.ContainsKey(ioDevice))
                _ioDictionary[ioDevice].EnQueue(pcb);               
            else
            {
                ExecutionQueue newQ = new ExecutionQueue();
                newQ.EnQueue(pcb);
                _ioDictionary.Add(ioDevice, newQ);
            }

            this._os.Gui.UpdateIoQueueLog();
        }

        /// <summary>
        /// Gets the number of pcbs in the scheduler
        /// </summary>
        /// <returns></returns>
        public int Size()
        {
            int counter = 0;

            foreach (KeyValuePair<string, ExecutionQueue> item in _ioDictionary)
            {
                counter += item.Value.Size; 
            }

            return counter;
        }

        /// <summary>
        /// Increment io cycles of pcbs in scheduler
        /// </summary>
        public void StartIo()
        {
            if (this._ioDictionary.Count > 0)
            {
                //decrement the io queues based on setting
                foreach (KeyValuePair<string, ExecutionQueue> item in _ioDictionary)
                {
                    ExecutionQueue q = item.Value;
                    if (this._os.Settings.DecrementAllIo)
                        item.Value.SimIoAll();
                    else
                        item.Value.SimIoTop();
                }

                //loop through one more time to check if any of the ios are ready. if they are create an event so 
                // the cpu will catch it on the next cc
                foreach (KeyValuePair<string, ExecutionQueue> item in _ioDictionary)
                {
                    ExecutionQueue q = item.Value;

                    foreach (ProcessControlBlock p in q.Pcbs)
                    {
                        //create an event that the io is ready
                        if (p.CurrentOperation.Cycles == 0)
                        {
                            EventControlBlock ecb = new EventControlBlock();
                            ecb.Type = EventType.ReleaseIO;
                            ecb.PcbId = p.PId;
                            ecb.IoType = p.CurrentOperation.IoType;
                            //add the event to the list that the cpu will check
                            this._os.InterruptProcessor.AddEvent(ecb);
                        }
                    }
                }

            }

            this._os.Gui.UpdateIoQueueCycles();
        }

        /// <summary>
        /// Gets the total memory of all pcbs in the scheduler
        /// </summary>
        public int TotalMemory
        {
            get
            {
                int total = 0;
                foreach (KeyValuePair<string, ExecutionQueue> item in _ioDictionary)
                {
                    ExecutionQueue q = item.Value;
                    foreach (var p in q.Pcbs)
                    {
                        total += p.MemoryRequirement;
                    }
                }

                return total;
            }     
        }
   
    }
}

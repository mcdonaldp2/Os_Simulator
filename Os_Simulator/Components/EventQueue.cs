using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Os_Simulator.Components
{
    /// <summary>
    /// Queue used to handle events in the os
    /// </summary>
    public class EventQueue
    {
        /// <summary>
        /// The actual list the queue is built on. Due to the fact that this queues functionality must be changed when 
        /// scheduling algorithms are changed, this must be made public so scheduler can access it
        /// </summary>
        public List<EventControlBlock> ecbs = new List<EventControlBlock>();

        /// <summary>
        /// Number of Ecbs in the queue
        /// </summary>
        public int Size => ecbs.Count;
       
        /// <summary>
        /// Removes and returns the top object in the queue
        /// </summary>
        /// <returns>Top ecb in queue</returns>
        public EventControlBlock DeQueue()
        {
            EventControlBlock newEvent = this.ecbs[0];
            this.ecbs.RemoveAt(0);

            return newEvent;
        }

        /// <summary>
        /// Add an ecb to the queue
        /// </summary>
        /// <param name="ecb"></param>
        public void EnQueue(EventControlBlock ecb)
        {
            this.ecbs.Add(ecb);
        }

        /// <summary>
        /// Get the current queue as a List object
        /// </summary>
        /// <returns>List of ecbs in the queue</returns>
        public List<EventControlBlock> GetCurrentQueueAsList()
        {
            return ecbs;
        }

        /// <summary>
        /// Checks if the queue contains any items
        /// </summary>
        /// <returns>True if empty, false if not</returns>
        public bool IsEmpty()
        {
            return this.ecbs.Count == 0;
        }
   
        /// <summary>
        /// Get the top ecb on the queue without removing it
        /// </summary>
        /// <returns>The top ecb on the queue</returns>
        public EventControlBlock Peek()
        {
            if (this.ecbs.Count <= 0)
                return null;

            return this.ecbs[0];
        }

        /// <summary>
        /// Resets and clears the queue
        /// </summary>
        public void Reset()
        {
            this.ecbs = new List<EventControlBlock>();
        }
      
    }
}

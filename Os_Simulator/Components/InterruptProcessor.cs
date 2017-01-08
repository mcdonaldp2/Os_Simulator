using Os_Simulator.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Os_Simulator.Components
{
    public class InterruptProcessor
    {
        private OperatingSystem _os;
        private int _idCounter = 0;

        /// <summary>
        /// Queue to hold events
        /// </summary>
        public EventQueue EventQueue = new EventQueue();

        /// <summary>
        /// Init component with its parent os
        /// </summary>
        /// <param name="os">Parent os</param>
        public InterruptProcessor(OperatingSystem os)
        {
            this._os = os;
        }

        /// <summary>
        /// Add an event to the queue
        /// </summary>
        /// <param name="ecb">Ecb to add</param>
        public void AddEvent(EventControlBlock ecb)
        {
            this.EventQueue.EnQueue(ecb);
        }

        /// <summary>
        /// Creates a new event and adds it to the event queue
        /// </summary>
        /// <param name="eType">Type of event to create</param>
        public void CreateAndAddEvent(EventType eType)
        {
            EventControlBlock ecb = new EventControlBlock();
            ecb.Type = eType;
            ecb.Id = _idCounter;
            _idCounter++;
            //ecb.clockCycle = cc;

            this.AddEvent(ecb);
        }

        /// <summary>
        /// Clear out the queue
        /// </summary>
        public void EmptyQueue()
        {
            this.EventQueue.ecbs = new List<EventControlBlock>();
        }
       
        /// <summary>
        /// Get the next event on the queue
        /// </summary>
        /// <returns></returns>
        public EventControlBlock GetEvent()
        {
            return EventQueue.DeQueue();
        }

        /// <summary>
        /// Returns true if there are events in the queue
        /// </summary>
        public bool HasInterrupt => EventQueue.Size > 0;
  
        /// <summary>
        /// Clear the dictionary out and reset the component
        /// </summary>
        public void Reset()
        {
            this.EventQueue.Reset();
            this._idCounter = 0;
        }

        /// <summary>
        /// Checks if there are any events waiting to be executed
        /// </summary>
        /// <returns>True if not empty</returns>
        public bool SignalInterrupt()
        {
            return !this.EventQueue.IsEmpty();
        }

    }
}

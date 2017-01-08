using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Os_Simulator.Enums;

namespace Os_Simulator.Components
{
    /// <summary>
    /// Represents an event in the os
    /// </summary>
    public class EventControlBlock
    {
        /// <summary>
        /// The ecbs unique id
        /// </summary>
        public int Id;

        /// <summary>
        /// The type of event the ecb represents
        /// </summary>
        public EventType Type;

        /// <summary>
        /// Represents type of io for event. Only used when Type = IO
        /// </summary>
        public string IoType;

        /// <summary>
        /// The id of the pcb related to the event
        /// </summary>
        public int PcbId;

        /// <summary>
        /// The clock cycle that event should be fired on
        /// </summary>
        public int ClockCycle;

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Os_Simulator.Enums;

namespace Os_Simulator.Components
{
    /// <summary>
    /// Simulates an operation in a process
    /// </summary>
    public class Operation
    {
        /// <summary>
        /// Type of operation
        /// </summary>
        public OperationType Type;

        /// <summary>
        /// Cycles remaining on operation
        /// </summary>
        public int Cycles;

        /// <summary>
        /// Determines if the operation is an io operation
        /// </summary>
        public bool IsIo = false;

        /// <summary>
        /// If isIo is set to true an io type must be given
        /// </summary>
        public string IoType = "";

        /// <summary>
        /// Init an operation with its type
        /// </summary>
        /// <param name="type"></param>
        public Operation(OperationType type)
        {
            this.Type = type;
        }

        /// <summary>
        /// Init an operation with type and cycles
        /// </summary>
        /// <param name="type"></param>
        /// <param name="cycles"></param>
        public Operation(OperationType type, int cycles)
        {
            this.Type = type;
            this.Cycles = cycles;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Os_Simulator.Components
{
    /// <summary>
    /// Simulates memory management in an os
    /// </summary>
    public class MemoryManagement
    {
        private readonly OperatingSystem _os;
        private int _currentMemory = 0;

        /// <summary>
        /// Init the component with access to the os
        /// </summary>
        /// <param name="os">Parent os</param>
        public MemoryManagement(OperatingSystem os)
        {
            this._os = os;
        }

        /// <summary>
        /// Simulates assigning memory in the os. Should never be more than the max
        /// </summary>
        /// <param name="mem">Amount of memory to simulate</param>
        public void AssignMemory(int mem) {
            if (this._currentMemory + mem > this._os.Settings.MemorySize)
                throw new Exception("MemoryManangement: assignMemory(" + mem + "). Assigning would exceed memory");

            this._currentMemory += mem;

        }

        /// <summary>
        /// Checks if the amount of memory given could be added to the os
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool CanAllocate(int amount)
        {
            return this._currentMemory + amount <= this._os.Settings.MemorySize;
        }

        /// <summary>
        /// Simulates releasing memory in the os
        /// </summary>
        /// <param name="amount">Amount to release</param>
        public void FreeMemory(int amount) {
            this._currentMemory -= amount;
        }

        /// <summary>
        /// Get the amount of memory being used
        /// </summary>
        /// <returns></returns>
        public int GetCurrentMemoryUsage()
        {
            return this._currentMemory;
        }

        /// <summary>
        /// Resets the memory to 0
        /// </summary>
        public void Reset()
        {
            this._currentMemory = 0;
        }

    }
}

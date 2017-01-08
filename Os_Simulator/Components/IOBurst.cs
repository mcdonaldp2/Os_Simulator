using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Os_Simulator.Components
{
    /// <summary>
    /// Simulates a random io burst for io operations
    /// </summary>
    public class IoBurst
    {
        /// <summary>
        /// Returns random number to simulate io burst
        /// </summary>
        /// <returns></returns>
        public int GenerateIoBurst()
        {
            var r = new Random();
            return r.Next(1, 20);
        }
    }
}

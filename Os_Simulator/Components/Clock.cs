
namespace Os_Simulator.Components
{
    /// <summary>
    /// Simulates the clock in an operating system
    /// </summary>
    public class Clock
    {
        private int _counter;

        /// <summary>
        /// Init the clock starting at 0
        /// </summary>
        public Clock()
        {
            _counter = 0;
        }

        /// <summary>
        /// Advances clock cycle by 1
        /// </summary>
        public void Execute()
        {
            this._counter++;
        }

        /// <summary>
        /// Gets the current clock cycle
        /// </summary>
        /// <returns>Current clock cycl</returns>
        public int GetClock()
        {
            return _counter;
        }

        /// <summary>
        /// Resets the Clock to 0
        /// </summary>
        public void Reset()
        {
            this._counter = 0;
        }

        /// <summary>
        /// Sets the clock to a particular clock cycle
        /// </summary>
        /// <param name="num"></param>
        public void SetClock(int num) {
            this._counter = num;
        }

    }
}

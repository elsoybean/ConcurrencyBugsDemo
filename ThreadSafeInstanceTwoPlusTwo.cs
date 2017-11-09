using System.Threading;

namespace ConcurrencyBugsDemo
{
    /// <summary>
    /// A thread-safe implementation. Note that the name makes it obvious that thread safety must be maintained.
    /// </summary>
    /// <seealso cref="ConcurrencyBugsDemo.ITwoPlusTwo" />
    public class ThreadSafeInstanceTwoPlusTwo : ITwoPlusTwo
    {
        private int _answer = 0;
        private readonly object _lock = new object();

        public int CalculateTwoPlusTwo()
        {
            Thread.Sleep(6);
            lock (_lock)
            {
                _answer = _answer + 2;
                _answer = _answer + 2;

                var result = _answer;
                _answer = 0;
                return result;
            }
        }
    }
}

using System.Threading;

namespace ConcurrencyBugsDemo
{
    /// <summary>
    /// This implementation is NOT thread safe, due to its use of the static field.
    /// Avoid these when possible, and clearly mark your class as unsafe when you must use them and cannot solve for thread safety some other way.
    /// </summary>
    /// <seealso cref="ConcurrencyBugsDemo.ITwoPlusTwo" />
    public class StaticTwoPlusTwo : ITwoPlusTwo
    {
        private static int _answer = 0;

        public int CalculateTwoPlusTwo()
        {
            Thread.Sleep(6);
            _answer = _answer + 2;
            _answer = _answer + 2;
            
            var result = _answer;
            _answer = 0;
            
            return result;
        }
    }
}

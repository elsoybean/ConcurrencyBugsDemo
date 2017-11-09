using System.Threading;

namespace ConcurrencyBugsDemo
{
    /// <summary>
    /// A thread safe implementation, as it has no instance properties or fields.
    /// </summary>
    /// <seealso cref="ConcurrencyBugsDemo.ITwoPlusTwo" />
    public class LocalTwoPlusTwo : ITwoPlusTwo
    {
        public int CalculateTwoPlusTwo()
        {            
            var answer = 0;
            Thread.Sleep(6);
            answer = answer + 2;
            answer = answer + 2;
            
            var result = answer;
            
            return result;
        }
    }
}
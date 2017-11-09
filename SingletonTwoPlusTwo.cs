using System.Threading;

namespace ConcurrencyBugsDemo
{
    /// <summary>
    /// This implementation is NOT thread safe, due to its use of an instance field and the signleton pattern.
    /// These are harder to avoid, of course, but you should always think about ways to make class implementations stateless, when possible.
    /// </summary>
    /// <seealso cref="ConcurrencyBugsDemo.ITwoPlusTwo" />
    public class SingletonTwoPlusTwo : ITwoPlusTwo
    {
        private SingletonTwoPlusTwo() {}

        private static SingletonTwoPlusTwo _instance;

        public static SingletonTwoPlusTwo Instance
        {
            get { return _instance ?? (_instance = new SingletonTwoPlusTwo()); }
        }

        private int _answer = 0;

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

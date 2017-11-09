using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ConcurrencyBugsDemo
{
    public class TwoPlusTwoTests
    {
        private const int Iterations = 5000;
        private const int RightAnswer = 4;
        private IEnumerable<int> _expected;

        [SetUp]
        public void Setup()
        {
            var results = new List<int>();
            for (var i = 0; i < Iterations; i++) results.Add(RightAnswer);
            _expected = results;
        }

        [Test]
        public void A_SanityCheck()
        {
            ITwoPlusTwo implementation = new StaticTwoPlusTwo();
            var result = implementation.CalculateTwoPlusTwo();
            Assert.AreEqual(RightAnswer, result);

            implementation = new InstanceTwoPlusTwo();
            result = implementation.CalculateTwoPlusTwo();
            Assert.AreEqual(RightAnswer, result);

            implementation = new ThreadSafeInstanceTwoPlusTwo();
            result = implementation.CalculateTwoPlusTwo();
            Assert.AreEqual(RightAnswer, result);

            implementation = new LocalTwoPlusTwo();
            result = implementation.CalculateTwoPlusTwo();
            Assert.AreEqual(RightAnswer, result);
        }

        [Test]
        public void B_Unsafe_StaticImplementation()
        {
            var results = new ConcurrentBag<int>();
            var opt = new ParallelOptions { MaxDegreeOfParallelism = Iterations };

            Parallel.For(0, Iterations, opt, i =>
            {
                ITwoPlusTwo implementation = new StaticTwoPlusTwo();
                var result = implementation.CalculateTwoPlusTwo();
                results.Add(result);
            });

            Assert.IsEmpty(results.Where(r => r != RightAnswer));
        }

        [Test]
        public void C_Unsafe_InstanceImplementation()
        {
            var results = new ConcurrentBag<int>();
            var opt = new ParallelOptions { MaxDegreeOfParallelism = Iterations };

            ITwoPlusTwo implementation = new InstanceTwoPlusTwo();

            Parallel.For(0, Iterations, opt, i =>
            {
                var result = implementation.CalculateTwoPlusTwo();
                results.Add(result);
            });

            Assert.IsEmpty(results.Where(r => r != RightAnswer));
        }

        [Test]
        public void D_Safe_StaticImplementation_SingleThreaded()
        {           
            // Here we guarantee thread safety by eliminating parallelism, but this has a big performance impact
            var results = new ConcurrentBag<int>();

            for (var i = 0; i < Iterations; i++)
            {
                ITwoPlusTwo implementation = new StaticTwoPlusTwo();
                var result = implementation.CalculateTwoPlusTwo();
                results.Add(result);
            };

            Assert.IsEmpty(results.Where(r => r != RightAnswer));
        }

        [Test]
        public void E_Safe_InstanceImplementation_ThreadLocalInstances()
        {
            // Here we guarantee thread safety by giving each thread its own instance, but this is not always possible or desirable
            var results = new ConcurrentBag<int>();
            var opt = new ParallelOptions { MaxDegreeOfParallelism = Iterations };

            Parallel.For(0, Iterations, opt, i =>
            {
                // The following line was moved inside the parallel loop so that each thread has its own instance
                ITwoPlusTwo implementation = new InstanceTwoPlusTwo();
                var result = implementation.CalculateTwoPlusTwo();
                results.Add(result);
            });

            Assert.IsEmpty(results.Where(r => r != RightAnswer));
        }

        [Test]
        public void F_Safe_InstanceImplementation_WithLocks()
        {
            // Here we guarantee thread safety by using a thread safe implementation.
            // Care must be taken when modifying the implementation that we maintain the thread safety
            var results = new ConcurrentBag<int>();
            var opt = new ParallelOptions { MaxDegreeOfParallelism = Iterations };

            ITwoPlusTwo implementation = new ThreadSafeInstanceTwoPlusTwo();

            Parallel.For(0, Iterations, opt, i =>
            {
                var result = implementation.CalculateTwoPlusTwo();
                results.Add(result);
            });

            Assert.IsEmpty(results.Where(r => r != RightAnswer));
        }

        [Test]
        public void G_Safe_LocalImplementation_WithLocks()
        {
            // Here we guarantee thread safety by not using any instance properties in the implementation.
            // This is not always possible, and care must still be taken not to add them in the future.
            var results = new ConcurrentBag<int>();
            var opt = new ParallelOptions { MaxDegreeOfParallelism = Iterations };

            ITwoPlusTwo implementation = new LocalTwoPlusTwo();

            Parallel.For(0, Iterations, opt, i =>
            {
                var result = implementation.CalculateTwoPlusTwo();
                results.Add(result);
            });

            Assert.IsEmpty(results.Where(r => r != RightAnswer));
        }
    }
}

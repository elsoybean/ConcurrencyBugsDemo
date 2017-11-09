# ConcurrencyBugsDemo
This is a simple repository showing some contrived examples to illustrate some common concurrency / multi-threading bugs.

## Introduction
To get started, open up the solution and take a look at the ```TwoPlusTwoTests``` unit test file.

We have several different implementations of ```ITwoPlusTwo``` that all add two and two together to get four.
They all seem to work just fine, but when we try to run some of them in a parallel, we start to see some intermittent errors.

## SanityCheck
This test just proves that each of our implementations successfully adds two and two when run a single time in a single thread.

## Failing Tests
When you run these tests, you will see that a handful of the 5000 executions returned something other than 4, and so the test fails.
Due to the nature of concurrency bugs, it will be a different number of failures each time, and they may even pass ocassionally.
Furthermore, your system configuration might affect when and how often they fail. These are all examples of confounding factors that
hide concurrency bugs and make them much harder to troubleshoot.

### Unsafe Static Implementation
This is not thread safe because the ```StaticTwoPlusTwo``` implementation uses a static field as part of its calculation.
All instances of a class share any static fields across all threads, so as different threads modify this field,
other threads are also affected.
- We should avoid static fields whenever possible.
- In a web service, this is usually the most common problem, as most instances are contained to the request thread.

### Unsafe Singleton Implementation
This is not thread safe because the ```SingletonTwoPlusTwo``` implementation uses the singleton pattern and an instance field
as part of its calculation. The pattern guarantees that there is only one instance of the class, so as different threads modify
the field, other threads are also affected.
- We should avoid keeping state in a singleton whenever possible.
- Not all implementations of the singleton pattern use the static keyword. The dependency injection contanier, for example,
might enforce a singleton lifecycle.

### Unsafe Instance Implementation
This is not thread safe because all threads in the parallel process reference the same instance of the class, and it uses an
instance field as part of its calculation. As with the previous examples, as different threads modify this field, other threads
are also affected.
- When possible, we should avoid (or at least think carefully) about saving state in a class.
- Where it is appropriate or unavoidable to save state, we need to take care that access to that state is thread-safe if there
is any chance that two threads might hold a reference to the same instance.
- Keep in mind you do not have complete control over how your class is used.

## Passing Tests
These tests have all had their concurrency bugs addressed in a variety of ways and should always pass.

### Single Threaded
This illustrates the simplest, but often least desirable, way to fix a multi-threading bug: simply force it to execute in a
single thread. There is a small delay built into the calculation for each implementation, though, and that delay will become
very apparent when the 5000 calculations are done in a single thread. Nevertheless, forcing a single thread can be a good
technique to prove you do in fact have a concurrency bug. However, care needs to be taken that you do not fix the bug by
inadvertantly forcing all of the code to run in a single thread.

### Thread Local Instances
This guarantees thread safety by giving each thread its own instance of the implementing class. The class still maintains state,
but since the instance is created within the parallel process, each thread will have an independent instance and cannot interfere
with another thread's instance. Note that this will not work with a static or singleton instance.

In a web application, this is often the (somewhat hidden) default solution. Since each request runs in its own thread, unless you
are specifically taking steps to share instances across threads, each thread has its own set of instances and is insulated from
concurrency bugs in that manner.

### Locks
This guarantees thread safety by protecting access to the instance field with a ```lock (obj) { }``` block. This allows to to
selectively force single threading for a few specific lines of code. If two threads share an instance, the second thread will have
to wait to enter the locked block until the first thread has exited it. In this case, we kept the expensive ```Thread.Sleep()``` call
outside of the lock, but all access to the instance field is done within the lock, ensuring that threads cannot interfere with each
other.

Again, care must be taken that the lock is as small as possible, otherwise you are inadvertantly forcing single-threading with the
associated performance impacts.

Additionally, we need to make sure that future developers can tell that this class needs to maintain that thread safety. If you're a
good and decent person, you'll name your class accordingly.

### Local Implementation
In this final example we are guaranteeing thread safety in the best way possible: by eliminating state from our class implementation.
This is not always possible, but it's worth thinking about whether a class really needs state at design time or when troubleshooting
concurrency bugs. Rather than use an instance field, this implementation moves the ```answer``` into the method scope, which also
prevetns multiple threads from interfering with each other.

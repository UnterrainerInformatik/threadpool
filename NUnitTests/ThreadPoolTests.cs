// *************************************************************************** 
// This is free and unencumbered software released into the public domain.
// 
// Anyone is free to copy, modify, publish, use, compile, sell, or
// distribute this software, either in source code form or as a compiled
// binary, for any purpose, commercial or non-commercial, and by any
// means.
// 
// In jurisdictions that recognize copyright laws, the author or authors
// of this software dedicate any and all copyright interest in the
// software to the public domain. We make this dedication for the benefit
// of the public at large and to the detriment of our heirs and
// successors. We intend this dedication to be an overt act of
// relinquishment in perpetuity of all present and future rights to this
// software under copyright law.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// 
// For more information, please refer to <http://unlicense.org>
// ***************************************************************************

using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using SplitStopWatch;

namespace ThreadPooling.NUnitTests
{
    /// <summary>
    ///     Tests for EnqueueWorkItem.
    /// </summary>
    [TestFixture]
    [Category("Threading.ThreadPool")]
    public class Threading
    {
        private ThreadPool threadPool;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        [SetUp]
        public void Init()
        {
        }

        /// <summary>
        ///     Finishes this instance.
        /// </summary>
        [TearDown]
        public void Finish()
        {
        }

        /// <summary>
        ///     Tests the thread pool.
        /// </summary>
        [Test]
        [Category("Threading.ThreadPool")]
        public void TestThreadPool()
        {
            threadPool = new ThreadPool(Environment.ProcessorCount, "testWorker");

            var ssw = new SplitStopwatch();
            ssw.Start("starting to enqueue...");

            var wir1 = threadPool.EnqueueWorkItem(CalcAverage, new[] {2, 3, 2, 5});
            var wir2 = threadPool.EnqueueWorkItem(CalcAverage, new[] {1, 1, 1, 1});
            var wir3 = threadPool.EnqueueWorkItem(CalcAverage, new[] {2, 3, 2, 5});
            var wir4 = threadPool.EnqueueWorkItem(CalcAverage, new[] {2, 3, 2, 5});
            var wir5 = threadPool.EnqueueWorkItem(CalcAverage, new[] {2, 3, 2, 5});
            var wir6 = threadPool.EnqueueWorkItem(CalcAverage, new[] {2, 3, 2, 5});
            var wir7 = threadPool.EnqueueWorkItem(CalcAverage, new[] {2, 3, 2, 5});
            var wir8 = threadPool.EnqueueWorkItem(CalcAverage, new[] {2, 3, 2, 5});
            var wir9 = threadPool.EnqueueWorkItem(CalcAverage, new[] {2, 3, 2, 5});
            var wir10 = threadPool.EnqueueWorkItem(CalcAverage, new[] {2, 3, 2, 5});

            ssw.Split("all items are enqueued...");

            var average = wir1.Result;
            Assert.AreEqual(average, 3.0);
            ssw.Split("we waited for result 1...");
            wir1.Dispose();

            average = wir2.Result;
            Assert.AreEqual(average, 1.0);
            ssw.Split("we waited for result 2...");
            wir2.Dispose();

            average = wir3.Result;
            Assert.AreEqual(average, 3.0);
            ssw.Split("we waited for result 3...");
            wir3.Dispose();

            average = wir4.Result;
            Assert.AreEqual(average, 3.0);
            ssw.Split("we waited for result 4...");
            wir4.Dispose();

            average = wir5.Result;
            Assert.AreEqual(average, 3.0);
            ssw.Split("we waited for result 5...");
            wir5.Dispose();

            average = wir6.Result;
            Assert.AreEqual(average, 3.0);
            ssw.Split("we waited for result 6...");
            wir6.Dispose();

            average = wir7.Result;
            Assert.AreEqual(average, 3.0);
            ssw.Split("we waited for result 7...");
            wir7.Dispose();

            average = wir8.Result;
            Assert.AreEqual(average, 3.0);
            ssw.Split("we waited for result 8...");
            wir8.Dispose();

            average = wir9.Result;
            Assert.AreEqual(average, 3.0);
            ssw.Split("we waited for result 9...");
            wir9.Dispose();

            average = wir10.Result;
            Assert.AreEqual(average, 3.0);
            ssw.Split("we waited for result 10...");
            wir10.Dispose();

            var wir11 = threadPool.EnqueueWorkItem(CalcAverage, new[] {2, 3, 2, 5});
            average = wir11.Result;
            Assert.AreEqual(average, 3.0);
            ssw.Split("we waited for result 11...");
            wir11.Dispose();

            threadPool.ShutDown();
        }

        /// <summary>
        ///     Tests the thread pool.
        /// </summary>
        [Test]
        [Category("Threading.ThreadPool")]
        public void TestThreadPoolManyThreadsFuncT()
        {
            threadPool = new ThreadPool(Environment.ProcessorCount, "testWorker");

            var wirs = new List<IWorkItemState<bool>>();

            for (var i = 0; i < 50; i++)
            {
                var wir = threadPool.EnqueueWorkItem(Not, true);
                wirs.Add(wir);
            }

            foreach (var workItemState in wirs)
            {
                var result = workItemState.Result;
                workItemState.Dispose();

                Assert.AreEqual(result, false);
            }

            threadPool.ShutDown();
        }

        /// <summary>
        ///     Tests the thread pool.
        /// </summary>
        [Test]
        [Category("Threading.ThreadPool")]
        public void TestThreadPoolManyWorkItemsSingleThread()
        {
            threadPool = new ThreadPool(1, "testWorker");

            var wirs = new List<IWorkItemState<bool>>();

            for (var i = 0; i < 50; i++)
            {
                var wir = threadPool.EnqueueWorkItem(Not, i%2 == 1);
                wirs.Add(wir);
            }

            var anticipatedResult = true;
            foreach (var workItemState in wirs)
            {
                var result = workItemState.Result;
                workItemState.Dispose();

                Assert.AreEqual(result, anticipatedResult);
                anticipatedResult = !anticipatedResult;
            }

            threadPool.ShutDown();
        }

        /// <summary>
        ///     Tests the thread pool.
        /// </summary>
        [Test]
        [Ignore]
        [Category("Threading.ThreadPool")]
        public void TestThreadPoolManyThreadsActionT()
        {
            threadPool = new ThreadPool(Environment.ProcessorCount, "testWorker");

            var wirs = new List<IWorkItemState>();

            for (var i = 0; i < 50; i++)
            {
                var wir = threadPool.EnqueueWorkItem(WriteToConsole, "test");
                wirs.Add(wir);
            }

            foreach (var workItemState in wirs)
            {
                workItemState.Result();
                workItemState.Dispose();
            }

            threadPool.ShutDown();
        }

        /// <summary>
        ///     Tests the thread pool.
        ///     Tests, if all the enqueued items are only calculated once.
        /// </summary>
        [Test]
        [Ignore]
        [Category("Threading.ThreadPool")]
        public void TestThreadPoolManyThreadsActionTNumberOfCalls()
        {
            threadPool = new ThreadPool(Environment.ProcessorCount, "testWorker");

            var wirs = new List<IWorkItemState>();
            var objects = new List<ThreadingThreadpoolTestObject>();

            for (var j = 0; j < 5000; j++)
            {
                wirs.Clear();
                objects.Clear();
                for (var i = 0; i < 100; i++)
                {
                    var o = new ThreadingThreadpoolTestObject();
                    objects.Add(o);
                    var wir = threadPool.EnqueueWorkItem(o.Call);
                    wirs.Add(wir);
                }

                foreach (var workItemState in wirs)
                {
                    workItemState.Result();
                    workItemState.Dispose();
                }

                foreach (var threadingThreadpoolTestObject in objects)
                {
                    Assert.AreEqual(1, threadingThreadpoolTestObject.NumberOfCalls,
                        "One of the objects has been called " + threadingThreadpoolTestObject.NumberOfCalls +
                        " times. It was expected to be called once only and exactly once.");
                }
            }

            threadPool.ShutDown();
        }

        /// <summary>
        ///     Tests the thread pool.
        /// </summary>
        [Test]
        [Ignore]
        [Category("Threading.ThreadPool")]
        public void TestThreadPoolManyThreadsActionTWithCallback()
        {
            threadPool = new ThreadPool(Environment.ProcessorCount, "testWorker");

            var wirs = new List<IWorkItemState>();

            for (var i = 0; i < 50; i++)
            {
                var wir = threadPool.EnqueueWorkItem(WriteToConsole, "test", CallbackFunction);
                wirs.Add(wir);
            }

            foreach (var workItemState in wirs)
            {
                workItemState.Result();
                workItemState.Dispose();
            }

            threadPool.ShutDown();
        }

        /// <summary>
        ///     Tests the thread pool.
        /// </summary>
        [Test]
        [Ignore]
        [Category("Threading.ThreadPool")]
        public void TestThreadPoolPausedAndResumed()
        {
            threadPool = new ThreadPool(Environment.ProcessorCount, "testWorker");

            var wirs = new List<IWorkItemState>();

            for (var i = 0; i < 10; i++)
            {
                if (i == 5)
                {
                    threadPool.Sleep();
                    Thread.Sleep(2000);
                }
                var wir = threadPool.EnqueueWorkItem(WriteToConsole, "test " + (i + 1));
                wirs.Add(wir);
            }

            threadPool.Wakeup();

            foreach (var workItemState in wirs)
            {
                workItemState.Result();
                workItemState.Dispose();
            }

            threadPool.ShutDown();
        }

        /// <summary>
        ///     Tests the thread pool's performance.
        ///     Tests without threadpool and with threadpools employing a workerthread-count of one to Environment.ProcessorCount *
        ///     2.
        /// </summary>
        [Test]
        [Category("Threading.ThreadPool.Performance")]
        public void TestThreadPoolPerformance()
        {
            const int numberOfOperations = 100000;
            var ssw = new SplitStopwatch();

            // Test without any threadPool.
            ssw.Start("SINGLE THREADED WITHOUT POOL:");
            for (var i = 0; i < numberOfOperations; i++)
            {
                CalcAverage(GetNumbersForAverage());
            }
            ssw.Stop("Done.", 1);
            Console.Out.WriteLine(string.Empty);
            ssw.Reset();

            for (var numberOfWorkerThreads = 1;
                numberOfWorkerThreads < Environment.ProcessorCount*2;
                numberOfWorkerThreads++)
            {
                ssw.Start("THREADPOOL (" + numberOfWorkerThreads + " workerThreads):");
                threadPool = new ThreadPool(numberOfWorkerThreads, "testWorker");
                ssw.Split("Starting to enqueue.", 1);

                for (var i = 0; i < numberOfOperations; i++)
                {
                    threadPool.EnqueueWorkItem(CalcAverage, GetNumbersForAverage());
                }

                ssw.Split("All items are enqueued.", 1);
                threadPool.WaitForEveryWorkerIdle();
                threadPool.ShutDown();

                ssw.Stop("Done.", 1);
                Console.Out.WriteLine(string.Empty);
                ssw.Reset();
            }
        }

        private static int[] GetNumbersForAverage()
        {
            return new[]
            {
                8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2,
                3, 4, 0,
                2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2,
                12, 7, 8, 2,
                3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4,
                0, 2, 12,
                7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8,
                2, 3, 4,
                0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2,
                12, 7, 8,
                2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3,
                4, 0, 2,
                12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12,
                7, 8, 2, 3,
                4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0,
                2, 12, 7,
                8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2,
                3, 4, 0,
                2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2,
                12, 7, 8, 2,
                3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4,
                0, 2, 12,
                7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8,
                2, 3, 4,
                0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2,
                12, 7, 8,
                2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3,
                4, 0, 2,
                12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12,
                7, 8, 2, 3,
                4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0,
                2, 12, 7,
                8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2,
                3, 4, 0,
                2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2,
                12, 7, 8, 2,
                3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4,
                0, 2, 12,
                7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8,
                2, 3, 4,
                0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2,
                12, 7, 8,
                2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3,
                4, 0, 2,
                12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12,
                7, 8, 2, 3,
                4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0,
                2, 12, 7,
                8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2,
                3, 4, 0,
                2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2,
                12, 7, 8, 2,
                3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4,
                0, 2, 12,
                7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8,
                2, 3, 4,
                0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2,
                12, 7, 8,
                2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3,
                4, 0, 2,
                12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12,
                7, 8, 2, 3,
                4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0,
                2, 12, 7,
                8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2,
                3, 4, 0,
                2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2,
                12, 7, 8, 2,
                3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4,
                0, 2, 12,
                7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8,
                2, 3, 4,
                0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2,
                12, 7, 8,
                2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3,
                4, 0, 2,
                12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12,
                7, 8, 2, 3,
                4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0,
                2, 12, 7,
                8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2,
                3, 4, 0,
                2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7
            };
        }

        /// <summary>
        ///     A callback function.
        /// </summary>
        private static void CallbackFunction()
        {
            Console.Out.WriteLine("callback reached.");
        }

        /// <summary>
        ///     Calculates the average of given numbers.
        /// </summary>
        /// <param name="numbers">The numbers.</param>
        /// <returns></returns>
        private static double CalcAverage(int[] numbers)
        {
            var average = 0.0;

            if (numbers == null || numbers.Length == 0)
            {
                return 0.0;
            }

            foreach (var number in numbers)
            {
                average += number;
            }
            average /= numbers.Length;

            //Utils.ConsumeCpu(1, TimeSpan.FromSeconds(.01));

            return average;
        }

        /// <summary>
        ///     Writes to the console.
        /// </summary>
        /// <param name="s">The string.</param>
        private static void WriteToConsole(string s)
        {
            Console.Out.WriteLine(s);
        }

        /// <summary>
        ///     Performs a not-operation.
        /// </summary>
        /// <param name="flag">
        ///     if set to <c>true</c> [flag].
        /// </param>
        /// <returns></returns>
        private static bool Not(bool flag)
        {
            return !flag;
        }
    }
}
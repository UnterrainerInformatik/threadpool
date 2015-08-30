// *************************************************************************** 
//  Copyright (c) 2013 by Unterrainer Informatik OG.
//  This source is licensed to Unterrainer Informatik OG.
//  All rights reserved.
//  
//  In other words:
//  YOU MUST NOT COPY, USE, CHANGE OR REDISTRIBUTE ANY ART, MUSIC, CODE OR
//  OTHER DATA, CONTAINED WITHIN THESE DIRECTORIES WITHOUT THE EXPRESS
//  PERMISSION OF Unterrainer Informatik OG.
// 
//  Classes using other, less restrictive licenses are explicitly marked.
// ---------------------------------------------------------------------------
//  Programmer: G U, 
//  Created: 2013-03-31
// ***************************************************************************

using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using SplitStopWatch;
using ThreadPooling.ThreadPooling;

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

			SplitStopwatch ssw = new SplitStopwatch();
			ssw.Start("starting to enqueue...");

			IWorkItemState<double> wir1 = threadPool.EnqueueWorkItem(new Func<int[], double>(CalcAverage), new[] {2, 3, 2, 5});
			IWorkItemState<double> wir2 = threadPool.EnqueueWorkItem(new Func<int[], double>(CalcAverage), new[] {1, 1, 1, 1});
			IWorkItemState<double> wir3 = threadPool.EnqueueWorkItem(new Func<int[], double>(CalcAverage), new[] {2, 3, 2, 5});
			IWorkItemState<double> wir4 = threadPool.EnqueueWorkItem(new Func<int[], double>(CalcAverage), new[] {2, 3, 2, 5});
			IWorkItemState<double> wir5 = threadPool.EnqueueWorkItem(new Func<int[], double>(CalcAverage), new[] {2, 3, 2, 5});
			IWorkItemState<double> wir6 = threadPool.EnqueueWorkItem(new Func<int[], double>(CalcAverage), new[] {2, 3, 2, 5});
			IWorkItemState<double> wir7 = threadPool.EnqueueWorkItem(new Func<int[], double>(CalcAverage), new[] {2, 3, 2, 5});
			IWorkItemState<double> wir8 = threadPool.EnqueueWorkItem(new Func<int[], double>(CalcAverage), new[] {2, 3, 2, 5});
			IWorkItemState<double> wir9 = threadPool.EnqueueWorkItem(new Func<int[], double>(CalcAverage), new[] {2, 3, 2, 5});
			IWorkItemState<double> wir10 = threadPool.EnqueueWorkItem(new Func<int[], double>(CalcAverage), new[] {2, 3, 2, 5});

			ssw.Split("all items are enqueued...");

			double average = wir1.Result;
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

			IWorkItemState<double> wir11 = threadPool.EnqueueWorkItem(new Func<int[], double>(CalcAverage), new[] {2, 3, 2, 5});
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

			List<IWorkItemState<bool>> wirs = new List<IWorkItemState<bool>>();

			for (int i = 0; i < 50; i++)
			{
				IWorkItemState<bool> wir = threadPool.EnqueueWorkItem(new Func<bool, bool>(Not), true);
				wirs.Add(wir);
			}

			foreach (IWorkItemState<bool> workItemState in wirs)
			{
				bool result = workItemState.Result;
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

			List<IWorkItemState<bool>> wirs = new List<IWorkItemState<bool>>();

			for (int i = 0; i < 50; i++)
			{
				IWorkItemState<bool> wir = threadPool.EnqueueWorkItem(new Func<bool, bool>(Not), i%2 == 1);
				wirs.Add(wir);
			}

			bool anticipatedResult = true;
			foreach (IWorkItemState<bool> workItemState in wirs)
			{
				bool result = workItemState.Result;
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

			List<IWorkItemState> wirs = new List<IWorkItemState>();

			for (int i = 0; i < 50; i++)
			{
				IWorkItemState wir = threadPool.EnqueueWorkItem(WriteToConsole, "test");
				wirs.Add(wir);
			}

			foreach (IWorkItemState workItemState in wirs)
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

			List<IWorkItemState> wirs = new List<IWorkItemState>();
			List<ThreadingThreadpoolTestObject> objects = new List<ThreadingThreadpoolTestObject>();

			for (int j = 0; j < 5000; j++)
			{
				wirs.Clear();
				objects.Clear();
				for (int i = 0; i < 100; i++)
				{
					ThreadingThreadpoolTestObject o = new ThreadingThreadpoolTestObject();
					objects.Add(o);
					IWorkItemState wir = threadPool.EnqueueWorkItem(o.Call);
					wirs.Add(wir);
				}

				foreach (IWorkItemState workItemState in wirs)
				{
					workItemState.Result();
					workItemState.Dispose();
				}

				foreach (ThreadingThreadpoolTestObject threadingThreadpoolTestObject in objects)
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

			List<IWorkItemState> wirs = new List<IWorkItemState>();

			for (int i = 0; i < 50; i++)
			{
				IWorkItemState wir = threadPool.EnqueueWorkItem(WriteToConsole, "test", CallbackFunction);
				wirs.Add(wir);
			}

			foreach (IWorkItemState workItemState in wirs)
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

			List<IWorkItemState> wirs = new List<IWorkItemState>();

			for (int i = 0; i < 10; i++)
			{
				if (i == 5)
				{
					threadPool.Sleep();
					Thread.Sleep(2000);
				}
				IWorkItemState wir = threadPool.EnqueueWorkItem(WriteToConsole, "test " + (i + 1));
				wirs.Add(wir);
			}

			threadPool.Wakeup();

			foreach (IWorkItemState workItemState in wirs)
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
			SplitStopwatch ssw = new SplitStopwatch();

			// Test without any threadPool.
			ssw.Start("SINGLE THREADED WITHOUT POOL:");
			for (int i = 0; i < numberOfOperations; i++)
			{
				CalcAverage(GetNumbersForAverage());
			}
			ssw.Stop("Done.", 1);
			Console.Out.WriteLine(string.Empty);
			ssw.Reset();

			for (int numberOfWorkerThreads = 1; numberOfWorkerThreads < Environment.ProcessorCount*2; numberOfWorkerThreads++)
			{
				ssw.Start("THREADPOOL (" + numberOfWorkerThreads + " workerThreads):");
				threadPool = new ThreadPool(numberOfWorkerThreads, "testWorker");
				ssw.Split("Starting to enqueue.", 1);

				for (int i = 0; i < numberOfOperations; i++)
				{
					threadPool.EnqueueWorkItem(new Func<int[], double>(CalcAverage), GetNumbersForAverage());
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
					8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0,
					2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2,
					3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12,
					7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4,
					0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8,
					2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2,
					12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3,
					4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7,
					8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0,
					2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2,
					3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12,
					7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4,
					0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8,
					2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2,
					12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3,
					4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7,
					8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0,
					2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2,
					3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12,
					7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4,
					0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8,
					2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2,
					12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3,
					4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7,
					8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0,
					2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2,
					3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12,
					7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4,
					0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8,
					2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2,
					12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3,
					4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7,
					8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0,
					2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2,
					3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12,
					7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4,
					0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8,
					2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2,
					12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3,
					4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7,
					8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0, 2, 12, 7, 8, 2, 3, 4, 0,
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
			double average = 0.0;

			if (numbers == null || numbers.Length == 0)
			{
				return 0.0;
			}

			foreach (int number in numbers)
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
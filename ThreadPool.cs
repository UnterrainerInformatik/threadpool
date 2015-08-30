/**************************************************************************
 * 
 * Copyright (c) Unterrainer Informatik OG.
 * This source is subject to the Microsoft Public License.
 * 
 * See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
 * All other rights reserved.
 * 
 * (In other words you may copy, use, change and redistribute it without
 * any restrictions except for not suing me because it broke something.)
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
 * PURPOSE.
 * 
 ***************************************************************************/

using System.Collections.Generic;
using System.Threading;
using LockFreeQueue;

namespace ThreadPooling
{
	/// <summary>
	///     This class implements a thread-pool. It buffers all the work items, which regrettably have to be classes, and
	///     reuses them. It only cleans them up at
	///     shutdown.
	/// </summary>
	public partial class ThreadPool
	{
		/// <summary>
		///     The form of the callback delegate that carries the payload of the workItem.
		/// </summary>
		internal delegate object WorkItemCallback(object state);

		/// <summary>
		///     The callback function that should be called when the work item is finished.
		/// </summary>
		public delegate void CallbackFunction();

		private bool isDisposeDoneWorkItemsAutomatically;
		private readonly Queue<SingleThreadRunner> threads;
		private readonly LockFreeQueue<SingleThreadRunner> threadsIdle;
		private int threadsWorking;
		private readonly LockFreeQueue<WorkItem> workItemQueue = new LockFreeQueue<WorkItem>();

		private readonly LockFreeQueue<WorkItem> returnedWorkItems = new LockFreeQueue<WorkItem>();
		private bool shutDownSignaled;
		private readonly object lockObjectShutDownSignaled = new object();

		public int NumberOfThreads { get; private set; }

		/// <summary>
		///     Gets or sets a value indicating whether this instance is set to dispose done work items automatically. Beware: If
		///     you enable this option, the
		///     dispose method of each work item is called immediately after its completion, thus destroying the reference. This
		///     obviously is only an viable option
		///     when using the action-interface (no return values) together with the <c>WaitForEveryWorkerIdle</c>-Method.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is dispose done work items automatically; otherwise, <c>false</c>.
		/// </value>
		public bool IsDisposeDoneWorkItemsAutomatically
		{
			get
			{
				Thread.MemoryBarrier();
				return isDisposeDoneWorkItemsAutomatically;
			}
			set
			{
				isDisposeDoneWorkItemsAutomatically = value;
				Thread.MemoryBarrier();
			}
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="ThreadPool" /> class.
		/// </summary>
		public ThreadPool(int numberOfThreads, string threadsNamePrefix)
		{
			NumberOfThreads = numberOfThreads;
			threads = new Queue<SingleThreadRunner>();
			threadsIdle = new LockFreeQueue<SingleThreadRunner>();

			// allocate threads...
			for (int i = 0; i < NumberOfThreads; i++)
			{
				SingleThreadRunner singleThreadRunner = new SingleThreadRunner(this);
				singleThreadRunner.Thread = new Thread(singleThreadRunner.DoWork);
				singleThreadRunner.Thread.Name = threadsNamePrefix + (i + 1);
				singleThreadRunner.Thread.IsBackground = true;

				threads.Enqueue(singleThreadRunner);
				threadsIdle.Enqueue(singleThreadRunner);

				singleThreadRunner.Thread.Start();
			}
		}

		/// <summary>
		///     Clears the work item queue.
		/// </summary>
		public void ClearWorkItemQueue()
		{
			WorkItem wi = workItemQueue.Dequeue();
			while (wi != null)
			{
				wi = workItemQueue.Dequeue();
			}
		}

		/// <summary>
		///     The number of items that are still to be processed.
		/// </summary>
		/// <returns></returns>
		public int NumberOfItemsLeft()
		{
			Thread.MemoryBarrier();
			return workItemQueue.Count;
		}

		/// <summary>
		///     The number of items that are done processing and returned.
		/// </summary>
		/// <returns></returns>
		public int NumberOfItemsDone()
		{
			Thread.MemoryBarrier();
			return returnedWorkItems.Count;
		}

		/// <summary>
		///     Enqueues the work item.
		/// </summary>
		/// <param name="workItem">The work item.</param>
		internal void EnqueueWorkItemInternal(WorkItem workItem)
		{
			// look for an idle worker...
			SingleThreadRunner singleThreadRunner = threadsIdle.Dequeue();
			Thread.MemoryBarrier();
			if (singleThreadRunner != null)
			{
				// hand over the work item...
				workItem.SingleThreadRunner = singleThreadRunner;
				Interlocked.Increment(ref threadsWorking);
				singleThreadRunner.SignalWork(workItem);
			}
			else
			{
				// just enqueue the item since all workers are busy...
				workItemQueue.Enqueue(workItem);
			}
		}

		/// <summary>
		///     Dequeues the work item.
		/// </summary>
		/// <param name="singleThreadRunner">The single thread runner.</param>
		/// <param name="isGetNewOne">
		///     if set to <c>true</c> [is get new one].
		/// </param>
		/// <param name="returnedWorkItem">The returned work item.</param>
		/// <returns>
		///     <see langword="true" />, if a work item has been
		///     successfully dequeued. <see langword="false" /> otherwise.
		/// </returns>
		internal WorkItem DequeueWorkItemInternal(SingleThreadRunner singleThreadRunner, bool isGetNewOne,
												WorkItem returnedWorkItem = null)
		{
			if (returnedWorkItem != null)
			{
				returnedWorkItems.Enqueue(returnedWorkItem);
			}

			if (!shutDownSignaled && isGetNewOne)
			{
				WorkItem workItem = workItemQueue.Dequeue();
				if (workItem != null)
				{
					workItem.SingleThreadRunner = singleThreadRunner;
					return workItem;
				}
			}

			// If we are here, there is no more work to do left...
			// The worker has to be set to idle...
			Interlocked.Decrement(ref threadsWorking);
			threadsIdle.Enqueue(singleThreadRunner);
			return null;
		}

		private WorkItem GetWorkItem(CallbackFunction asyncCallback)
		{
			WorkItem workItem = returnedWorkItems.Dequeue();
			if (workItem == null)
			{
				workItem = new WorkItem();
				workItem.WorkItemStateTypeless = new WorkItemStateTypeless(workItem);
			}

			workItem.SingleThreadRunner = null;
			workItem.IsCompleted = false;
			workItem.Result = null;
			workItem.AsyncCallback = asyncCallback;
			return workItem;
		}

		/// <summary>
		///     Returns the work item.
		/// </summary>
		/// <param name="returnedWorkItem">The returned work item.</param>
		public void ReturnWorkItem(WorkItem returnedWorkItem)
		{
			returnedWorkItems.Enqueue(returnedWorkItem);
		}

		/// <summary>
		///     Waits for the queue to empty.
		/// </summary>
		public void WaitForEveryWorkerIdle()
		{
			// A spinWait ensures a yield from time to time, forcing the CPU to do a context switch, thus allowing other processes to finish.
			SpinWait spinWait = new SpinWait();
			while (threadsWorking > 0)
			{
				Thread.MemoryBarrier();
				spinWait.SpinOnce();
			}
		}

		/// <summary>
		///     Clears the work item cache of all returned and "to be reused" work items returned via the dispose-method of a
		///     work-item-state-struct.
		/// </summary>
		public void ClearWorkItemCache()
		{
			WorkItem w;
			do
			{
				w = returnedWorkItems.Dequeue();
			}
			while (w != null);
		}

		/// <summary>
		///     Aborts all active threads.
		/// </summary>
		public void ShutDown()
		{
			// First, we want to close. So stop dealing new work items...
			lock (lockObjectShutDownSignaled)
			{
				shutDownSignaled = true;
			}

			// signal the shutdown-command to all workers...
			if (threads.Count > 0)
			{
				foreach (SingleThreadRunner thread in threads)
				{
					thread.SignalShutDown();
				}
			}
		}

		/// <summary>
		///     Pauses all active threads.
		/// </summary>
		public void Sleep()
		{
			// signal the pause-command to all workers...
			if (threads.Count > 0)
			{
				foreach (SingleThreadRunner thread in threads)
				{
					thread.SignalPause();
				}
			}
		}

		/// <summary>
		///     Resumes all active threads.
		/// </summary>
		public void Wakeup()
		{
			// signal the resume-command to all workers...
			if (threads.Count > 0)
			{
				foreach (SingleThreadRunner thread in threads)
				{
					thread.SignalResume();
				}
			}
		}
	}
}
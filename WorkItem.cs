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

using System.Threading;

namespace ThreadPooling
{
	/// <summary>
	///     This is a wrapper-class for a work item.
	/// </summary>
	public class WorkItem
	{
		private object result;

		public bool IsCompleted { get; set; }
		internal ThreadPool.WorkItemCallback Delegate { get; set; }
		public object DelegateInputParameters { get; set; }
		public WorkItemStateTypeless WorkItemStateTypeless { get; set; }
		public SingleThreadRunner SingleThreadRunner { get; set; }

		/// <summary>
		///     Gets or sets the result.
		/// </summary>
		/// <value>The result.</value>
		public object Result
		{
			get
			{
				// SpinWait for the workItem to finish.
				SpinWait spinWait = new SpinWait();
				while (!IsCompleted)
				{
					spinWait.SpinOnce();
					Thread.MemoryBarrier();
				}
				return result;
			}
			set { result = value; }
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="WorkItem" /> class.
		/// </summary>
		internal WorkItem()
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="WorkItem" /> class.
		/// </summary>
		internal WorkItem(ThreadPool.WorkItemCallback functionDelegate, object delegateInputParameters)
		{
			Delegate = functionDelegate;
			DelegateInputParameters = delegateInputParameters;
			WorkItemStateTypeless = new WorkItemStateTypeless(this);
		}

		/// <summary>
		///     Gets or sets the async callback.
		/// </summary>
		/// <value>The async callback.</value>
		public ThreadPool.CallbackFunction AsyncCallback { get; set; }
	}
}
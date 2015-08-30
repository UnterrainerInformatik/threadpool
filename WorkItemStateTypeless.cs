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

namespace ThreadPooling
{
	/// <summary>
	///     The state and the remote control for a work item.
	/// </summary>
	public class WorkItemStateTypeless : IWorkItemStateTypeless
	{
		public WorkItem WorkItem { get; set; }

		/// <summary>
		///     Gets a value indicating whether this instance is completed gracefully.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is completed gracefully; otherwise, <c>false</c>.
		/// </value>
		public bool IsStopped
		{
			get { return WorkItem.IsCompleted; }
		}

		/// <summary>
		///     Gets the result.
		/// </summary>
		/// <value>The result.</value>
		public object Result
		{
			get { return WorkItem.Result; }
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="WorkItemStateTypeless" /> class.
		/// </summary>
		/// <param name="workItem">The work item.</param>
		public WorkItemStateTypeless(WorkItem workItem)
		{
			WorkItem = workItem;
		}

		/// <summary>
		///     Performs application-defined tasks associated with freeing,
		///     releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
		}
	}
}
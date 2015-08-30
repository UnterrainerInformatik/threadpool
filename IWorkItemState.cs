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

using System;

namespace ThreadPooling
{
	/// <summary>
	///     This is an interface for a thread-workItem.
	/// </summary>
	/// <typeparam name="T">The type of the result.</typeparam>
	public interface IWorkItemState<out T> : IDisposable
	{
		/// <summary>
		///     Gets a value indicating whether this instance is completed.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is completed; otherwise, <c>false</c>.
		/// </value>
		bool IsStopped { get; }

		/// <summary>
		///     Is a blocking operation.
		///     Gets the result.
		/// </summary>
		/// <value>The result.</value>
		T Result { get; }
	}

	/// <summary>
	///     This is an interface for a thread-workItem.
	/// </summary>
	public interface IWorkItemState : IDisposable
	{
		/// <summary>
		///     Gets a value indicating whether this instance is completed.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is completed; otherwise, <c>false</c>.
		/// </value>
		bool IsStopped { get; }

		/// <summary>
		///     Is a blocking operation.
		///     Waits for the work item to finish.
		/// </summary>
		/// <value>The result.</value>
		void Result();
	}
}
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
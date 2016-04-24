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
        public bool IsStopped => WorkItem.IsCompleted;

        /// <summary>
        ///     Gets the result.
        /// </summary>
        /// <value>The result.</value>
        public object Result => WorkItem.Result;

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
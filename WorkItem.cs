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
                var spinWait = new SpinWait();
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
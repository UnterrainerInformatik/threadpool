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
    ///     This is a frame for a single thread to run the defined payload.
    /// </summary>
    public class SingleThreadRunner
    {
        private bool signalClose;
        private bool signalWork;

        private WorkItem currentWorkItem;

        public ThreadPool ThreadPool { get; set; }
        public Thread Thread { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SingleThreadRunner" /> class.
        /// </summary>
        public SingleThreadRunner()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SingleThreadRunner" /> class.
        /// </summary>
        /// <param name="threadPool">The thread pool.</param>
        public SingleThreadRunner(ThreadPool threadPool)
        {
            ThreadPool = threadPool;
        }

        /// <summary>
        ///     Does the work reacting on and setting various signals.
        /// </summary>
        public void DoWork()
        {
            var spinWait = new SpinWait();
            while (!signalClose)
            {
                if (signalWork)
                {
                    while (currentWorkItem != null && !signalClose)
                    {
                        // Start the payload.
                        currentWorkItem.Result = currentWorkItem.Delegate(currentWorkItem.DelegateInputParameters);

                        // Set the work item to completed.
                        currentWorkItem.IsCompleted = true;

                        // Call the async callback - method, if available.
                        if (currentWorkItem.AsyncCallback != null)
                        {
                            currentWorkItem.AsyncCallback.Invoke();
                        }

                        // Dequeue the next work item.
                        if (ThreadPool.IsDisposeDoneWorkItemsAutomatically)
                        {
                            // Return the work item automatically for reuse, if preferred.
                            currentWorkItem = ThreadPool.DequeueWorkItemInternal(this, signalWork, currentWorkItem);
                        }
                        else
                        {
                            currentWorkItem = ThreadPool.DequeueWorkItemInternal(this, signalWork);
                        }
                    }
                    // The worker has no more work or is paused.
                    signalWork = false;
                }
                else
                {
                    spinWait.SpinOnce();
                }
            }
            // The thread is dead.
            signalClose = false;
        }

        /// <summary>
        ///     Signals this instance to immediately start doing some work.
        /// </summary>
        public void SignalWork(WorkItem workItemToProcess)
        {
            // Wait for the main loop to be not busy before changing the current workItem.
            var spinWait = new SpinWait();
            while (signalWork)
            {
                spinWait.SpinOnce();
                Thread.MemoryBarrier();
            }
            Interlocked.Exchange(ref currentWorkItem, workItemToProcess);
            signalWork = true;
            Thread.MemoryBarrier();
        }

        /// <summary>
        ///     Signals the workers to close.
        /// </summary>
        public void SignalShutDown()
        {
            signalClose = true;
        }

        /// <summary>
        ///     Signals the workers to pause.
        /// </summary>
        public void SignalPause()
        {
            Thread.MemoryBarrier();
            signalWork = false;
            Thread.MemoryBarrier();
        }

        /// <summary>
        ///     Signals the workers to resume.
        /// </summary>
        public void SignalResume()
        {
            Thread.MemoryBarrier();
            signalWork = true;
            Thread.MemoryBarrier();
        }

        /// <summary>
        ///     Aborts this instance.
        /// </summary>
        public void Abort()
        {
            Thread.Abort();
        }
    }
}
```
/**************************************************************************
 * 
 * by Unterrainer Informatik OG.
 * This is free and unencumbered software released into the public domain.
 * Anyone is free to copy, modify, publish, use, compile, sell, or
 * distribute this software, either in source code form or as a compiled
 * binary, for any purpose, commercial or non-commercial, and by any
 * means.
 *
 * In jurisdictions that recognize copyright laws, the author or authors
 * of this software dedicate any and all copyright interest in the
 * software to the public domain. We make this dedication for the benefit
 * of the public at large and to the detriment of our heirs and
 * successors. We intend this dedication to be an overt act of
 * relinquishment in perpetuity of all present and future rights to this
 * software under copyright law.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 * IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
 * OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
 * ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 *
 * For more information, please refer to <http://unlicense.org>
 * 
 * (In other words you may copy, use, change, redistribute and sell it without
 * any restrictions except for not suing me because it broke something.)
 * 
 ***************************************************************************/

```

# General  

This section contains various useful projects that should help your development-process.  

This section of our GIT repositories is free. You may copy, use or rewrite every single one of its contained projects to your hearts content.  
In order to get help with basic GIT commands you may try [the GIT cheat-sheet][coding] on our [homepage][homepage].  

This repository located on our  [homepage][homepage] is private since this is the master- and release-branch. You may clone it, but it will be read-only.  
If you want to contribute to our repository (push, open pull requests), please use the copy on github located here: [the public github repository][github]  

# ThreadPool  

This class implements a thread-pool.  
It buffers all the work items, which regrettably have to be classes, and reuses them. It only cleans them up at shutdown.  

The reason we wrote this although there is the Parallels library is because unlike with that library you can enqueue different workers in a single thread-pool. It takes every function that is wrapped as a workload regardless of how many parameters or return-values it has. You can mix them all together and start the pool.  

You may wait for the whole pool to finis (join) or on every single work-item.

#### Example  
    
```csharp
threadPool = new ThreadPool(Environment.ProcessorCount, "testWorker");

List<IWorkItemState<bool>> wirs = new List<IWorkItemState<bool>>();

for (int i = 0; i < 50; i++)
{
	IWorkItemState<bool> wir = threadPool.EnqueueWorkItem(new Func<bool, bool>(Not), true);
	wirs.Add(wir);
}

threadPool.WaitForEveryWorkerIdle();
threadPool.ShutDown();
```

You may as well pause and resume whenever you please.

```csharp
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
```

Take a look at the included unit-tests for further guidance.

[homepage]: http://www.unterrainer.info
[coding]: http://www.unterrainer.info/Home/Coding
[github]: https://github.com/UnterrainerInformatik/threadpool
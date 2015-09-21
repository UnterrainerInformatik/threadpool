```
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
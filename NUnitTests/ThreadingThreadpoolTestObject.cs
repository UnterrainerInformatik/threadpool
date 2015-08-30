﻿// *************************************************************************** 
//  Copyright (c) 2013 by Unterrainer Informatik OG.
//  This source is licensed to Unterrainer Informatik OG.
//  All rights reserved.
//  
//  In other words:
//  YOU MUST NOT COPY, USE, CHANGE OR REDISTRIBUTE ANY ART, MUSIC, CODE OR
//  OTHER DATA, CONTAINED WITHIN THESE DIRECTORIES WITHOUT THE EXPRESS
//  PERMISSION OF Unterrainer Informatik OG.
// 
//  Classes using other, less restrictive licenses are explicitly marked.
// ---------------------------------------------------------------------------
//  Programmer: G U, 
//  Created: 2013-03-31
// ***************************************************************************

namespace ThreadPooling.ThreadPooling
{
	public class ThreadingThreadpoolTestObject
	{
		private readonly object lockObject = new object();
		public int NumberOfCalls;

		public void Call()
		{
			lock (lockObject)
			{
				NumberOfCalls++;
			}
		}
	}
}
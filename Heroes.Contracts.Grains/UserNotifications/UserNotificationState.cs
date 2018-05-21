﻿using System;
using System.Diagnostics;

namespace Heroes.Contracts.Grains.UserNotifications
{
	public class UserNotificationState
	{
		public UserNotification UserNotification { get; set; }
	}

	[Serializable, DebuggerDisplay("{DebuggerDisplay,nq}")]
	public class UserNotification
	{
		protected string DebuggerDisplay => $"MessageCount: '{MessageCount}'";
		public int MessageCount { get; set; }

		public override string ToString() => DebuggerDisplay;
	}
}
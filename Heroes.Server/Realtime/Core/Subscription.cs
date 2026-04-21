using System.Reactive.Subjects;
using Orleans.Streams;

namespace Heroes.Server.Realtime.Core;

public class Subscription<T>
{
	public required StreamSubscriptionHandle<T> Stream { get; init; }
	public required Subject<T> Subject { get; init; }
}
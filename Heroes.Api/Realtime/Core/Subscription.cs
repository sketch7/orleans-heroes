using System.Reactive.Subjects;
using Orleans.Streams;

namespace Heroes.Api.Realtime.Core
{
	public class Subscription<T>
	{
		public StreamSubscriptionHandle<T> Stream { get; set; }
		public Subject<T> Subject { get; set; }
	}
}
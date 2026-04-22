using System.Threading.Channels;

namespace Heroes.Server.Infrastructure;

public static class ObservableExtensions
{
	/// <summary>Bridges an <see cref="IObservable{T}"/> to a <see cref="ChannelReader{T}"/> without back-pressure.</summary>
	/// <remarks>If the channel is unbounded and the consumer is slower than the producer, memory will increase.
	/// Pass <paramref name="maxBufferSize"/> for a bounded channel that drops items when full.</remarks>
	public static ChannelReader<T> AsChannelReader<T>(this IObservable<T> observable, int? maxBufferSize = null)
	{
		var channel = maxBufferSize != null
			? Channel.CreateBounded<T>(maxBufferSize.Value)
			: Channel.CreateUnbounded<T>();

		var disposable = observable.Subscribe(
			value => channel.Writer.TryWrite(value),
			error => channel.Writer.TryComplete(error),
			() => channel.Writer.TryComplete());

		channel.Reader.Completion.ContinueWith(_ => disposable.Dispose());

		return channel.Reader;
	}
}

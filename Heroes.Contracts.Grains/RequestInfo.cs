using System.Diagnostics;

namespace Heroes.Contracts.Grains
{
	public interface IRequestInfo
	{
		string TraceId { get; set; }
		string UserAgent { get; set; }
	}

	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	public class RequestInfo : IRequestInfo
	{
		private string DebuggerDisplay => $"TraceId: '{TraceId}', UserAgent: '{UserAgent}'";

		public string TraceId { get; set; }
		public string UserAgent { get; set; }
	}
}
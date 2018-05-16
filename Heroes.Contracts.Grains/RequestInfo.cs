using System.Diagnostics;

namespace Heroes.Contracts.Grains
{
	public interface IRequestInfo
	{
		string TraceId { get; set; }
		string UserAgent { get; set; }
		string Tenant { get; set; }
	}

	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	public class RequestInfo : IRequestInfo
	{
		private string DebuggerDisplay => $"TraceId: '{TraceId}', UserAgent: '{UserAgent}'";

		public string TraceId { get; set; }
		public string UserAgent { get; set; }
		public string Tenant { get; set; }
	}
}
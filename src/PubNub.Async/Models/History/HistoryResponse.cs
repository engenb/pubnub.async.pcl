using System.Collections.Generic;

namespace PubNub.Async.Models.History
{
	public class HistoryResponse<TMessage>
	{
		public string Error { get; set; }

		public IEnumerable<HistoryMessage<TMessage>> Messages { get; set; }
		public long Start { get; set; }
		public long End { get; set; }
	}
}
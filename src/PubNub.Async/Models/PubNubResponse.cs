using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubNub.Async.Models
{
	public class PubNubResponse<TContent>
	{
		public TContent Content { get; set; }
	}
}

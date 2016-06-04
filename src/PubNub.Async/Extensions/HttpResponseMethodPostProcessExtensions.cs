﻿using System.Net.Http;
using System.Threading.Tasks;

namespace PubNub.Async.Extensions
{
	public static class HttpResponseMethodPostProcessExtensions
	{
		public static async Task<HttpResponseMessage> ProcessResponse(this Task<HttpResponseMessage> responseTask)
		{
			return (await responseTask.ConfigureAwait(false))
				.StripCharsetQuotes();
		}

		private static HttpResponseMessage StripCharsetQuotes(this HttpResponseMessage response)
		{
			if (response?.Content?.Headers?.ContentType?.CharSet != null)
			{
				response.Content.Headers.ContentType.CharSet = response.Content.Headers.ContentType.CharSet.Replace("\"",
					string.Empty);
			}
			return response;
		}
	}
}
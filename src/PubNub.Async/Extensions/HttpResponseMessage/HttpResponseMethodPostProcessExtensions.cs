using System.Threading.Tasks;

namespace PubNub.Async.Extensions.HttpResponseMessage
{
	public static class HttpResponseMethodPostProcessExtensions
	{
		public static async Task<System.Net.Http.HttpResponseMessage> ProcessResponse(this Task<System.Net.Http.HttpResponseMessage> responseTask)
		{
			return (await responseTask.ConfigureAwait(false))
				.StripCharsetQuotes();
		}

		private static System.Net.Http.HttpResponseMessage StripCharsetQuotes(this System.Net.Http.HttpResponseMessage response)
		{
			if (response?.Content?.Headers?.ContentType?.CharSet != null)
			{
				response.Content.Headers.ContentType.CharSet = response.Content.Headers.ContentType.CharSet.Replace("\"", string.Empty);
			}
			return response;
		}
	}
}

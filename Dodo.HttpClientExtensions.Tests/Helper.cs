using System.Net.Http;
using System.Threading.Tasks;

namespace Dodo.HttpClientExtensions.Tests
{
	public static class Helper
	{
		public static async Task InvokeMultipleHttpRequests(HttpClient client, int taskCount)
		{
			var tasks = new Task[taskCount];
			for (var i = 0; i < taskCount; i++)
			{
				var requestMessage = new HttpRequestMessage(HttpMethod.Get, "http://localhost");
				requestMessage.Headers.Add("TaskId", i.ToString());
				tasks[i] = client.SendAsync(requestMessage);
			}

			await Task.WhenAll(tasks);
		}
	}
}

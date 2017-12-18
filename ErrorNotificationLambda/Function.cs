using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using System.IO.Compression;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Amazon.Lambda.Core;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace ErrorNotificationLambda
{
    public class Function
    {

		/// <summary>
        /// Notify contents waritten out on CloudWatch Logs
		/// </summary>
		/// <param name="logEvent"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public async Task<bool> FunctionHandler(LogEvent logEvent, ILambdaContext context)
		{
			var slackWebhookUrl = Environment.GetEnvironmentVariable("SLACK_WEBHOOK_URL");
			var cloudWatchLogGroupUrl = Environment.GetEnvironmentVariable("CLOUDWATCH_LOG_GROUP_URL");
			var cloudWatchMetricsUrl = Environment.GetEnvironmentVariable("CLOUDWATCH_METRICS_URL");

			var payload = new
			{
				channel = "dev",
				username = "CloudWatch Notification",
                text = $"{Decode(logEvent.Awslogs.Data)}"+ Environment.NewLine +
                       $"Logs: <{cloudWatchLogGroupUrl}|Click here>" + Environment.NewLine +
                       $"Metrics: <{cloudWatchMetricsUrl}|Click here>",
			};

			var jsonString = JsonConvert.SerializeObject(payload);

			var content = new FormUrlEncodedContent(new Dictionary<string, string>
			{
				{ "payload", jsonString}
			});

			try
			{
				using (var client = new HttpClient())
				{
					await client.PostAsync(slackWebhookUrl, content);
				}
			}
			catch (Exception e)
			{
				context.Logger.LogLine("error!!!!" + Environment.NewLine + $"{e.Message}" + Environment.NewLine + $"{e.StackTrace}");
				throw;
			}
			
			return true;
		}

        private string Decode(string encodedString)
        {
            var decodedString = "";

            byte[] data = Convert.FromBase64String(encodedString);

            using (GZipStream stream = new GZipStream(new MemoryStream(data), CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);

                    var messages = JObject.Parse(Encoding.UTF8.GetString(memory.ToArray())).GetValue("logEvents");
                    foreach (var item in messages)
                    {
                        decodedString += item["message"].Value<string>() + Environment.NewLine;
                    }
                }
            }

            return decodedString;
        }

    }

	public class LogEvent
	{
		public  Log Awslogs { get; set; }
		public class Log
		{
			public  string Data { get; set; }
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Amazon.Lambda.Core;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace ErrorNotificationLambda
{
    public class Function
    {

		/// <summary>
		/// CloudWatch Logsに書き出された内容を通知する
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
				username = "通知奴",
				text = $"{logEvent.Awslogs.Data}\nLogs: <{cloudWatchLogGroupUrl}|Click here>\nMetrics: <{cloudWatchMetricsUrl}|Click here>",
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

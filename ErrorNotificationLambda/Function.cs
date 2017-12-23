using Amazon.Lambda.CloudWatchLogsEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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
		public async Task<bool> FunctionHandler(CloudWatchLogsEvent logEvent, ILambdaContext context)
		{
			var slackWebhookUrl = Environment.GetEnvironmentVariable("SLACK_WEBHOOK_URL");
			var cloudWatchLogGroupUrl = Environment.GetEnvironmentVariable("CLOUDWATCH_LOG_GROUP_URL");
			var cloudWatchMetricsUrl = Environment.GetEnvironmentVariable("CLOUDWATCH_METRICS_URL");

			var payload = new
			{
				channel = "dev",
				username = "CloudWatch Notification",
                text = $"{GetMessage(logEvent.Awslogs.DecodeData())}"+ Environment.NewLine +
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

	    private static string GetMessage(string decodedData)
	    {
		    string extractedMessage = "";
		    var messages = JObject.Parse(decodedData).GetValue("logEvents");
		    foreach (var item in messages)
		    {
			    extractedMessage += item["message"].Value<string>() + Environment.NewLine;
		    }
            return extractedMessage;
	    }

    }
}

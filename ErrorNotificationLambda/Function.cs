using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace ErrorNotificationLambda
{
    public class Function
    {

		/// <summary>
		/// CloudWatch Logs‚É‘‚«o‚³‚ê‚½“à—e‚ğ’Ê’m‚·‚é
		/// </summary>
		/// <param name="input"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public string FunctionHandler(LogEvent logEvent, ILambdaContext context)
		{
			return logEvent.Awslogs.Data;
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

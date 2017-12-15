using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;

using ErrorNotificationLambda;

namespace ErrorNotificationLambda.Tests
{
    public class FunctionTest
    {
        [Fact]
        public void TestToUpperFunction()
        {
            var function = new Function();
            var context = new TestLambdaContext();

	        var evnt = new LogEvent
	        {
		        Awslogs = new LogEvent.Log
				{
			        Data = "error!"
		        }
	        };

			var notification = function.FunctionHandler(evnt, context);

            Assert.Equal("error!", notification);
        }
    }
}

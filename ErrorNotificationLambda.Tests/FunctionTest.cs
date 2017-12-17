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
    public class FunctionTest : IClassFixture<LaunchSettingsFixture>
    {
	    LaunchSettingsFixture _fixture;

	    public FunctionTest(LaunchSettingsFixture fixture)
	    {
		    _fixture = fixture;
	    }

		[Fact]
        public async void TestErrorNotificationFunction()
        {
			var function = new Function();
            var context = new TestLambdaContext();

	        var evnt = new LogEvent
	        {
		        Awslogs = new LogEvent.Log
				{
			        Data = "error!" + Environment.NewLine + "stackTrace at hogehogehogehoge"
				}
	        };

			var notification = await function.FunctionHandler(evnt, context);

            Assert.True(notification);
        }
    }
}

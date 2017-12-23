using Amazon.Lambda.CloudWatchLogsEvents;
using Amazon.Lambda.TestUtilities;
using Xunit;

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

            var evnt = new CloudWatchLogsEvent()
            {
                Awslogs = new CloudWatchLogsEvent.Log
                {
                    EncodedData = "H4sIAAAAAAAAAHWPwQqCQBCGX0Xm7EFtK+smZBEUgXoLCdMhFtKV3akI8d0bLYmibvPPN3wz00CJxmQnTO41whwWQRIctmEcB6sQbFC3CjW3XW8kxpOpP+OC22d1Wml1qZkQGtoMsScxaczKN3plG8zlaHIta5KqWsozoTYw3/djzwhpLwivWFGHGpAFe7DL68JlBUk+l7KSN7tCOEJ4M3/qOI49vMHj+zCKdlFqLaU2ZHV2a4Ct/an0/ivdX8oYc1UVX860fQDQiMdxRQEAAA=="
                }
            };

            var notification = await function.FunctionHandler(evnt, context);

            Assert.True(notification);
        }
    }
}

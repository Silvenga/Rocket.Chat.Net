namespace Rocket.Chat.Net.Tests
{
    using Rocket.Chat.Net.Interfaces;

    using Xunit.Abstractions;

    public class XUnitLogger : ILogger
    {
        private readonly ITestOutputHelper _helper;

        public XUnitLogger(ITestOutputHelper helper)
        {
            _helper = helper;
        }

        public void Debug(string message)
        {
            _helper.WriteLine(message);
        }

        public void Info(string message)
        {
            _helper.WriteLine(message);
        }
    }
}
namespace Rocket.Chat.Net.Tests.Helpers
{
    using System;

    using Rocket.Chat.Net.Interfaces;

    using Xunit.Abstractions;

    public class XUnitLogger : ILogger, IDisposable
    {
        private readonly ITestOutputHelper _helper;

        public bool IsDisposed { get; set; }

        public XUnitLogger(ITestOutputHelper helper)
        {
            _helper = helper;
        }

        public void Debug(string message)
        {
            if (IsDisposed)
            {
                return;
            }
            _helper.WriteLine(message);
        }

        public void Info(string message)
        {
            if (IsDisposed)
            {
                return;
            }
            _helper.WriteLine(message);
        }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}
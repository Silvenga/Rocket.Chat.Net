namespace Rocket.Chat.Net.Tests.Helpers
{
    using System;

    public static class Constants
    {
        public static readonly string RocketServer =
            Environment.GetEnvironmentVariable(nameof(RocketServer))
            ?? "dev0:3000";

        public static readonly string RocketUsername =
            Environment.GetEnvironmentVariable(nameof(RocketUsername))
            ?? "mark.lopez";

        public static readonly string RocketEmail =
            Environment.GetEnvironmentVariable(nameof(RocketEmail))
            ?? "m@silvenga.com";

        public static readonly string RocketPassword =
            Environment.GetEnvironmentVariable(nameof(RocketPassword))
            ?? "silverlight";

        public static readonly string TestUsername =
            Environment.GetEnvironmentVariable(nameof(TestUsername))
            ?? "test.user";

        public static readonly string TestEmail =
            Environment.GetEnvironmentVariable(nameof(TestEmail))
            ?? "test.user@silvenga.com";
    }
}
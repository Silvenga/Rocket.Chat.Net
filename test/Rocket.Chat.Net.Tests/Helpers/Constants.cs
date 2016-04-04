namespace Rocket.Chat.Net.Tests.Helpers
{
    using System;

    public static class Constants
    {
        public static readonly string RocketServer =
            Environment.GetEnvironmentVariable(nameof(RocketServer))
            ?? "dev0:3000";

        public static readonly string Username =
            Environment.GetEnvironmentVariable(nameof(Username))
            ?? "mark.lopez";

        public static readonly string Email =
            Environment.GetEnvironmentVariable(nameof(Email))
            ?? "m@silvenga.com";

        public static readonly string Password =
            Environment.GetEnvironmentVariable(nameof(Password))
            ?? "silverlight";

        public static readonly string TestUsername =
            Environment.GetEnvironmentVariable(nameof(TestUsername))
            ?? "test.user";

        public static readonly string TestEmail =
            Environment.GetEnvironmentVariable(nameof(TestEmail))
            ?? "test.user@silvenga.com";
    }
}
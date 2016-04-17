namespace Rocket.Chat.Net.Tests.Helpers
{
    using System;

    public static class Constants
    {
        public static readonly string RocketServer =
            Environment.GetEnvironmentVariable(nameof(RocketServer))
            ?? "dev0:3000";

        public static readonly string OneUsername =
            Environment.GetEnvironmentVariable(nameof(OneUsername))
            ?? "user.one";

        public static readonly string OneEmail =
            Environment.GetEnvironmentVariable(nameof(OneEmail))
            ?? "one@silvenga.com";

        public static readonly string OneName =
            Environment.GetEnvironmentVariable(nameof(OneName))
            ?? "User One";

        public static readonly string OnePassword =
            Environment.GetEnvironmentVariable(nameof(OnePassword))
            ?? "silverlight";

        public static readonly string TwoUsername =
            Environment.GetEnvironmentVariable(nameof(TwoUsername))
            ?? "user.two";

        public static readonly string TwoEmail =
            Environment.GetEnvironmentVariable(nameof(TwoEmail))
            ?? "two@silvenga.com";

        public static readonly string TwoName =
            Environment.GetEnvironmentVariable(nameof(TwoName))
            ?? "User Two";

        public static readonly string TwoPassword =
            Environment.GetEnvironmentVariable(nameof(TwoPassword))
            ?? "silverlight";
    }
}
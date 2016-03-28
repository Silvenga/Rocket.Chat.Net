namespace Rocket.Chat.Net.Loggers
{
    using System;

    using Rocket.Chat.Net.Interfaces;

    public class ConsoleLogger : ILogger
    {
        public void Debug(string message)
        {
            Console.WriteLine($"DEBUG: {message}");
        }

        public void Info(string message)
        {
            Console.WriteLine($"INFO: {message}");
        }
    }
}
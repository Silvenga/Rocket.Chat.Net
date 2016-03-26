# Rocket.Chat.Net

[![Build status](https://ci.appveyor.com/api/projects/status/0d4nc078d7dqgj89?svg=true)](https://ci.appveyor.com/project/Silvenga/rocket-chat-net) [![NuGet](https://img.shields.io/nuget/vpre/Rocket.Chat.Net.svg)](https://www.nuget.org/packages/Rocket.Chat.Net/)

A Rocket.Chat real-time, managed, .Net driver, and bot. 

## Usage

```csharp
const string username = "m@silvenga.com";
const string password = "silverlight";
const string rocketServerUrl = "dev0:3000"; // just the host and port
const bool useSsl = false; // Basically use ws or wss.

// Basic logger
ILogger logger = new ConsoleLogger();

// Create the bot - an abstraction of the driver
RocketChatBot bot = new RocketChatBot(rocketServerUrl, useSsl, logger);

// Connect to Rocket.Chat
await bot.ConnectAsync();

// Login
ILoginOption loginOption = new EmailLoginOption
{
    Email = username,
    Password = password
};
await bot.LoginAsync(loginOption);

// Start listening for messages
await bot.SubscribeAsync();

// Add possible responses to be checked in order
// This is not thead safe, FYI 
IBotResponse giphyResponse = new GiphyResponse();
bot.AddResponse(giphyResponse);

// And that's it
// Checkout GiphyResponse in the example project for more info.
```

## TODO

[docs/todo.md](docs/todo.md)
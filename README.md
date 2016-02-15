# Rocket.Chat.Net

[![Build status](https://ci.appveyor.com/api/projects/status/0d4nc078d7dqgj89?svg=true)](https://ci.appveyor.com/project/Silvenga/rocket-chat-net) [![NuGet](https://img.shields.io/nuget/vpre/Rocket.Chat.Net.svg)](https://www.nuget.org/packages/Rocket.Chat.Net/)

A Rocket.Chat real-time, managed, .Net driver, and bot

## Usage

```csharp
const string username = "m@silvenga.com";
const string password = "silverlight";
const string rocketServerUrl = "dev0:3000"; // just the host and port
const bool useSsl = false; // Basically use ws or wss.

// Basic logger
ILogger logger = new ConsoleLogger();

// Create the rocket driver - will connect to the server using websockets
_driver = new RocketChatDriver(rocketServerUrl, useSsl, logger);

// Request connection to Rocket.Chat
await _driver.ConnectAsync();

// Login with a email address (opposed to logging in with LDAP or Username)
await _driver.LoginWithEmailAsync(username, password);

// Most rooms have a GUID - GENERAL is always called GENERAL
string roomId = await _driver.GetRoomIdAsync("GENERAL");

// Join the room if not already joined
await _driver.JoinRoomAsync(roomId);

// Start listening for messages
// Don't specify a roomId if you want to listen on all channels
await _driver.SubscribeToRoomAsync(roomId);

// Create the bot - an abstraction of the driver
RocketChatBot bot = new RocketChatBot(_driver, logger);

// Add possible responses to be checked in order
// This is not thead safe, FYI
IBotResponse giphyResponse = new GiphyResponse();
bot.AddResponse(giphyResponse);

// And that's it
// Checkout GiphyResponse in the example project for more info.
```

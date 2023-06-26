﻿namespace Rocket.Chat.Net.Tests.Integration
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using FluentAssertions;

    using Ploeh.AutoFixture;

    using Rocket.Chat.Net.Driver;
    using Rocket.Chat.Net.Interfaces;
    using Rocket.Chat.Net.Models;

    using NLog;

    using Xunit;
    using Xunit.Abstractions;
    using Rocket.Chat.Net.Tests.Helpers;
    using Rocket.Chat.Net.Models.LoginOptions;
    using Action = Net.Models.Action;
    using System.IO;
    using System.Reflection;
    using RestSharp;
    using System.Collections.Generic;

    [Trait("Category", "Driver")]
    public class MessagingFacts : IDisposable
    {
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(15);
        private readonly MessagingFixture _fixture;
        private static readonly Fixture AutoFixture = new Fixture();

        public MessagingFacts(ITestOutputHelper helper)
        {
            var roomName = AutoFixture.Create<string>();
            _fixture = new MessagingFixture(helper, roomName);
        }

        [Fact]
        public async Task Can_send_messages()
        {
            var text = AutoFixture.Create<string>();
            var messageReceived = new AutoResetEvent(false);

            RocketMessage message = null;
            _fixture.Master.Driver.MessageReceived += rocketMessage =>
            {
                if (rocketMessage.RoomId != _fixture.RoomId)
                {
                    return;
                }
                message = rocketMessage;
                messageReceived.Set();
            };

            await _fixture.Master.InitAsync(Constants.OneUsername, Constants.OnePassword);

            // Act
            var result = await _fixture.Master.Driver.SendMessageAsync(text, _fixture.RoomId);

            messageReceived.WaitOne(_timeout);

            // Assert
            result.HasError.Should().BeFalse();
            message.Should().NotBeNull();
            result.Result.ToString().Should().Be(message.ToString());
        }

        [Fact]
        public async Task Can_send_attachment()
        {
            var attachment = AutoFixture.Build<Attachment>()
                                        .Without(x => x.Timestamp)
                                        .Create();

            var messageReceived = new AutoResetEvent(false);

            RocketMessage message = null;
            _fixture.Master.Driver.MessageReceived += rocketMessage =>
            {
                if (rocketMessage.RoomId != _fixture.RoomId)
                {
                    return;
                }
                message = rocketMessage;
                messageReceived.Set();
            };

            await _fixture.Master.InitAsync(Constants.OneUsername, Constants.OnePassword);

            // Act
            var result = await _fixture.Master.Driver.SendCustomMessageAsync(attachment, _fixture.RoomId);

            messageReceived.WaitOne(_timeout);

            // Assert
            result.HasError.Should().BeFalse();
            message.Should().NotBeNull();
            message.Message.Should().BeEmpty();
            message.Attachments.Should().HaveCount(1);

            var resultAttachment = message.Attachments.First();
            resultAttachment.Should().Be(attachment);
        }

        [Fact]
        public async Task Can_update_messages()
        {
            var originalText = AutoFixture.Create<string>();
            var text = AutoFixture.Create<string>();
            var messageReceived = new AutoResetEvent(false);

            RocketMessage resultMessage = null;

            _fixture.Master.Driver.MessageReceived += rocketMessage =>
            {
                if (rocketMessage.Message == text)
                {
                    messageReceived.Set();
                    resultMessage = rocketMessage;
                }
            };

            await _fixture.Master.InitAsync(Constants.OneUsername, Constants.OnePassword);
            var message = await _fixture.Master.Driver.SendMessageAsync(originalText, _fixture.RoomId);

            // Act
            var result = await _fixture.Master.Driver.UpdateMessageAsync(message.Result.Id, message.Result.RoomId,
                text);

            messageReceived.WaitOne(_timeout);

            // Assert
            result.HasError.Should().BeFalse();
            resultMessage.Message.Should().Be(text);
            resultMessage.WasEdited.Should().BeTrue();
        }

        [Fact]
        public async Task Can_pin_message()
        {
            // TODO - add better coverage

            var text = AutoFixture.Create<string>();

            await _fixture.Master.InitAsync(Constants.OneUsername, Constants.OnePassword);

            // Act
            var sentMessageResult = await _fixture.Master.Driver.SendMessageAsync(text, _fixture.RoomId);
            var pinMessageResult = await _fixture.Master.Driver.PinMessageAsync(sentMessageResult.Result);
            var unpinMessageResult = await _fixture.Master.Driver.UnpinMessageAsync(sentMessageResult.Result);

            // Assert
            sentMessageResult.HasError.Should().BeFalse();
            pinMessageResult.HasError.Should().BeFalse();
            unpinMessageResult.HasError.Should().BeFalse();
        }

        [Fact]
        public async Task Can_delete_message()
        {
            var text = AutoFixture.Create<string>();
            var messageReceived = new AutoResetEvent(false);

            RocketMessage message = null;
            _fixture.Master.Driver.MessageReceived += rocketMessage =>
            {
                if (rocketMessage.RoomId != _fixture.RoomId)
                {
                    return;
                }
                message = rocketMessage;
                messageReceived.Set();
            };

            await _fixture.Master.InitAsync(Constants.OneUsername, Constants.OnePassword);

            // Act
            await _fixture.Master.Driver.SendMessageAsync(text, _fixture.RoomId);

            messageReceived.WaitOne(_timeout);

            var result = await _fixture.Master.Driver.DeleteMessageAsync(message.Id, _fixture.RoomId);

            // Assert
            result.HasError.Should().BeFalse();
        }

        [Fact]
        public async Task Can_set_reaction()
        {
            const string reactionName = ":grinning:";
            var text = AutoFixture.Create<string>();
            var messageReceived = new AutoResetEvent(false);

            RocketMessage message = null;
            _fixture.Master.Driver.MessageReceived += rocketMessage =>
            {
                if (rocketMessage.RoomId != _fixture.RoomId)
                {
                    return;
                }
                message = rocketMessage;
                messageReceived.Set();
            };

            await _fixture.Master.InitAsync(Constants.OneUsername, Constants.OnePassword);
            var messageResult = await _fixture.Master.Driver.SendMessageAsync(text, _fixture.RoomId);

            messageReceived.WaitOne(_timeout);
            messageReceived.Reset();

            // Act
            var result = await _fixture.Master.Driver.SetReactionAsync(reactionName, messageResult.Result.Id);

            messageReceived.WaitOne(_timeout);

            // Assert
            result.HasError.Should().BeFalse();
            message.Reactions.Should().HaveCount(1);
            message.Reactions[0].Name.Should().Be(reactionName);
        }

        [Fact]
        public async Task Messages_received_should_populate_created_by()
        {
            var text = AutoFixture.Create<string>();
            var messageReceived = new AutoResetEvent(false);

            RocketMessage message = null;
            _fixture.Master.Driver.MessageReceived += rocketMessage =>
            {
                if (rocketMessage.RoomId != _fixture.RoomId)
                {
                    return;
                }
                message = rocketMessage;
                messageReceived.Set();
            };

            await _fixture.Master.InitAsync(Constants.OneUsername, Constants.OnePassword);
            var userId = ((RocketChatDriver) _fixture.Master.Driver).UserId;

            // Act
            await _fixture.Master.Driver.SendMessageAsync(text, _fixture.RoomId);

            messageReceived.WaitOne(_timeout);

            // Assert
            message.Should().NotBeNull();
            message.CreatedBy.Id.Should().Be(userId);
            message.CreatedBy.Username.Should().Be(Constants.OneUsername);
        }

        [Fact]
        public async Task When_sending_messages_bot_flag_should_be_set()
        {
            var text = AutoFixture.Create<string>();
            var messageReceived = new AutoResetEvent(false);

            RocketMessage message = null;
            _fixture.Master.Driver.MessageReceived += rocketMessage =>
            {
                if (rocketMessage.RoomId != _fixture.RoomId)
                {
                    return;
                }
                message = rocketMessage;
                messageReceived.Set();
            };

            await _fixture.Master.InitAsync(Constants.OneUsername, Constants.OnePassword);

            // Act
            await _fixture.Master.Driver.SendMessageAsync(text, _fixture.RoomId);

            messageReceived.WaitOne(_timeout);

            // Assert
            message.Should().NotBeNull();
            message.IsBot.Should().BeTrue();
        }

        [Fact]
        public async Task When_bot_is_mentioned_set_mentioned_flag()
        {
            var text = AutoFixture.Create<string>() + " @" + Constants.OneUsername;

            var masterReceived = new AutoResetEvent(false);
            RocketMessage masterMessage = null;
            _fixture.Master.Driver.MessageReceived += rocketMessage =>
            {
                if (rocketMessage.RoomId != _fixture.RoomId)
                {
                    return;
                }
                masterMessage = rocketMessage;
                masterReceived.Set();
            };

            var slaveReceived = new AutoResetEvent(false);
            RocketMessage slaveMessage = null;
            _fixture.Slave.Driver.MessageReceived += rocketMessage =>
            {
                if (rocketMessage.RoomId != _fixture.RoomId)
                {
                    return;
                }
                slaveMessage = rocketMessage;
                slaveReceived.Set();
            };

            await _fixture.Master.InitAsync(Constants.OneUsername, Constants.OnePassword);
            await _fixture.Slave.InitAsync(Constants.TwoUsername, Constants.TwoPassword);

            // Act
            await _fixture.Master.Driver.SendMessageAsync(text, _fixture.RoomId);

            masterReceived.WaitOne(_timeout);
            slaveReceived.WaitOne(_timeout);

            // Assert
            masterMessage.Should().NotBeNull();
            masterMessage.IsBotMentioned.Should().BeTrue();

            slaveMessage.Should().NotBeNull();
            slaveMessage.IsBotMentioned.Should().BeFalse();
        }

        [Fact]
        public async Task When_message_is_sent_set_room_property()
        {
            var text = AutoFixture.Create<string>();

            var masterReceived = new AutoResetEvent(false);
            RocketMessage masterMessage = null;
            _fixture.Master.Driver.MessageReceived += rocketMessage =>
            {
                if (rocketMessage.RoomId != _fixture.RoomId)
                {
                    return;
                }
                masterMessage = rocketMessage;
                masterReceived.Set();
            };

            var slaveReceived = new AutoResetEvent(false);
            RocketMessage slaveMessage = null;
            _fixture.Slave.Driver.MessageReceived += rocketMessage =>
            {
                if (rocketMessage.RoomId != _fixture.RoomId)
                {
                    return;
                }
                slaveMessage = rocketMessage;
                slaveReceived.Set();
            };

            await _fixture.Master.InitAsync(Constants.OneUsername, Constants.OnePassword);
            await _fixture.Slave.InitAsync(Constants.TwoUsername, Constants.TwoPassword);

            // await _fixture.Master.Driver.SubscribeToRoomAsync(_fixture.RoomId);
            // await _fixture.Slave.Driver.SubscribeToRoomAsync(_fixture.RoomId);

            // Act
            var result = await _fixture.Master.Driver.SendMessageAsync(text, _fixture.RoomId);
            result.HasError.Should().BeFalse();

            masterReceived.WaitOne(_timeout);
            slaveReceived.WaitOne(_timeout);

            // Assert
            masterMessage.Should().NotBeNull();
            masterMessage.Room.Should().NotBeNull();

            slaveMessage.Should().NotBeNull();
            slaveMessage.Room.Should().BeNull(); // User is not apart of this room, should be null
        }

        [Fact]
        public async Task When_bot_sends_message_on_receive_set_myself_flag()
        {
            var text = AutoFixture.Create<string>() + " @" + Constants.TwoUsername;

            var masterReceived = new AutoResetEvent(false);
            RocketMessage masterMessage = null;
            _fixture.Master.Driver.MessageReceived += rocketMessage =>
            {
                if (rocketMessage.RoomId != _fixture.RoomId)
                {
                    return;
                }
                masterMessage = rocketMessage;
                masterReceived.Set();
            };

            var slaveReceived = new AutoResetEvent(false);
            RocketMessage slaveMessage = null;
            _fixture.Slave.Driver.MessageReceived += rocketMessage =>
            {
                if (rocketMessage.RoomId != _fixture.RoomId)
                {
                    return;
                }
                slaveMessage = rocketMessage;
                slaveReceived.Set();
            };

            await _fixture.Master.InitAsync(Constants.OneUsername, Constants.OnePassword);
            await _fixture.Slave.InitAsync(Constants.TwoUsername, Constants.TwoPassword);

            // Act
            var result = await _fixture.Master.Driver.SendMessageAsync(text, _fixture.RoomId);
            result.HasError.Should().BeFalse();

            masterReceived.WaitOne(_timeout);
            slaveReceived.WaitOne(_timeout);

            // Assert
            masterMessage.Should().NotBeNull();
            masterMessage.IsFromMyself.Should().BeTrue();

            slaveMessage.Should().NotBeNull();
            slaveMessage.IsFromMyself.Should().BeFalse();
        }

        [Fact]
        public async Task Load_message_history_loads_past_messages()
        {
            var text = AutoFixture.Create<string>();
            var messageReceived = new AutoResetEvent(false);

            _fixture.Master.Driver.MessageReceived += rocketMessage =>
            {
                if (rocketMessage.RoomId != _fixture.RoomId)
                {
                    return;
                }
                messageReceived.Set();
            };

            await _fixture.Master.InitAsync(Constants.OneUsername, Constants.OnePassword);
            await _fixture.Master.Driver.SendMessageAsync(text, _fixture.RoomId);

            messageReceived.WaitOne(_timeout);

            // Act
            var result = await _fixture.Master.Driver.LoadMessagesAsync(_fixture.RoomId);

            // Assert
            result.Result.Messages.Should().Contain(x => x.Message == text);
        }

        [Fact]
        public async Task Search_message_history_finds_past_messages()
        {
            var text = AutoFixture.Create<string>();
            var messageReceived = new AutoResetEvent(false);

            _fixture.Master.Driver.MessageReceived += rocketMessage =>
            {
                if (rocketMessage.RoomId != _fixture.RoomId)
                {
                    return;
                }
                messageReceived.Set();
            };

            await _fixture.Master.InitAsync(Constants.OneUsername, Constants.OnePassword);
            await _fixture.Master.Driver.SendMessageAsync(text, _fixture.RoomId);

            messageReceived.WaitOne(_timeout);

            // Act
            var result = await _fixture.Master.Driver.SearchMessagesAsync(text, _fixture.RoomId);

            // Assert
            result.Result.Messages.Should().Contain(x => x.Message == text);
        }

        [Fact]
        public async Task Send_attachment()
        {
            var text = AutoFixture.Create<string>();
            Attachment attachment = new Attachment();
            attachment.Text = "Hello world!";
            var result = await _fixture.Fixture.Driver.CreatePrivateMessageAsync("antonio.zhu");
            await _fixture.Fixture.Driver.SendCustomMessageAsync(attachment, result.Result.RoomId);


        }

        [Fact]
        public async Task Send_action_buttons()
        {
            var text = AutoFixture.Create<string>();
            Attachment attachment = new Attachment();
            attachment.Actions = new Action[]
            {
                new Action()
                {
                    Type = "button",
                    MsgInChatWindow = true,
                    Text = "1",
                    Message = "1"
                },
                new Action()
                {
                    Type = "button",
                    MsgInChatWindow = true,
                    Text = "2",
                    Message = "2"
                }
            };
            var result = await _fixture.Fixture.Driver.CreatePrivateMessageAsync("antonio.zhu");
            await _fixture.Fixture.Driver.SendCustomMessageAsync(attachment, result.Result.RoomId);

            
        }

        [Fact]
        public async Task Download_Attachments()
        {
            var text = AutoFixture.Create<string>();
            await _fixture.Master.InitAsync(Constants.OneUsername, Constants.OnePassword);
            await _fixture.Slave.InitAsync(Constants.OneUsername, Constants.OnePassword);

            await _fixture.Slave.Driver.SubscribeToRoomAsync("GENERAL");

            var slaveReceived = new AutoResetEvent(false);
            _fixture.Slave.Driver.MessageReceived += (msg) =>
            {
                if (msg.Attachments != null && msg.Attachments.Count > 0)
                {
                    IEnumerable<string> paths = _fixture.Slave.Driver.GetAttachments(msg).Result;
                }
                slaveReceived.Set();

            };
            var sentMessage = await _fixture.Master.Driver.UploadFileToRoomAsync("GENERAL", "Assets\\random-image.png");
            slaveReceived.WaitOne();
            
        }

        [Fact]
        public async Task Upload_file_to_room()
        { 
            var text = AutoFixture.Create<string>();
            // var result = await _fixture.Fixture.Driver.UploadFileToRoomAsync("general", @"C:\Users\antonio.zhu\repos\sbw\Rocket.Chat.Net\test\Rocket.Chat.Net.Tests\bin\Debug\Assets\random-image.png");
            
            var result = await _fixture.Fixture.Driver.UploadFileToRoomAsync("GENERAL", "./Assets/random-image.png");
            result.Success.Should().BeTrue();

        }

        [Fact]
        public async Task Upload_filestream_to_room()
        {
            var text = AutoFixture.Create<string>();
            using (var fs = System.IO.File.OpenRead(".\\Assets\\random-image.png"))
            {
                var result = await _fixture.Fixture.Driver.UploadFileToRoomAsync("GENERAL", fs);
                result.Success.Should().BeTrue();
            }
        }

        public void Dispose()
        {
            _fixture.Dispose();
        }
    }

    public class MessagingFixture : IDisposable
    {
        private readonly ILogger _logger;
        public RocketChatDriverFixture Fixture { get; }
        public RocketChatDriverFixture Master { get; }
        public RocketChatDriverFixture Slave { get; }

        public string RoomId { get; }
        public string RoomName { get; }

        public MessagingFixture(ITestOutputHelper helper, string roomName)
        {
            RoomName = roomName;
            _logger = NLog.LogManager.GetCurrentClassLogger();
            Master = new RocketChatDriverFixture(_logger);
            Slave = new RocketChatDriverFixture(_logger);

            Fixture = new RocketChatDriverFixture(_logger);
            Fixture.InitAsync(Constants.OneUsername, Constants.OnePassword).Wait();
            RoomId = Fixture.Driver.CreateChannelAsync(roomName)?.Result?.Result?.RoomId;
        }

        public void Dispose()
        {
            Fixture.Driver.EraseRoomAsync(RoomId).Wait();

            Fixture.Dispose();
            Master.Dispose();
            Slave.Dispose();
        }
    }

    public class RocketChatDriverFixture : IDisposable
    {
        public IRocketChatDriver Driver { get; }

        public RocketChatDriverFixture(ILogger helper)
        {
            Driver = new RocketChatDriver(Constants.RocketServer, false, helper);
        }

        public async Task InitAsync(string username, string password)
        {
            await Driver.ConnectAsync();
            await Driver.LoginAsync(new UsernameLoginOption() { Username = username, Password = password });
            await Driver.SubscribeToRoomAsync();
        }

        public void Dispose()
        {
            Driver.Dispose();
        }
    }
}
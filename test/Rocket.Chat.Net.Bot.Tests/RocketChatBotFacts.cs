namespace Rocket.Chat.Net.Bot.Tests
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using FluentAssertions;

    using NSubstitute;
    using NSubstitute.ExceptionExtensions;

    using Ploeh.AutoFixture;

    using Rocket.Chat.Net.Bot.Interfaces;
    using Rocket.Chat.Net.Bot.Models;
    using Rocket.Chat.Net.Interfaces;
    using Rocket.Chat.Net.Models;
    using Rocket.Chat.Net.Models.MethodResults;

    using Xunit;

    public class RocketChatBotFacts
    {
        private static readonly Fixture AutoFixture = new Fixture();

        private readonly ILogger _loggerMock = Substitute.For<ILogger>();
        private readonly IRocketChatDriver _driverMock = Substitute.For<IRocketChatDriver>();

        [Fact]
        public void When_constructed_set_driver()
        {
            // Act
            var bot = new RocketChatBot(_driverMock, _loggerMock);

            // Assert
            bot.Driver.Should().Be(_driverMock);
        }

        [Fact]
        public void When_disposed_dispose_driver()
        {
            // Act
            using (new RocketChatBot(_driverMock, _loggerMock))
            {
            }

            new RocketChatBot(_driverMock, _loggerMock).Dispose();

            // Assert
            _driverMock.Received(2).Dispose();
        }

        [Fact]
        public async Task On_connect_driver_should_connect()
        {
            var bot = new RocketChatBot(_driverMock, _loggerMock);

            // Act
            await bot.ConnectAsync();

            // Assert
            await _driverMock.Received().ConnectAsync();
        }

        [Fact]
        public async Task On_successful_login_set_login_token()
        {
            var loginOption = Substitute.For<ILoginOption>();
            var loginResult = AutoFixture.Build<MethodResult<LoginResult>>()
                                         .With(x => x.Error, null)
                                         .Create();
            _driverMock.LoginAsync(loginOption).Returns(Task.FromResult(loginResult));

            var bot = new RocketChatBot(_driverMock, _loggerMock);

            // Act
            await bot.LoginAsync(loginOption);

            // Assert
            bot.LoginToken.Should().Be(loginResult.Result.Token);
        }

        [Fact]
        public void On_unsuccessful_login_throw()
        {
            var loginOption = Substitute.For<ILoginOption>();
            var loginResult = AutoFixture.Create<MethodResult<LoginResult>>();
            _driverMock.LoginAsync(loginOption).Returns(Task.FromResult(loginResult));

            var bot = new RocketChatBot(_driverMock, _loggerMock);

            // Act
            Action loginAction = () => bot.LoginAsync(loginOption).Wait();

            // Assert
            var exception = loginAction.ShouldThrow<Exception>().And;
            exception.Message.Should().Contain(loginResult.Error.Message);
        }

        [Fact]
        public async Task On_subscribe_subscribe_to_all_rooms()
        {
            var bot = new RocketChatBot(_driverMock, _loggerMock);

            // Act
            await bot.SubscribeAsync();

            // Assert
            await _driverMock.Received().SubscribeToRoomAsync();
        }

        [Fact]
        public void On_resume_throw_if_not_logged_in()
        {
            var bot = new RocketChatBot(_driverMock, _loggerMock);

            // Act
            Action resumeAction = () => bot.ResumeAsync().Wait();

            // Assert
            resumeAction.ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public async Task On_successful_resume_set_login_token()
        {
            var loginOption = Substitute.For<ILoginOption>();
            var loginResult = AutoFixture.Build<MethodResult<LoginResult>>()
                                         .With(x => x.Error, null)
                                         .Create();
            _driverMock.LoginAsync(loginOption).Returns(Task.FromResult(loginResult));

            var resumeResult = AutoFixture.Build<MethodResult<LoginResult>>()
                                          .With(x => x.Error, null)
                                          .Create();
            _driverMock.LoginResumeAsync(loginResult.Result.Token)
                       .Returns(Task.FromResult(resumeResult));

            var bot = new RocketChatBot(_driverMock, _loggerMock);

            // Act
            await bot.LoginAsync(loginOption);
            await bot.ResumeAsync();

            // Assert
            bot.LoginToken.Should().Be(resumeResult.Result.Token);
        }

        [Fact]
        public async Task On_unsuccessful_resume_throw()
        {
            var loginOption = Substitute.For<ILoginOption>();
            var loginResult = AutoFixture.Build<MethodResult<LoginResult>>()
                                         .With(x => x.Error, null)
                                         .Create();
            _driverMock.LoginAsync(loginOption).Returns(Task.FromResult(loginResult));

            var resumeResult = AutoFixture.Build<MethodResult<LoginResult>>()
                                          .Create();
            _driverMock.LoginResumeAsync(loginResult.Result.Token)
                       .Returns(Task.FromResult(resumeResult));

            var bot = new RocketChatBot(_driverMock, _loggerMock);

            // Act
            await bot.LoginAsync(loginOption);
            Action resumeAction = () => bot.ResumeAsync().Wait();

            // Assert
            resumeAction.ShouldThrow<Exception>();
        }

        [Fact]
        public void On_message_receive_process_messages()
        {
            var rocketMessage = AutoFixture.Create<RocketMessage>();

            var waitHandle = new AutoResetEvent(false);
            var responseMock = Substitute.For<IBotResponse>();
            responseMock.CanRespond(Arg.Any<ResponseContext>()).Returns(true);
            responseMock.GetResponse(Arg.Do<ResponseContext>(message => waitHandle.Set()), Arg.Any<RocketChatBot>());

            var bot = new RocketChatBot(_driverMock, _loggerMock);
            bot.AddResponse(responseMock);

            // Act
            _driverMock.MessageReceived += Raise.Event<MessageReceived>(rocketMessage);
            waitHandle.WaitOne(TimeSpan.FromSeconds(5));

            // Assert
            responseMock.Received().GetResponse(Arg.Any<ResponseContext>(), bot);
        }

        [Fact]
        public void When_response_returns_false_dont_run()
        {
            var rocketMessage = AutoFixture.Create<RocketMessage>();

            var responseMock = Substitute.For<IBotResponse>();
            responseMock.CanRespond(Arg.Any<ResponseContext>()).Returns(false);

            var bot = new RocketChatBot(_driverMock, _loggerMock);
            bot.AddResponse(responseMock);

            // Act
            _driverMock.MessageReceived += Raise.Event<MessageReceived>(rocketMessage);
            Thread.Sleep(1 * 1000);

            // Assert
            responseMock.DidNotReceive().GetResponse(Arg.Any<ResponseContext>(), Arg.Any<RocketChatBot>());
        }

        [Fact]
        public void When_response_returns_message_send_message()
        {
            var rocketMessage = AutoFixture.Create<RocketMessage>();
            var basicResponses = AutoFixture.Build<BasicResponse>().CreateMany().ToList();

            var responseMock = Substitute.For<IBotResponse>();
            responseMock.CanRespond(Arg.Any<ResponseContext>()).Returns(true);
            responseMock.GetResponse(Arg.Any<ResponseContext>(), Arg.Any<RocketChatBot>()).Returns(basicResponses);

            var bot = new RocketChatBot(_driverMock, _loggerMock);
            bot.AddResponse(responseMock);

            // Act
            _driverMock.MessageReceived += Raise.Event<MessageReceived>(rocketMessage);
            Thread.Sleep(200);

            // Assert
            _driverMock.Received(basicResponses.Count)
                       .SendMessageAsync(Arg.Is<string>(s => basicResponses.Any(x => x.Message == s)), Arg.Any<string>());
        }

        [Fact]
        public void When_response_returns_attachment_send_attachment()
        {
            var rocketMessage = AutoFixture.Create<RocketMessage>();
            var basicResponses = AutoFixture.Build<AttachmentResponse>().CreateMany().ToList();

            var responseMock = Substitute.For<IBotResponse>();
            responseMock.CanRespond(Arg.Any<ResponseContext>()).Returns(true);
            responseMock.GetResponse(Arg.Any<ResponseContext>(), Arg.Any<RocketChatBot>()).Returns(basicResponses);

            var bot = new RocketChatBot(_driverMock, _loggerMock);
            bot.AddResponse(responseMock);

            // Act
            _driverMock.MessageReceived += Raise.Event<MessageReceived>(rocketMessage);
            Thread.Sleep(200);

            // Assert
            _driverMock.Received(basicResponses.Count)
                       .SendCustomMessageAsync(Arg.Is<Attachment>(s => basicResponses.Any(x => x.Attachment.Equals(s))), Arg.Any<string>());
        }

        [Fact]
        public void When_response_returns_unsupported_response_throw()
        {
            var rocketMessage = AutoFixture.Create<RocketMessage>();
            var basicResponses = AutoFixture.Build<MockMessageResponse>().CreateMany().ToList();

            var responseMock = Substitute.For<IBotResponse>();
            responseMock.CanRespond(Arg.Any<ResponseContext>()).Returns(true);
            responseMock.GetResponse(Arg.Any<ResponseContext>(), Arg.Any<RocketChatBot>()).Returns(basicResponses);

            var bot = new RocketChatBot(_driverMock, _loggerMock);
            bot.AddResponse(responseMock);

            // Act
            _driverMock.MessageReceived += Raise.Event<MessageReceived>(rocketMessage);
            Thread.Sleep(100);

            // Assert
            _driverMock.DidNotReceive().SendMessageAsync(Arg.Any<string>(), Arg.Any<string>());
            _driverMock.DidNotReceive().SendCustomMessageAsync(Arg.Any<Attachment>(), Arg.Any<string>());
        }

        [Fact]
        public void When_response_throws_handle_and_try_next()
        {
            var responseMock1 = Substitute.For<IBotResponse>();
            responseMock1.CanRespond(Arg.Any<ResponseContext>()).Returns(true);
            responseMock1.GetResponse(Arg.Any<ResponseContext>(), Arg.Any<RocketChatBot>())
                         .Throws(new Exception());

            var responseMock2 = Substitute.For<IBotResponse>();
            responseMock2.CanRespond(Arg.Any<ResponseContext>()).Returns(true);
            responseMock2.GetResponse(Arg.Any<ResponseContext>(), Arg.Any<RocketChatBot>());

            var bot = new RocketChatBot(_driverMock, _loggerMock);
            bot.AddResponse(responseMock1);
            bot.AddResponse(responseMock2);

            var rocketMessage = AutoFixture.Create<RocketMessage>();

            // Act
            _driverMock.MessageReceived += Raise.Event<MessageReceived>(rocketMessage);
            Thread.Sleep(200);

            // Assert
            responseMock1.Received().GetResponse(Arg.Any<ResponseContext>(), Arg.Any<RocketChatBot>());
            responseMock2.Received().GetResponse(Arg.Any<ResponseContext>(), Arg.Any<RocketChatBot>());
        }

        [Fact]
        public async Task On_disconnect_and_if_logged_in_resume_old_session()
        {
            var loginOption = Substitute.For<ILoginOption>();
            var loginResult = AutoFixture.Build<MethodResult<LoginResult>>()
                                         .With(x => x.Error, null)
                                         .Create();
            _driverMock.LoginAsync(loginOption).Returns(Task.FromResult(loginResult));

            var resumeResult = AutoFixture.Build<MethodResult<LoginResult>>()
                                          .With(x => x.Error, null)
                                          .Create();
            _driverMock.LoginResumeAsync(loginResult.Result.Token)
                       .Returns(Task.FromResult(resumeResult));

            var bot = new RocketChatBot(_driverMock, _loggerMock);
            await bot.LoginAsync(loginOption);

            // Act
            _driverMock.DdpReconnect += Raise.Event<DdpReconnect>();
            Thread.Sleep(200);

            // Assert
            await _driverMock.Received().LoginResumeAsync(Arg.Any<string>());
        }
    }

    public class MockMessageResponse : IMessageResponse
    {
        public string RoomId { get; set; }
    }
}
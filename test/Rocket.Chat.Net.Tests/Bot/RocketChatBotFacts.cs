namespace Rocket.Chat.Net.Tests.Bot
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using FluentAssertions;

    using NSubstitute;
    using NSubstitute.ExceptionExtensions;

    using Ploeh.AutoFixture;

    using Rocket.Chat.Net.Bot;
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

            // Assert
            _driverMock.Received().Dispose();
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
        public void On_add_response_add_response()
        {
            // Act

            // Assert
        }

        [Fact]
        public void On_message_receive_process_messages()
        {
            var rocketMessage = AutoFixture.Create<RocketMessage>();

            var waitHandle = new AutoResetEvent(false);
            var responseMock = Substitute.For<IBotResponse>();
            responseMock.Response(Arg.Do<RocketMessage>(message => waitHandle.Set()), Arg.Any<RocketChatBot>());

            var bot = new RocketChatBot(_driverMock, _loggerMock);
            bot.AddResponse(responseMock);

            // Act
            _driverMock.MessageReceived += Raise.Event<MessageReceived>(rocketMessage);
            waitHandle.WaitOne(TimeSpan.FromSeconds(5));

            // Assert
            responseMock.Received().Response(rocketMessage, bot);
        }

        [Fact]
        public void When_response_returns_message_send_message()
        {
            var rocketMessage = AutoFixture.Create<RocketMessage>();
            var basicResponses = AutoFixture.CreateMany<BasicResponse>().ToList();
            
            var responseMock = Substitute.For<IBotResponse>();
            responseMock.Response(rocketMessage, Arg.Any<RocketChatBot>())
                        .Returns(basicResponses);

            var bot = new RocketChatBot(_driverMock, _loggerMock);
            bot.AddResponse(responseMock);

            // Act
            _driverMock.MessageReceived += Raise.Event<MessageReceived>(rocketMessage);
            Thread.Sleep(100);

            // Assert
            _driverMock.Received(basicResponses.Count)
                       .SendMessageAsync(Arg.Is<string>(s => basicResponses.Any(x => x.Message == s)), Arg.Any<string>());
        }

        [Fact]
        public void When_response_does_not_response_try_next()
        {
            var responseMock1 = Substitute.For<IBotResponse>();
            responseMock1.Response(Arg.Any<RocketMessage>(), Arg.Any<RocketChatBot>());

            var responseMock2 = Substitute.For<IBotResponse>();
            responseMock2.Response(Arg.Any<RocketMessage>(), Arg.Any<RocketChatBot>());

            var bot = new RocketChatBot(_driverMock, _loggerMock);
            bot.AddResponse(responseMock1);
            bot.AddResponse(responseMock2);

            var rocketMessage = AutoFixture.Create<RocketMessage>();

            // Act
            _driverMock.MessageReceived += Raise.Event<MessageReceived>(rocketMessage);
            Thread.Sleep(100);

            // Assert
            responseMock1.Received().Response(rocketMessage, bot);
            responseMock2.Received().Response(rocketMessage, bot);
        }

        [Fact]
        public void When_response_does_response_break()
        {
            var basicResponses = AutoFixture.CreateMany<BasicResponse>();
            var responseMock1 = Substitute.For<IBotResponse>();
            responseMock1.Response(Arg.Any<RocketMessage>(), Arg.Any<RocketChatBot>())
                         .Returns(basicResponses);

            var responseMock2 = Substitute.For<IBotResponse>();
            responseMock2.Response(Arg.Any<RocketMessage>(), Arg.Any<RocketChatBot>());

            var bot = new RocketChatBot(_driverMock, _loggerMock);
            bot.AddResponse(responseMock1);
            bot.AddResponse(responseMock2);

            var rocketMessage = AutoFixture.Create<RocketMessage>();

            // Act
            _driverMock.MessageReceived += Raise.Event<MessageReceived>(rocketMessage);
            Thread.Sleep(10);

            // Assert
            responseMock1.Received().Response(rocketMessage, bot);
            responseMock2.DidNotReceive().Response(rocketMessage, bot);
        }

        [Fact]
        public void When_response_throws_handle_and_try_next()
        {
            var responseMock1 = Substitute.For<IBotResponse>();
            responseMock1.Response(Arg.Any<RocketMessage>(), Arg.Any<RocketChatBot>())
                         .Throws(new Exception());

            var responseMock2 = Substitute.For<IBotResponse>();
            responseMock2.Response(Arg.Any<RocketMessage>(), Arg.Any<RocketChatBot>());

            var bot = new RocketChatBot(_driverMock, _loggerMock);
            bot.AddResponse(responseMock1);
            bot.AddResponse(responseMock2);

            var rocketMessage = AutoFixture.Create<RocketMessage>();

            // Act
            _driverMock.MessageReceived += Raise.Event<MessageReceived>(rocketMessage);
            Thread.Sleep(100);

            // Assert
            responseMock1.Received().Response(rocketMessage, bot);
            responseMock2.Received().Response(rocketMessage, bot);
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
            Thread.Sleep(10);

            // Assert
            await _driverMock.Received().LoginResumeAsync(Arg.Any<string>());
        }
    }
}
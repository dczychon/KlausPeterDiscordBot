using DiscordBot.Util.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using DiscordBot.Util.Commands.Predefined;
using DiscordBot.Util.Commands.Filter;
using Xunit;
using Moq;
using Discord.WebSocket;
using Discord;
using System.Threading.Tasks;
using Discord.Rest;

namespace DiscordBot.Util.Tests
{
    public class NewMessageHandlerTests
    {
        [Theory]
        [InlineData("!ping", "pong")]
        [InlineData("This is a normal message", null)]  //On a normal message the bot should not respond
        public async Task MessageTriggersExpectedOutput(string inputMessage, string expectedResponse)
        {
            string response = null;

            Mock<IMessage> socketMessageMock = new Mock<IMessage>();
            socketMessageMock
                .SetupGet(m => m.Content)
                .Returns(inputMessage);
            socketMessageMock
                .SetupGet(m => m.Source)
                .Returns(MessageSource.User);
            Mock<IMessageChannel> messageChannelMock = new Mock<IMessageChannel>();
            messageChannelMock
                .Setup(c => c.SendMessageAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<Embed>(), It.IsAny<RequestOptions>()))
                .Callback<string, bool, Embed, RequestOptions>((msg, tts, embed, reqOptions) => response = msg)
                .Returns(() => Task.FromResult<IUserMessage>(default(SocketUserMessage)));
            socketMessageMock
                .Setup(x => x.Channel)
                .Returns(messageChannelMock.Object);

            NewMessageHandler messageHandler = new NewMessageHandler(new ServiceCollection().BuildServiceProvider(), GetPingCommandContainer(), Array.Empty<CommandFilter>());
            await messageHandler.HandleMessage(socketMessageMock.Object);

            messageChannelMock.Verify(c => c.SendMessageAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<Embed>(), It.IsAny<RequestOptions>()), expectedResponse == null ? Times.Never() : Times.Once());
            Assert.Equal(expectedResponse, response);
        }

        private static ICommandContainer GetPingCommandContainer()
        {
            CommandContainer container = new CommandContainer();
            container.AddCommandHandler<PingCommand>();
            return container;
        }
    }
}

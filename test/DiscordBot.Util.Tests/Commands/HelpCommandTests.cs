using Discord;
using DiscordBot.Util.Commands;
using DiscordBot.Util.Commands.Predefined;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace DiscordBot.Util.Tests.Commands
{
    public class HelpCommandTests
    {
        [Fact]
        public async Task RendersHelpForAllCommandInCommandContainerTest()
        {
            string expectedOutput = @":information_source: __**Here are all supported commands:**__ :information_source:

**!help** 	- Prints usage for a command
**!ping** 	- Execute this to get a super special response
";

            ICommandContainer container = new CommandContainer();
            container.AddCommandHandler<PingCommand>();
            container.AddCommandHandler<HelpCommand>();

            string respone = null;

            Mock<IMessage> socketMessageMock = new Mock<IMessage>();
            Mock<IMessageChannel> messageChannelMock = new Mock<IMessageChannel>();
            messageChannelMock
                .Setup(c => c.SendMessageAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<Embed>(), It.IsAny<RequestOptions>()))
                .Callback<string, bool, Embed, RequestOptions>((msg, tts, embed, reqOptions) => respone = msg)
                .Returns(() => Task.FromResult<IUserMessage>(default(SocketUserMessage)));
            socketMessageMock
                .Setup(x => x.Channel)
                .Returns(messageChannelMock.Object);

            HelpCommand helpCommand = new HelpCommand(container);

            //Act
            await helpCommand.Execute(socketMessageMock.Object);

            //Assert
            messageChannelMock.Verify(c => c.SendMessageAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<Embed>(), It.IsAny<RequestOptions>()), Times.Once());
            Assert.Equal(expectedOutput, respone);
        }

        [Fact]
        public async Task ShowErrorIfHelpForUnknownCommandIsRequested()
        {
            string expectedOutput = "\"foo\" is an unknown command. Type !help to list all available commands!";

            ICommandContainer container = new CommandContainer();
            container.AddCommandHandler<PingCommand>();

            string respone = null;

            //TODO Write a helper method to create this mock, because we use the same code in a lot of tests to create this basic mock
            Mock<IMessage> socketMessageMock = new Mock<IMessage>();
            Mock<IMessageChannel> messageChannelMock = new Mock<IMessageChannel>();
            messageChannelMock
                .Setup(c => c.SendMessageAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<Embed>(), It.IsAny<RequestOptions>()))
                .Callback<string, bool, Embed, RequestOptions>((msg, tts, embed, reqOptions) => respone = msg)
                .Returns(() => Task.FromResult<IUserMessage>(default(SocketUserMessage)));
            socketMessageMock
                .Setup(x => x.Channel)
                .Returns(messageChannelMock.Object);

            HelpCommand helpCommand = new HelpCommand(container);
            helpCommand.Command = "foo";

            //Act
            await helpCommand.Execute(socketMessageMock.Object);

            //Assert
            messageChannelMock.Verify(c => c.SendMessageAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<Embed>(), It.IsAny<RequestOptions>()), Times.Once());
            Assert.Equal(expectedOutput, respone);
        }
    }
}

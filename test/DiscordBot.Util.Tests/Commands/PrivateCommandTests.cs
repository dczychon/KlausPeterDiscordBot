using Discord;
using DiscordBot.Util.Commands.Predefined;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DiscordBot.Util.Tests.Commands
{
    public class PrivateCommandTests
    {
        [Fact]
        public async Task CreatesPrivateChannel()
        {
            Mock<IDMChannel> dmChannelMock = new Mock<IDMChannel>();

            Mock<IUser> userMock = new Mock<IUser>();
            userMock
                .Setup(u => u.GetOrCreateDMChannelAsync(It.IsAny<RequestOptions>()))
                .Returns(() => Task.FromResult(dmChannelMock.Object));

            Mock<IMessageChannel> channelMock = new Mock<IMessageChannel>();
            Mock<IMessage> messageMock = new Mock<IMessage>();
            messageMock
                .SetupGet(m => m.Channel)
                .Returns(channelMock.Object);
            messageMock
                .SetupGet(m => m.Author)
                .Returns(userMock.Object);

            //Act
            PrivateCommand command = new PrivateCommand();
            await command.Execute(messageMock.Object);

            //Assert
            dmChannelMock.Verify(u => u.SendMessageAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<Embed>(), It.IsAny<RequestOptions>()), Times.AtLeastOnce());
        }
    }
}

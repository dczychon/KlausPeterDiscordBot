using Discord;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace DiscordBot.Util.Tests
{
    public class ExtensionMethodsTests
    {
        [Theory]
        [InlineData("test", "test")]
        [InlineData("null", "null")]
        [InlineData(null, "null")]
        [InlineData("", "null")]
        [InlineData("    ", "null")]
        public void ToNullStringTest(string input, string expected)
        {
            Assert.Equal(expected, input.ToNullString());
        }

        [Theory]
        [MemberData(nameof(GetTestDataForIsPrivateMessageTest))]
        public void IsPrivateMessageTest(IMessage message, bool isPrivate)
        {
            Assert.Equal(isPrivate, message.IsPrivateMessage());
        }

        public static IEnumerable<object[]> GetTestDataForIsPrivateMessageTest()
        {
            yield return new object[] { BuildMessageMock("peter", false).Object, false };
            yield return new object[] { BuildMessageMock("peter", true).Object, true };
            yield return new object[] { BuildMessageMock("klaus", false).Object, false };
        }

        private static Mock<IMessage> BuildMessageMock(string username, bool privateMessage)
        {
            string userDiscriminator = GenerateRandomDiscriminator(username.Length).ToString();    //Generates a 4 digit random number with the username as seed

            Mock<IMessageChannel> channelMock = new Mock<IMessageChannel>();
            if (privateMessage)
            {
                channelMock.SetupGet(c => c.Name).Returns($"@{username}#{userDiscriminator}");
            }
            else
            {
                channelMock.SetupGet(c => c.Name).Returns($"somechannel");
            }
            
            Mock<IUser> userMock = new Mock<IUser>();
            userMock.SetupGet(u => u.Username).Returns(username);
            userMock.SetupGet(u => u.Discriminator).Returns(userDiscriminator);

            Mock<IMessage> messageMock = new Mock<IMessage>();
            messageMock.SetupGet(m => m.Channel).Returns(channelMock.Object);
            messageMock.SetupGet(m => m.Author).Returns(userMock.Object);

            return messageMock;
        }

        private static int GenerateRandomDiscriminator(int seed)
        {
            return new Random(seed).Next(1000, 10000);
        }
    }
}

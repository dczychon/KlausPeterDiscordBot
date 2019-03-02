using Discord;
using DiscordBot.Util.Commands;
using DiscordBot.Util.Commands.Attributes;
using DiscordBot.Util.Commands.Filter;
using DiscordBot.Util.RuntimeDebugging;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DiscordBot.Util.Tests
{
    public class CommandFilterTests
    {
        [Theory]
        [MemberData(nameof(GetTestDataForCommandFilterRestrictsExecutionTest))]
        public void CommandFilterRestrictsExecutionTest(IMessage message, CommandFilter filter, BotConfiguration configuration, bool canExecute)
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton(configuration);

            Assert.Equal(canExecute, filter.CanExecuteInContext(message, services.BuildServiceProvider()));
        }

        public static IEnumerable<object[]> GetTestDataForCommandFilterRestrictsExecutionTest()
        {
            //RestrictToBotAdmin tests
            yield return new object[] { BuildMessageMock("test", 1234, 0000).Object, new RestrictToBotAdminAttribute(), new BotConfiguration { BotAdministrators = new ulong[] { 4444, 3333 } }, false };
            yield return new object[] { BuildMessageMock("test", 1234, 0000).Object, new RestrictToBotAdminAttribute(), new BotConfiguration { BotAdministrators = new ulong[] { 1234, 4321 } }, true };

            //RestrictToChannel tests
            yield return new object[] { BuildMessageMock("test", 1234, 0000).Object, new RestrictToChannelAttribute(1111), new BotConfiguration(), false };
            yield return new object[] { BuildMessageMock("test", 1234, 0000).Object, new RestrictToChannelAttribute(0000), new BotConfiguration(), true };
        }

        [Theory]
        [MemberData(nameof(GetTestDataForCommandFilterCheckerDenyScopeTest))]
        internal void CommandFilterCheckerDenyScopeTest(IMessage message, IEnumerable<CommandFilter> globalFilter, FilterScope? expectedDenyScope)
        {
            CommandFilterChecker checker = new CommandFilterChecker(globalFilter);
            if (checker.ExecutionAllowed(typeof(SecretOnlyAdminCommand), message, new ServiceCollection().BuildServiceProvider(), out FilterScope? scope))
            {
                Assert.Null(expectedDenyScope);
            }
            else
            {
                Assert.Equal(expectedDenyScope, scope);
            }
        }

        public static IEnumerable<object[]> GetTestDataForCommandFilterCheckerDenyScopeTest()
        {
            yield return new object[] { BuildMessageMock("secret test message", 1234, 5678).Object, new CommandFilter[] { new RestrictToChannelAttribute(5678) }, null };
            yield return new object[] { BuildMessageMock("secret test message", 1234, 1111).Object, new CommandFilter[] { new RestrictToChannelAttribute(5678) }, FilterScope.Global };
            yield return new object[] { BuildMessageMock("secret test message", 2222, 1111).Object, new CommandFilter[] { new RestrictToChannelAttribute(5678) }, FilterScope.Global };
            yield return new object[] { BuildMessageMock("secret test message", 2222, 1111).Object, Array.Empty<CommandFilter>(), FilterScope.Command };
        }

        private static Mock<IMessage> BuildMessageMock(string content, ulong authorId, ulong channelId)
        {
            Mock<IMessageChannel> channelMock = new Mock<IMessageChannel>();
            channelMock.SetupGet(c => c.Id).Returns(channelId);

            Mock<IUser> userMock = new Mock<IUser>();
            userMock.SetupGet(u => u.Id).Returns(authorId);

            Mock<IMessage> messageMock = new Mock<IMessage>();
            messageMock.SetupGet(m => m.Content).Returns(content);
            messageMock.SetupGet(m => m.Channel).Returns(channelMock.Object);
            messageMock.SetupGet(m => m.Author).Returns(userMock.Object);

            return messageMock;
        }
    }

    [CommandVerb("onlyadmin")]
#pragma warning disable CS0618 // Typ oder Element ist veraltet
    [RestrictToUser(1234)]
#pragma warning restore CS0618 // Typ oder Element ist veraltet
    class SecretOnlyAdminCommand : ICommand
    {
        public Task Execute(IMessage rawMessage)
        {
            throw new NotImplementedException();
        }
    }
}

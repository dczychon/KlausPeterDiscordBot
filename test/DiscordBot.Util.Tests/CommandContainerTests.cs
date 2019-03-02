using Discord;
using DiscordBot.Util.Commands;
using DiscordBot.Util.Commands.Attributes;
using DiscordBot.Util.Commands.Predefined;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DiscordBot.Util.Tests
{
    public class CommandContainerTests
    {
        [Theory]
        [MemberData(nameof(GetTestDataForGetCommandTypeForMessageInputTest))]
        public void GetCommandTypeForMessageInputTest(IEnumerable<Type> commands, string msgCommand, Type expectedCommandHandler)
        {
            CommandContainer container = new CommandContainer();
            foreach (Type command in commands)
            {
                container.AddCommandHandler(command);
            }

            if (container.TryGetCommandType(msgCommand, out Type commandHandler))
            {
                Assert.Equal(expectedCommandHandler, commandHandler);
            }
            else
            {
                Assert.Null(expectedCommandHandler);
            }
        }

        public static IEnumerable<object[]> GetTestDataForGetCommandTypeForMessageInputTest()
        {
            yield return new object[] { new Type[] { typeof(PingCommand), typeof(HelpCommand) }, "ping", typeof(PingCommand) };
            yield return new object[] { new Type[] { typeof(PingCommand), typeof(HelpCommand) }, "help", typeof(HelpCommand) };
            yield return new object[] { new Type[] { typeof(HelpCommand) }, "help", typeof(HelpCommand) };
            yield return new object[] { new Type[] { typeof(PingCommand), typeof(HelpCommand) }, "null", null };
        }

        [Theory]
        [InlineData(typeof(PingCommand), true)]
        [InlineData(typeof(HelpCommand), true)]
        [InlineData(typeof(CommandWithoutVerbCommand), false)]  //Here the CommandVerb Attribute is missing
        [InlineData(typeof(VerbWithoutCommand), false)]         //Here the ICommand Interface implementation is missing
        [InlineData(typeof(String), false)]                     //Here everything is missing
        public void OnlyAllowValidTypesToBeAddedToCommandContainerTest(Type typeToAdd, bool isValidType)
        {
            CommandContainer container = new CommandContainer();

            if (isValidType)
            {
                container.AddCommandHandler(typeToAdd);
                Assert.Single(container);   //Verify command was added
            }
            else
            {
                Assert.Throws<Exception>(() => container.AddCommandHandler(typeToAdd));
                Assert.Empty(container);    //Verify type was not added
            }
        }
    }

    class CommandWithoutVerbCommand : ICommand
    {
        public Task Execute(IMessage rawMessage)
        {
            throw new NotImplementedException();
        }
    }

    [CommandVerb("foo")]
    class VerbWithoutCommand
    {

    }
}

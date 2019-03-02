using Discord;
using DiscordBot.Util.Commands;
using DiscordBot.Util.Commands.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DiscordBot.Util.Tests
{
    public class ArgumentsMapperTests
    {
        [Theory]
        [MemberData(nameof(GetTestDataForMapArgumentsToCommandInstanceTest))]
        internal void MapArgumentsToCommandInstanceTest(string[] args, ArgumenTestCommand expected)
        {
            ArgumentsMapper mapper = new ArgumentsMapper(args, new ArgumenTestCommand());

            if (expected != null)
            {
                Assert.Equal(expected, (ArgumenTestCommand)mapper.MapToInstance());
            }
            else
            {
                Assert.Throws<CommandArgumentException>(() => mapper.MapToInstance());
            }
        }

        public static IEnumerable<object[]> GetTestDataForMapArgumentsToCommandInstanceTest()
        {
            yield return new object[] { new string[] { "Peter", "Test", "Hallo" }, new ArgumenTestCommand { Name = "Peter", SomeText = "Test", Foo = "Hallo" } };
            yield return new object[] { new string[] { "Peter", "Test" }, new ArgumenTestCommand { Name = "Peter", SomeText = "Test", Foo = "Bar" } };
            yield return new object[] { new string[] { "Peter" }, new ArgumenTestCommand { Name = "Peter", SomeText = null, Foo = "Bar" } };
            yield return new object[] { Array.Empty<string>(), null };
        }
    }

    [CommandVerb("attrtest")]
    class ArgumenTestCommand : ICommand, IEquatable<ArgumenTestCommand>
    {
        [CommandArgument(0, nameof(Name), Required = true)]
        public string Name { get; set; }

        [CommandArgument(1, nameof(SomeText))]
        public string SomeText { get; set; }

        [CommandArgument(2, nameof(Foo), DefaultValue = "Bar")]
        public string Foo { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as ArgumenTestCommand);
        }

        public bool Equals(ArgumenTestCommand other)
        {
            return other != null &&
                   Name == other.Name &&
                   SomeText == other.SomeText;
        }

        public Task Execute(IMessage rawMessage)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, SomeText);
        }

        public static bool operator ==(ArgumenTestCommand left, ArgumenTestCommand right)
        {
            return EqualityComparer<ArgumenTestCommand>.Default.Equals(left, right);
        }

        public static bool operator !=(ArgumenTestCommand left, ArgumenTestCommand right)
        {
            return !(left == right);
        }
    }
}

using DiscordBot.Util.Commands;
using DiscordBot.Util.Commands.Predefined;
using DiscordBot.Util.RuntimeDebugging;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace DiscordBot.Util.Tests
{
    public class HelpTextRendererTests
    {
        [Theory]
        [InlineData(typeof(PingCommand), "Syntax: **!ping**")]
        [InlineData(typeof(HelpCommand), "Syntax: **!help** (Command name)")]
        [InlineData(typeof(InspectDscCommand), "Syntax: **!inspect-dsc** [Path]")]
        public void RenderSyntaxHelpTest(Type inputType, string expectedSyntaxHelp)
        {
            HelpTextRenderer helpTextRenderer = new HelpTextRenderer(inputType);
            Assert.Equal(expectedSyntaxHelp, helpTextRenderer.GenerateSyntaxHelp().ToString());
        }

        [Theory]
        [InlineData(typeof(PingCommand), "Syntax: **!ping**")]
        [InlineData(typeof(HelpCommand), "Syntax: **!help** (Command name)")]
        [InlineData(typeof(InspectDscCommand), "Syntax: **!inspect-dsc** [Path]")]
        public void RenderArgumentsHelp(Type inputType, string expectedArgumentsHelp)
        {
            HelpTextRenderer helpTextRenderer = new HelpTextRenderer(inputType);
            Assert.Equal(expectedArgumentsHelp, helpTextRenderer.GenerateArgumentsHelp().ToString());
        }
    }
}

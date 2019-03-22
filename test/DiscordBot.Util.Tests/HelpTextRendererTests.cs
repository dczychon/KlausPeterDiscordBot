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
        [InlineData(typeof(PingCommand), "**!ping** \t- Execute this to get a super special response")]
        [InlineData(typeof(HelpCommand), "**!help** \t- Prints usage for a command")]
        [InlineData(typeof(InspectDscCommand), "**!inspect-dsc** \t- Allows the inspection of the DiscordSocketClient at runtime")]
        public void RenderHeaderHelpTest(Type inputType, string expectedHeader)
        {
            HelpTextRenderer helpTextRenderer = new HelpTextRenderer(inputType);
            Assert.Equal(expectedHeader, helpTextRenderer.GenerateHeaderHelp().ToString());
        }

        [Fact]
        public void DontRenderHelpForTypesWithoutCommandVerbAttribute()
        {
            Assert.Throws<ArgumentException>(()=> new HelpTextRenderer(typeof(String)).GetHelp());
        }
    }
}

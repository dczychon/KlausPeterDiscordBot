using DiscordBot.Util.Commands;
using DiscordBot.Util.Commands.Predefined;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace DiscordBot.Util.Tests
{
    public class HelpTextRendererTests
    {
        [Theory]
        [InlineData(typeof(PingCommand), "some text")]
        public void RenderSyntaxHelpTest(Type inputType, string expectedSyntaxHelp)
        {
            HelpTextRenderer helpTextRenderer = new HelpTextRenderer(inputType);
            Assert.Equal(expectedSyntaxHelp, helpTextRenderer.GenerateSyntaxHelp().ToString());
        }
    }
}

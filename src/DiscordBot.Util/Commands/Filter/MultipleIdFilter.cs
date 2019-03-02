using System.Collections.Generic;

namespace DiscordBot.Util.Commands.Filter
{
    public abstract class MultipleIdFilter : CommandFilter
    {
        protected readonly IEnumerable<ulong> _ids;

        protected MultipleIdFilter(IEnumerable<ulong> ids)
        {
            _ids = ids;
        }
    }
}
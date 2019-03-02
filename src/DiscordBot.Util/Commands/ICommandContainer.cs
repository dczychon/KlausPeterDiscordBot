using System;
using System.Collections.Generic;

namespace DiscordBot.Util.Commands
{
    public interface ICommandContainer : IEnumerable<Type>
    {
        void AddCommandHandler<T>() where T : ICommand;

        void AddCommandHandler(Type commandType);

        bool TryGetCommandType(string command, out Type type);
    }
}
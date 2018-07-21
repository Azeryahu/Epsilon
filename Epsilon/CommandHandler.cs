using System.Reflection;
using System.Threading.Tasks;
using System.Linq;
using System;
using Discord.WebSocket;
using Discord.Commands;

namespace Epsilon
{
    public class CommandHandler
    {
        private DiscordSocketClient _client;
        private CommandService _responsesService;
        private CommandService _helpService;
        private CommandService _marketService;
        public CommandHandler(DiscordSocketClient client)
        {
            _client = client;
            _responsesService = new CommandService();
            _responsesService.AddModuleAsync(typeof(Modules.Responses));
            _helpService = new CommandService();
            _helpService.AddModuleAsync(typeof(Modules.Help));
            _marketService = new CommandService();
            _marketService.AddModuleAsync(typeof(Modules.Market));
            _client.MessageReceived += HandleCommandAsync;
        }
        private async Task HandleCommandAsync(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            if (msg == null) return;
            var context = new SocketCommandContext(_client, msg);
            int argPos = 0;
            CommandService _service = null;
            string errText = string.Empty;
            if (msg.HasCharPrefix('~', ref argPos))
            {
                _service = _responsesService;
                errText = "Something has gone terribly wrong.  ";
            }
            else if (msg.HasCharPrefix('?', ref argPos))
            {
                _service = _helpService;
                errText = "Something has gone terribly wrong with our help.  ";
            }
            else if (msg.HasCharPrefix('$', ref argPos))
            {
                _service = _marketService;
                errText = "Something has gone terribly wrong with our help.  ";
            }
            if (_service != null)
            {
                var result = await _service.ExecuteAsync(context, argPos);
                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                {
                    await context.Channel.SendMessageAsync(errText + result.ErrorReason);
                }
            }
        }
    }
}

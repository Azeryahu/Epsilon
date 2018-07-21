using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using Discord.WebSocket;

namespace Epsilon.Modules
{
    public class Market : ModuleBase<SocketCommandContext>
    {
        [Command("")]
        private async Task SendLogMessage(SocketGuildUser userID, string commandName)
        {
            var channel = Context.Guild.GetTextChannel(400816839967375361);
            await channel.SendMessageAsync("```" + userID.ToString() + " used the command " + commandName + " at " + DateTimeOffset.UtcNow + " UTC. ```");
        }

    }
}

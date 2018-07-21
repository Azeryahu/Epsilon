using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace Epsilon
{
    public class ConnectedEventHandler : EventHandler
    {
        public ConnectedEventHandler(DiscordSocketClient client) : base(client)
        {
            base._client.Connected += _client_Connected;
        }

        private Task _client_Connected()
        {
            return Task.Run(() =>
            {
                Console.WriteLine("Bot is connected.");
            });
        }
    }
}

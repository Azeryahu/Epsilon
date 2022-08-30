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
            base._client.Ready += _client_Ready;
        }
        private Task _client_Ready()
        {
            return Task.Run(() =>
            {
                var announceChannel = _client.GetChannel(Epsilon.ConfigFile.AnnouncementChannelID) as ISocketMessageChannel;
                Console.WriteLine("Epsilon has connected to the server.");
            });
        }
    }
}

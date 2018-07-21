using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace Epsilon
{
    public class EventHandler
    {
        protected DiscordSocketClient _client;
        public string UserID { get; set; }
        public EventHandler(DiscordSocketClient client)
        {
            _client = client;
        }
    }
}

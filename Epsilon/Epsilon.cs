using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.Common;
using System.IO;
using Discord;
using Discord.WebSocket;
using System;

namespace Epsilon
{
    class Epsilon
    {
        static void Main(string[] args)
        {
            StartAsync();
            Console.ReadLine();
        }
        private static DiscordSocketClient _client;
        private static CommandHandler _handler;
        public async static Task StartAsync()
        {
            _client = new DiscordSocketClient();
            string token = "";
            try
            {
                StreamReader TokenReader = new StreamReader("EpsilonBotToken.txt");
                token = TokenReader.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception:  " + e.Message);
            }
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
            _handler = new CommandHandler(_client);
            await Task.Delay(-1);

        }

    }
}

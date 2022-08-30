using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using System.IO;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace Epsilon
{
    class Epsilon
    {
        private static DiscordSocketClient _client;
        private static CommandHandler _handler;
        public static Config ConfigFile;
        public static Dictionary<string, string> RanksDictionary; 
        public static List<string> StandingRolesList;
        public static DateTimeOffset LastCheck = DateTimeOffset.UtcNow.AddDays(-7);
        public static TimeSpan WaitTime = TimeSpan.FromSeconds(0);
        static void Main(string[] args)
        {
            new Epsilon().MainAsync().GetAwaiter().GetResult();
        }
        public Epsilon()
        {
            _client = new DiscordSocketClient();
            _handler = new CommandHandler(_client);
            var userJoinedHandler = new UserJoinedHandler(_client);
        }
        public async Task MainAsync()
        {
            string token = "";
            var connectedEvent = new ConnectedEventHandler(_client);
            try
            {
                StreamReader configReader = new StreamReader("config\\config.json");
                string configString = configReader.ReadToEnd();
                ConfigFile = JsonConvert.DeserializeObject<Config>(configString);
                token = ConfigFile.Token;
                StandingRolesList = ConfigFile.StandingRolesList;
                RanksDictionary = ConfigFile.RanksDictionary;
                configReader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception:  " + e.Message);
            }
            await _client.LoginAsync(Discord.TokenType.Bot, token);
            await _client.StartAsync();
            await Task.Delay(-1);
        }
    }
}

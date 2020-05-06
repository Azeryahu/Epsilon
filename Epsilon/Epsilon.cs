using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using System.IO;

namespace Epsilon
{
    class Epsilon
    {
        private static DiscordSocketClient _client;
        private static CommandHandler _handler;
        public static int TimeoutTimeLimit = 180;
        public static ulong BotSpamChannelID = 468054497244217344;
        public static ulong AnnounceChannelID = 648990637596737536;
        public static ulong SecureChannelID = 648990541161299989;
        public static ulong LogChannelID = 648990953402794012;
        public static ulong LambertID = 151043323379843073;
        public static ulong AdminID = 622595673665044481;
        public static string OrganizationName = "Raiders of the Lost Sector";
        public static Dictionary<string, string> RanksDictionary = new Dictionary<string, string> { {"C00", "Trainee" },{"C01", "Student" },{"C02", "Beginner" },
            {"C03", "Apprentice" },{"C04", "Assistant" },{"C05", "Practitioner" },{"C06", "Associate" },{"C07", "Journyman" },{"C08", "Trainer" },
            {"C09", "Expert" },{"C10", "Mentor" },{"C11", "Master" },{"C12", "Elite" },{"M01", "Team Leader" },{"M02", "Group Leader" },
            {"M03", "Moderator" },{"M04", "Supervisor" },{"M05", "Administrator" },{"M06", "Vice President" },{"M07", "Senior Vice President" },
            {"M08", "Executive Vice President" },{"M09", "Deputy President" },{"M10", "President" },{"M11", "Director" },{"M12", "Director General" },
            {"E00", "Recruit" },{"E01", "Private" },{"E02", "Private First Class" },{"E03", "Lance Corporal" },{"E04", "Corporal" },{"E05", "Sergeant" },
            {"E06", "Staff Sergeant" },{"E07", "Gunnery Sergeant" },{"E08", "Master Sergeant" },{"E09", "First Sergeant" },{"E10", "Master Gunnery Sergeant" },
            {"E11", "Sergeant Major" },{"E12", "Sergeant Major of the Corps" },{"O01", "Second Lieutenant" },{"O02", "First Lieutenant" },{"O03", "Captain" },
            {"O04", "Major" },{"O05", "Lieutenant Colonel" },{"O06", "Colonel" },{"O07", "Brigadier General" },{"O08", "Major General" },
            {"O09", "Lieutenant General" },{"O10", "General" },{"O11", "Commander" },{"O12", "Admiral" }
        };
        public static List<string> StandingRolesList = new List<string> { "trusted", "friendly", "neutral", "suspicious", "criminal" };
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
                StreamReader TokenReader = new StreamReader("EpsilonBotToken.txt");
                token = TokenReader.ReadLine();
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

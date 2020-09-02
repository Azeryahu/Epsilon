using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Addons.Interactive;
using System.IO;

namespace Epsilon.Modules
{
    class HelpCommands : InteractiveBase<SocketCommandContext>
    {
        private List<string> GameNamesList = new List<string>();
        private string versionNumber = "V 3.0.3";
        [Command("Help")]
        public async Task help()
        {
            var user = Context.User as SocketGuildUser;
            if (user == null) return;
            if (user.Id == Epsilon.MasterID || user.Roles.Any(x => x.Name == "Administrator"))
            {
                try
                {
                    var channel = Context.Guild.GetTextChannel(Epsilon.SecureChannelID);
                    await ReplyAsync("Here is a list of available commands for you (no capitalization required):\n" +
                        "\"~Get Access(Game Name)\" use ?Game Names for a list of titles we currently have channels for\n" +
                        "\"~Check Standing(username)\", username is optional\n" +
                        "\"~Check Warnings(username)\", username is optional\n" +
                        "\"~Check Attempts(username)\", username is optional\n" +
                        "\"~Enlist\" - verification required.\n" +
                        "\"~Join\" - verification required.\n" +
                        "Here is a list of the other help commands:\n" +
                        "\"?Command Help(command)\", command not optional\n" +
                        "\"?About\"\n" +
                        "\"?Patch Notes\"");
                    await channel.SendMessageAsync(Context.Message.Author.Mention + ".  Here is a list of available admin commands for you:\n" +
                        "\"-Update Database\" - to update the database with the current members of the server.\n" +
                        "\"-Add Standing(int, username)\" - adds the integer amount of standing to username entered in %.\n" +
                        "\"-Remove (or take) Standing(int, username)\" - opposite of the 'Add Standing' command.\n" +
                        "\"-Issue Warning(username, reason)\" - issues a warning to the username for the reason provided.\n" +
                        "\"-Set Standing(int, username)\" - sets the standing of the username entered to the integer entered.  (greater than or equal to -10, or less than or equal to 10)\n" +
                        "\"-Enable Join(username)\" - allows the username entered to join the faction.\n" +
                        "Here is a list of basic user commands:\n" +
                        "\"~Check Standing(username)\" - checks the standing of username provided.  Leave blank to check own standing.\n" +
                        "\"~Check Warnings(username)\" - checks the amount of warnings of the username provided.  Leave blank to check own warning amount.\n" +
                        "\"~Check Attempts(username)\" - checks the amount of attempts of the username provided.  Leave blank to check own attempt amount.\n" +
                        "Here is a list of the other help commands:\n" +
                        "\"?Command Help(command)\" - provides further details on the specific command provided.\n" +
                        "\"?About\" - provides information about my programming.\n" +
                        "\"?Patch Notes\" - provides a list of fixes and updates.");
                }
                catch (Exception e)
                {
                    await ReplyAsync("I am sorry.  I cannot provide you a list of commands available to you in this unsecure channel.\n" +
                        e.Message);
                }
            }
            else
            {
                await ReplyAsync("Here is a list of available commands for you (no capitalization required):\n" +
                    "\"~Get Access(Game Name)\", full game name\n" +
                    "\"~Check Standing(username)\", username is optional\n" +
                    "\"~Check Warnings(username)\", username is optional\n" +
                    "\"~Check Attempts(username)\", username is optional\n" +
                    "\"~Join\" - verification required.\n" +
                    "Here is a list of the other help commands:\n" +
                    "\"?Command Help(command)\", command not optional\n" +
                    "\"?About\"\n" +
                    "\"?Patch Notes\"");
            }
            ResetAttempts(user);
            await SendLogMessage(user, "Help");
        }
        [Command("Command Help")]
        public async Task commandHelp([Remainder] string command)
        {
            var user = Context.User as SocketGuildUser;
            if (user == null) return;
            if (user.Roles.Any(x => x.Name == "Administrator") || user.Id == 151043323379843073)
            {
                if (command.ToUpper() == "Ark".ToUpper() || command.ToUpper() == "Ark: Survival Evolved".ToUpper() || command.ToUpper() == "Ark Survival Evolved".ToUpper())
                {
                    await ReplyAsync("This command provides access to the Ark: Survival Evolved channels which include a text and voice channel.");
                }
                else if (command.ToUpper() == "Arma".ToUpper() || command.ToUpper() == "Arma3".ToUpper() || command.ToUpper() == "Arma 3".ToUpper())
                {
                    await ReplyAsync("This command provides access to the Arma 3 channels which include a text and voice channel.");
                }
                else if (command.ToUpper() == "Avorion".ToUpper())
                {
                    await ReplyAsync("This command provides access to the Avorion channels which include a text and voice channel.");
                }
                else if (command.ToUpper() == "DCS".ToUpper() || command.ToUpper() == "DCS World".ToUpper())
                {
                    await ReplyAsync("This command provides access to the DCS World channels which include a text and voice channel.");
                }
                else if (command.ToUpper() == "Dual Universe".ToUpper())
                {
                    await ReplyAsync("This command provides access to the Dual Universe channels which includes a text for NDA and non-NDA and voice channels for NDA and non-NDA.");
                }
                else if (command.ToUpper() == "EVE Online".ToUpper() || command.ToUpper() == "EVE".ToUpper())
                {
                    await ReplyAsync("This command provides access to the EVE Online channels which include a text and voice channel.");
                }
                else if (command.ToUpper() == "No Man's Sky".ToUpper() || command.ToUpper() == "NMS".ToUpper() || command.ToUpper() == "No Mans Sky".ToUpper())
                {
                    await ReplyAsync("This command provides access to the No Man's Sky channels which include a text and voice channel.");
                }
                else if (command.ToUpper() == "Space Engineers".ToUpper())
                {
                    await ReplyAsync("This command provides access to the Space Engineers channels with include a text and voice channel.");
                }
                else if (command.ToUpper() == "Warhammer 40K".ToUpper() || command.ToUpper() == "Warhammer".ToUpper())
                {
                    await ReplyAsync("This command provieds access to the Warhammer 40K channels which include a text and voice channel.");
                }
                else if (command.ToUpper() == "Check Standing".ToUpper())
                {
                    await ReplyAsync("This command has an optional field where you can type the username or mention the person of whom you wnat to know their standing.  " +
                        "If you leave this field blank, this command will return your own standing with The Raiders of the Lost Sector.");
                }
                else if (command.ToUpper() == "Set Standing".ToUpper())
                {
                    await ReplyAsync("This command takes two arguments:  float of the new standing, [Remainder] user to which standings will affect.");
                }
            }
            else
            {
                if (command.ToUpper() == "Ark".ToUpper() || command.ToUpper() == "Ark: Survival Evolved".ToUpper() || command.ToUpper() == "Ark Survival Evolved".ToUpper())
                {
                    await ReplyAsync("This command provides access to the Ark: Survival Evolved channels which include a text and voice channel.");
                }
                else if (command.ToUpper() == "Arma".ToUpper() || command.ToUpper() == "Arma3".ToUpper() || command.ToUpper() == "Arma 3".ToUpper())
                {
                    await ReplyAsync("This command provides access to the Arma 3 channels which include a text and voice channel.");
                }
                else if (command.ToUpper() == "Avorion".ToUpper())
                {
                    await ReplyAsync("This command provides access to the Avorion channels which include a text and voice channel.");
                }
                else if (command.ToUpper() == "DCS".ToUpper() || command.ToUpper() == "DCS World".ToUpper())
                {
                    await ReplyAsync("This command provides access to the DCS World channels which include a text and voice channel.");
                }
                else if (command.ToUpper() == "Dual Universe".ToUpper())
                {
                    await ReplyAsync("This command provides access to the Dual Universe channels which includes a text for NDA and non-NDA and voice channels for NDA and non-NDA.");
                }
                else if (command.ToUpper() == "EVE Online".ToUpper() || command.ToUpper() == "EVE".ToUpper())
                {
                    await ReplyAsync("This command provides access to the EVE Online channels which include a text and voice channel.");
                }
                else if (command.ToUpper() == "No Man's Sky".ToUpper() || command.ToUpper() == "NMS".ToUpper() || command.ToUpper() == "No Mans Sky".ToUpper())
                {
                    await ReplyAsync("This command provides access to the No Man's Sky channels which include a text and voice channel.");
                }
                else if (command.ToUpper() == "Space Engineers".ToUpper())
                {
                    await ReplyAsync("This command provides access to the Space Engineers channels with include a text and voice channel.");
                }
                else if (command.ToUpper() == "Warhammer 40K".ToUpper() || command.ToUpper() == "Warhammer".ToUpper())
                {
                    await ReplyAsync("This command provieds access to the Warhammer 40K channels which include a text and voice channel.");
                }
                else if (command.ToUpper() == "Check Standing".ToUpper())
                {
                    await ReplyAsync("This command has an optional field where you can type the username or mention the person of whom you wnat to know their standing.  " +
                        "If you leave this field blank, this command will return your own standing with The Raiders of the Lost Sector.");
                }
            }
            ResetAttempts(user);
            await SendLogMessage(user, "Command Help");
        }
        [Command("About")]
        public async Task about()
        {
            var user = Context.User as SocketGuildUser;
            if (user == null) return;
            await ReplyAsync("Hello, my name is Epsilon.  The Raiders of the Lost Sector was founded in New Eden years ago in the EVE universe.  " +
                "With new advancements in technologies, operations were moved from this universe and spread into others.  With this rapid growth " +
                "and expansion, it left the home colonies in New Eden desolate and barren and thus they died off.  The faction is still expanding into " +
                "other universes and will most likely continue to spread. " +
                "\n\nMy current version is " + versionNumber + ".");
            ResetAttempts(user);
            await SendLogMessage(user, "About");
        }
        [Command("Patch Notes")]
        public async Task patchNotes()
        {
            var user = Context.User as SocketGuildUser;
            if (user == null) return;
            await ReplyAsync("Here is a list of bug fixes and features in " + versionNumber + ":\n" +
                "```" +
                "Fixed:  Many command issues." +
                "Added:  More commands");
            ResetAttempts(user);
            await SendLogMessage(user, "Patch Notes");
        }
        [Command("Info")]
        public async Task info([Remainder] SocketGuildUser userId = null)
        {
            var user = Context.User as SocketGuildUser;
            if (user == null) return;
            if (userId == null)
            {
                var asking = GetUser(user);
                await ReplyAsync("Hello, " + user.Username + ".  Here is some of the information I have on you:\n" +
                    "You have a standing of " + asking.PersonalStanding + " with The Raiders of the Lost Sector.\n" +
                    "You have " + asking.NumberOfWarnings + " number of warnings against you.\n" +
                    "You have tried " + asking.NumberOfAttempts + " without a successful command.\n");
            }
            else
            {

            }
            ResetAttempts(user);
            await SendLogMessage(user, "Info");
        }
        [Command("Game Names")]
        public async Task gameNames()
        {
            var user = Context.User as SocketGuildUser;
            if (user == null) return;
            GetGameList();
            string gameNames = "";
            gameNames = string.Join(", ", GameNamesList);
            await ReplyAsync("Here is a list of available titles we currently have channels for on this server\n" +
                "" + gameNames);
            ResetAttempts(user);
            await SendLogMessage(user, "Game Names");
        }
        private async Task SendLogMessage(SocketGuildUser userID, string commandName)
        {
            var channel = Context.Guild.GetTextChannel(Epsilon.LogChannelID);
            await channel.SendMessageAsync("```" + userID.ToString() + " used the command " + commandName + " at " + DateTimeOffset.UtcNow + " UTC. ```");
        }
        private void ResetAttempts(SocketGuildUser user)
        {
            var guest = GetUser(user);
            guest.NumberOfAttempts = 0;
            SaveUser(guest);
        }
        private void SaveUser(User guest)
        {
            var db = new DatabaseContext();
            try
            {
                db.Users.Update(guest);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine("User Failed to retrieve. " + e.Message);
            }
        }
        private User GetUser(SocketGuildUser user)
        {
            var db = new DatabaseContext();
            try
            {
                return db.Users.Single(x => x.DiscordUserID == user.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("User failed to retrieve from the database. " + e.Message);
                return null;
            }
        }
        private void GetGameList()
        {
            try
            {
                StreamReader gameNameReader = new StreamReader("GameList.txt");
                while (!gameNameReader.EndOfStream)
                {
                    string gameName = gameNameReader.ReadLine();
                    GameNamesList.Add(gameName);
                }
                gameNameReader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to get list of games. " + e.Message);
            }
        }
    }
}

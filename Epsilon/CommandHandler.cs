using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Discord.WebSocket;
using Discord.Commands;
using Discord.Addons.Interactive;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace Epsilon
{
    public class CommandHandler
    {
        private static readonly string reader = string.Empty;
        private DiscordSocketClient _client;
        private CommandService _adminService;
        private CommandService _responsesService;
        private CommandService _helpService;
        private IServiceProvider _services;
        private InteractiveService _interactiveService;
        private List<ulong> InConvo = new List<ulong>();
        private SocketCommandContext Context = null;
        double c = 75.409748628491;
        double k = 0.334918564750126;
        public CommandHandler(DiscordSocketClient client)
        {
            _client = client;
            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton<InteractiveService>()
                .BuildServiceProvider();
            _adminService = new CommandService();
            _adminService.AddModuleAsync(typeof(Modules.AdminCommands), _services);
            _responsesService = new CommandService();
            _responsesService.AddModuleAsync(typeof(Modules.UserCommands), _services);
            _helpService = new CommandService();
            _helpService.AddModuleAsync(typeof(Modules.HelpCommands), _services);
            _interactiveService = new InteractiveService(_client, TimeSpan.FromSeconds(Epsilon.TimeoutTimeLimit));
            _client.MessageReceived += HandleCommandAsync;
            _client.MessageReceived += AddStanding;
            _client.MessageReceived += CheckRank;
        }
        private async Task HandleCommandAsync(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            var db = new DatabaseContext();
            if (msg == null) return;
            Context = new SocketCommandContext(_client, msg);
            string errText = string.Empty;
            var user = Context.User as SocketGuildUser;
            if (user != null)
            {
                var msgUser = GetUser(user, db);
                if (msgUser == null && ((user.Roles.Any(x => x.Id.Equals(Epsilon.SeniorOfficerID))) || user.Id.Equals(Epsilon.MasterID)))
                {
                    var newUser = new User();
                    newUser.ServerJoinDate = user.JoinedAt;
                    newUser.DiscordId = user.Id;
                    newUser.DiscordUserID = user.ToString(); ;
                    newUser.DiscordUsername = user.Username;
                    db.Users.Add(newUser);
                    db.SaveChanges();
                    msgUser = GetUser(user, db);
                    await Context.Guild.GetTextChannel(Epsilon.BotSpamChannelID).SendMessageAsync("The database was empty.  Populating now with the current members of " +
                        "the server.");
                }
                msgUser.LastMessageRecieved = DateTimeOffset.UtcNow;
                SaveUser(msgUser, db);
            }
            while (DateTimeOffset.UtcNow.Subtract(Epsilon.LastCheck).Days > 2)
            {
                Epsilon.LastCheck = Epsilon.LastCheck.AddDays(2);
            }
            if (DateTimeOffset.UtcNow.Subtract(Epsilon.LastCheck).Days >= 2)
            {
                CheckHiatus(db);
            }
            CommandService _service = null;
            int argPos = 0;
            var targetGuest = GetUser(user, db);
            targetGuest.NumberOfAttempts++;
            if (msg.HasCharPrefix('~', ref argPos))
            {
                if (msg.Channel.Id.Equals(Epsilon.BotSpamChannelID) || msg.Channel.Id.Equals(Epsilon.SecureChannelID))
                {
                    _service = _responsesService;
                    errText = "Something has gone terribly wrong with user commands.  ";
                }
                else
                {
                    await msg.DeleteAsync();
                    await Context.Guild.GetTextChannel(Epsilon.BotSpamChannelID).SendMessageAsync(msg.Author.Mention + ", please use commands in this channel.  " +
                        "This attempt makes " + targetGuest.NumberOfAttempts + " attempts.  As a reminder, three attempts will result in a warning.");
                    if (targetGuest.NumberOfAttempts == 1)
                    {
                        SaveUser(targetGuest, db);
                    }
                    else if (targetGuest.NumberOfAttempts == 2)
                    {
                        SaveUser(targetGuest, db);
                    }
                    else if (targetGuest.NumberOfAttempts >= 3)
                    {
                        targetGuest.NumberOfAttempts = 0;
                        targetGuest.NumberOfWarnings++;
                        await CheckWarnings(user, targetGuest, msg, db);
                        SaveUser(targetGuest, db);
                        await Context.Guild.GetTextChannel(Epsilon.BotSpamChannelID).SendMessageAsync(msg.Author.Mention + ", you have failed three times to send " +
                            "a command in the correct channel.  You now have " + targetGuest.NumberOfWarnings + " number of warnings against you.  " +
                            "If you reach three warnings, your standing with " + Epsilon.OrganizationName + " will drop.");
                    }
                }
            }
            else if (msg.HasCharPrefix('-', ref argPos))
            {
                _service = _adminService;
                errText = "Something has gone terribly wrong with admin commands.  ";
            }
            else if (msg.HasCharPrefix('?', ref argPos))
            {
                _service = _helpService;
                errText = "Something has gone terribly wrong with our help commands.  ";
            }
            if (_service != null)
            {
                var result = await _service.ExecuteAsync(Context, argPos, _services);
                if (result.ErrorReason == "User not found.")
                {
                    await Context.Channel.SendMessageAsync("I'm sorry, but the person does not belong to this server.  " +
                        "I can only give you information on those who are in this server.");
                }
                else if (!result.IsSuccess && result.Error == CommandError.UnknownCommand)
                {
                    if (targetGuest.NumberOfAttempts == 1)
                    {
                        await Context.Channel.SendMessageAsync("Nope.  The command you tried to enter was not correct.  I would suggest you try the command" +
                            " '?Help' for a list of available help commands, or you could try the command '?Command Help (command)' for help on a specific command.  Good try though." +
                            "\n" + result.ErrorReason);
                        SaveUser(targetGuest, db);
                    }
                    else if (targetGuest.NumberOfAttempts == 2)
                    {
                        await Context.Channel.SendMessageAsync("I have given you options where you can find the correct information.  If you are not capable of " +
                            "typing a correct command, I will have to issue a warning.  The number of consecutive incorrect attempts is now at " + targetGuest.NumberOfAttempts +
                            ".  Once you reach three attempts, a warning will be issued.");
                        SaveUser(targetGuest, db);
                    }
                    else if (targetGuest.NumberOfAttempts >=3)
                    {
                        await Context.Channel.SendMessageAsync("You have left me no other choice.  Since you refuse to seek help, I am issuing you a warning.");
                        targetGuest.NumberOfAttempts = 0;
                        targetGuest.NumberOfWarnings++;
                        await CheckWarnings(user, targetGuest, msg, db);
                        SaveUser(targetGuest, db);
                    }
                }
            }
        }
        private User GetUser(SocketUser user, DatabaseContext db)
        {
            if (user.IsBot != true)
            {
                try
                {
                    return db.Users.Single(x => x.DiscordId == user.Id);
                }
                catch (Exception e)
                {
                    Console.WriteLine("I was unable to locate the Guest in the database. " + e.Message);
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        private void SaveUser(User user, DatabaseContext db)
        {
            try
            {
                db.Users.Update(user);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine("Guest failed to update and save. " + e.Message);
            }
        }
        private async Task SetStandingRole(SocketGuildUser user, string roleTitle)
        {
            var currentStandingRole = user.Roles.FirstOrDefault(x => Epsilon.StandingRolesList.Any(t => t.Equals(x.Name,StringComparison.OrdinalIgnoreCase)));
            var newStandingRole = user.Guild.Roles.FirstOrDefault(x => x.Name.Equals(roleTitle, StringComparison.OrdinalIgnoreCase));
            if (currentStandingRole != null)
            {
                await user.RemoveRoleAsync(currentStandingRole);
            }
            await user.AddRoleAsync(newStandingRole);
        }
        private async Task AddStanding(SocketMessage messageRecieved)
        {
            var msg = messageRecieved as SocketUserMessage;
            if (msg == null) return;
            var db = new DatabaseContext();
            var Context = new SocketCommandContext(_client, msg);
            var user = Context.User as SocketGuildUser;
            var targetGuest = GetUser(user, db);
            if (db.Users.Any(x => x.DiscordUserID == targetGuest.DiscordUserID))
            {
                targetGuest.PersonalStanding = ((10 - targetGuest.PersonalStanding) * 0.00012207F) + targetGuest.PersonalStanding;
                await CheckStanding(user, targetGuest);
                SaveUser(targetGuest, db);
            }
        }
        private async Task CheckStanding(SocketGuildUser user, User guest)
        {
            if (guest.PersonalStanding >= 10)
            {
                await SetStandingRole(user, "Trusted");
            }
            else if (guest.PersonalStanding < 10 && guest.PersonalStanding >= 5)
            {
                await SetStandingRole(user, "Friendly");
            }
            else if (guest.PersonalStanding < 5 && guest.PersonalStanding >= 0)
            {
                await SetStandingRole(user, "Neutral");
            }
            else if (guest.PersonalStanding < 0 && guest.PersonalStanding >= -5)
            {
                await SetStandingRole(user, "Suspect");
            }
            else if (guest.PersonalStanding < -5 && guest.PersonalStanding >= -10)
            {
                await SetStandingRole(user, "Criminal");
            }
        }
        private async Task CheckWarnings(SocketGuildUser user, User guest, SocketMessage s, DatabaseContext db)
        {
            var msg = s as SocketUserMessage;
            if (msg == null) return;
            var Context = new SocketCommandContext(_client, msg);
            if (guest.NumberOfWarnings == 3)
            {
                guest.PersonalStanding = ((-10 - guest.PersonalStanding) * 0.1F) + guest.PersonalStanding;
                await CheckStanding(user, guest);
                SaveUser(guest, db);
                await Context.Channel.SendMessageAsync(user.Username + ", you have reached 3 warnings and I will have to drop your standing.  You now have a standing of " + guest.PersonalStanding + ".");
            }
            else if (guest.NumberOfWarnings > 3)
            {
                guest.NumberOfWarnings = 1;
                SaveUser(guest, db);
            }
        }
        private Task CheckRank(SocketMessage messageRecieved)
        {
            var msg = messageRecieved as SocketUserMessage;
            if (msg == null || msg.Author.IsBot) return Task.CompletedTask;
            if (InConvo.Contains(msg.Author.Id)) return Task.CompletedTask;
            var Context = new SocketCommandContext(_client, msg);
            var user = Context.User as SocketGuildUser;
            var db = new DatabaseContext();
            var targetUser = GetUser(user, db);
            var announceChannel = Context.Guild.GetTextChannel(Epsilon.AnnounceChannelID);
            var secureChannel = Context.Guild.GetTextChannel(Epsilon.SecureChannelID);
            var botChannel = Context.Guild.GetTextChannel(Epsilon.BotSpamChannelID);
            if (targetUser.JoinedFaction == true)
            {
                var daysInFaction = DateTimeOffset.Now.Subtract(targetUser.FactionJoinDate.Value).Days;
                char gradeLetter = targetUser.Grade[0];
                int gradeNumber = int.Parse(targetUser.Grade[1].ToString()) * 10 + int.Parse(targetUser.Grade[2].ToString());
                var requiredDays = Math.Round(c * (Math.Exp(k * (gradeNumber + 1)) - 1), 0);
                if (gradeLetter != 'M' && gradeLetter != 'O' && gradeNumber < 4)
                {
                    while (gradeNumber < 4 && Math.Round(c * (Math.Exp(k * (gradeNumber + 1)) - 1), 0) < daysInFaction)
                    {
                        gradeNumber++;
                    }
                    if (gradeNumber == 4)
                    {
                        double rankPointValue = (int)Math.Pow(10, gradeNumber - 4);
                        if (targetUser.PointBalance >= rankPointValue - 1)
                        {
                            InConvo.Add(msg.Author.Id);
                            _ = Task.Run(async () =>
                            {
                                await botChannel.SendMessageAsync(user.Mention + ", you have reached the point where we offer you a career path choice.  " +
                                      "You can choose the management path where promotion points will be required along with time in grade to advance to the " +
                                      "next rank.  These are aquired through leadership skills and achievements.  \n" +
                                      "The non-management path does not require promotion points.  As a result, these positions carry less responsabilities " +
                                      "and therefore are not permitted to be in charge of groups.  Instead, non-managers will report to the managers whom " +
                                      "they are placed under. \n" +
                                      "Which path do you choose?  You have " + Epsilon.TimeoutTimeLimit + " seconds to reply, or this will time out.");
                                var response = await _interactiveService.NextMessageAsync(Context);
                                if (response != null && response.Content.Equals("management", StringComparison.OrdinalIgnoreCase))
                                {
                                    await botChannel.SendMessageAsync("You have chosen the management side.  Congratulations!");
                                    gradeNumber = 1;
                                    if (gradeLetter.Equals('E'))
                                    {
                                        gradeLetter = 'O';
                                    }
                                    else
                                    {
                                        gradeLetter = 'M';
                                    }
                                    bool done = false;
                                    if (Math.Round(c * (Math.Exp(k * (gradeNumber + 3)) - 1), 0) < daysInFaction)
                                    {
                                        while (Math.Round(c * (Math.Exp(k * (gradeNumber + 4)) - 1), 0) < daysInFaction)
                                        {
                                            rankPointValue = (int)Math.Pow(10, gradeNumber);
                                            if (targetUser.PointBalance >= rankPointValue)
                                            {
                                                gradeNumber++;
                                                done = true;
                                            }
                                        }
                                        if (gradeNumber <= 9)
                                        {
                                            targetUser.Grade = gradeLetter + "0" + gradeNumber.ToString();
                                        }
                                        else
                                        {
                                            targetUser.Grade = gradeLetter + gradeNumber.ToString();
                                        }
                                        done = true;
                                        targetUser.PromotionDate = DateTimeOffset.UtcNow;
                                    }
                                    await SetRank(user, targetUser.Grade, db);
                                    if (done)
                                    {
                                        await announceChannel.SendMessageAsync(user.Mention + 
                                            ", congratuations on your promotion!  you have been a member for " + daysInFaction + " days, and have earned a promotion!" +
                                            "  You have been promoted to " + targetUser.Grade + " " + targetUser.Rank + ".");
                                    }
                                }
                                else if (response != null && response.Content.Equals("non-management", StringComparison.OrdinalIgnoreCase))
                                {
                                    await botChannel.SendMessageAsync("You have chosen the non-managment side.  Well done!");
                                    bool done = false;
                                    if (Math.Round(c * (Math.Exp(k * (gradeNumber + 4)) - 1), 0) < daysInFaction)
                                    {
                                        while (Math.Round(c * (Math.Exp(k * (gradeNumber + 4)) - 1), 0) < daysInFaction)
                                        {
                                            gradeNumber++;
                                            if (targetUser.PointBalance >= rankPointValue)
                                            {
                                                if (gradeNumber <= 9)
                                                {
                                                    targetUser.Grade = gradeLetter + "0" + gradeNumber.ToString();
                                                }
                                                else
                                                {
                                                    targetUser.Grade = gradeLetter + gradeNumber.ToString();
                                                }
                                                targetUser.PromotionDate = DateTimeOffset.UtcNow;
                                                await SetRank(user, targetUser.Grade, db);
                                            }
                                        }
                                        done = true;
                                    }
                                    if (done)
                                    {
                                        await SetRank(user, targetUser.Grade, db);
                                        await announceChannel.SendMessageAsync("@everyone, let us congragulate " + targetUser.DiscordUsername +
                                            " on their promotion!  They have been a member for " + daysInFaction + " days, and have earned a promotion!  " +
                                            user.Mention + " You have been promoted to " + targetUser.Grade + " " + targetUser.Rank + ".");
                                    }
                                    SaveUser(targetUser, db);
                                    await SetRank(user, targetUser.Grade, db);
                                }
                                else
                                {
                                    await botChannel.SendMessageAsync("I'm sorry, that response is invalid");
                                }
                                if (InConvo.Contains(msg.Author.Id))
                                    InConvo.Remove(msg.Author.Id);
                            });
                            return Task.CompletedTask;
                        }
                    }
                    else
                    {
                        _ = Task.Run(async () =>
                       {
                           if (gradeNumber < 9)
                           {
                               targetUser.Grade = gradeLetter + "0" + gradeNumber;
                           }
                           else
                           {
                               targetUser.Grade = gradeLetter + gradeNumber.ToString();
                           }
                           SaveUser(targetUser, db);
                           await SetRank(user, targetUser.Grade, db);
                       });
                    }
                }
                else
                {
                    _ = Task.Run(async () =>
                    {
                        bool done = false;
                        if (Math.Round(c * (Math.Exp(k * (gradeNumber + 4)) - 1), 0) < daysInFaction)
                        {
                            while (Math.Round(c * (Math.Exp(k * (gradeNumber + 4)) - 1), 0) < daysInFaction && !done)
                            {
                                double rankPointValue = (int)Math.Pow(10, gradeNumber);
                                if (targetUser.PointBalance >= rankPointValue)
                                {
                                    gradeNumber++;
                                    if (gradeNumber <= 9)
                                    {
                                        targetUser.Grade = gradeLetter + "0" + gradeNumber.ToString();
                                    }
                                    else
                                    {
                                        targetUser.Grade = gradeLetter + gradeNumber.ToString();
                                    }
                                    targetUser.PromotionDate = DateTimeOffset.UtcNow;
                                    await SetRank(user, targetUser.Grade, db);
                                    await announceChannel.SendMessageAsync("@everyone, let us congragulate " + targetUser.DiscordUsername +
                                        " on their promotion!  They have been a member for " + daysInFaction + " days, and have earned a promotion!  " +
                                        user.Mention + " You have been promoted to " + targetUser.Grade + " " + targetUser.Rank + ".");
                                }
                                else
                                {
                                    done = true;
                                    await SetRank(user, targetUser.Grade, db);
                                }
                            }
                        }
                    });
                }                
            }
            else
            {
                if (DateTimeOffset.UtcNow.Subtract(targetUser.ServerJoinDate.Value).Days > 365)
                {
                    var ambassadorRole = user.Guild.Roles.FirstOrDefault(x => x.Name.Equals("ambassador", StringComparison.OrdinalIgnoreCase));
                    var diplomatRole = user.Guild.Roles.FirstOrDefault(x => x.Name.Equals("diplomat", StringComparison.OrdinalIgnoreCase));
                    user.RemoveRoleAsync(diplomatRole);
                    user.AddRoleAsync(ambassadorRole);
                }
            }
            return Task.CompletedTask;
        }
        private async Task SetRank(SocketGuildUser user, string grade, DatabaseContext db)
        {
            var targetUser = GetUser(user, db);
            var currentRole = user.Guild.Roles.FirstOrDefault(x => x.Name.Equals(targetUser.Rank, StringComparison.OrdinalIgnoreCase));
            if (currentRole != null)
            {
                await user.RemoveRoleAsync(currentRole);
            }
            if (Epsilon.RanksDictionary.TryGetValue(grade, out string title))
            {
                targetUser.Grade = grade;
                targetUser.Rank = title;
                var newRole = user.Guild.Roles.FirstOrDefault(x => x.Name.Equals(title, StringComparison.OrdinalIgnoreCase));
                if (newRole != null)
                {
                    await user.AddRoleAsync(newRole);
                }
                SaveUser(targetUser, db);
            }
        }
        private void CheckHiatus(DatabaseContext db)
        {
            var hiatusList = db.Users.ToList();
            foreach (var member in hiatusList)
            {
                var days = DateTimeOffset.UtcNow.Subtract(member.LastMessageRecieved.Value).Days;
                if (days >= 30)
                {
                    var users = Context.Guild.Users.ToList();
                    var user = users.FirstOrDefault(x => x.Id.Equals(member.DiscordId));
                    var currentRole = user.Roles.FirstOrDefault(x => x.Id.Equals(Epsilon.DUActiveID));
                    var hiatusRole = user.Guild.Roles.FirstOrDefault(x => x.Id.Equals(Epsilon.HiatusID));
                    user.RemoveRoleAsync(currentRole);
                    user.AddRoleAsync(hiatusRole);
                }
            }
        }
    }
}

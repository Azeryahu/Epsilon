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
using Newtonsoft.Json;
using Discord;

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
            _client.MessageReceived += HandleCommandAsync;
        }
        private async Task HandleCommandAsync(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            var msgChannel = msg.Channel.GetType().FullName;
            var db = new DatabaseContext();
            if (msg.Equals(null)) return;
            Context = new SocketCommandContext(_client, msg);
            string errText = string.Empty;
            var user = Context.User as SocketGuildUser;
            /*try
            {
                if (!user.IsBot)
                {
                    var msgUser = GetUser(user, db);
                    if (user.Equals(null))
                    {

                    }
                    else
                    {
                        msgUser.LastMessageRecieved = DateTimeOffset.UtcNow;
                        SaveUser(msgUser, db);
                    }
                }
            }
            catch (Exception e )
            {
                Console.WriteLine("Error in getting user from database. CommandHandler line 56" + e.Message);
            }*/
            if (msg.Channel.GetType().FullName.Equals("Discord.WebSocket.SocketTextChannel") && !user.Guild.Name.Equals(Epsilon.ConfigFile.OrganizationName, StringComparison.OrdinalIgnoreCase))
            {
                Epsilon.ConfigFile.OrganizationName = user.Guild.Name;
                var configToJson = JsonConvert.SerializeObject(Epsilon.ConfigFile, Formatting.Indented);
                StreamWriter configWriter = new StreamWriter("config.json");
                configWriter.Write(configToJson);
                configWriter.Close();
            }
            if (!user.Equals(null) && !user.IsBot)
            {
                var msgUser = GetUser(user, db);
                if (msgUser == null && (user.Roles.Any(x => x.Id.Equals(Epsilon.ConfigFile.SeniorOfficerRoleID))) || user.Id.Equals(Epsilon.ConfigFile.BotMasterID))
                {
                    var newUser = new User();
                    newUser.ServerJoinDate = user.JoinedAt;
                    newUser.LastMessageRecieved = DateTimeOffset.UtcNow;
                    newUser.DiscordId = user.Id;
                    newUser.DiscordUserID = user.ToString();
                    newUser.DiscordUsername = user.Username;
                    newUser.VerificationKey = KeyGenerator.GetUniqueKey(32);
                    newUser.PersonalStanding = 10;
                    newUser.FactionJoinDate = user.JoinedAt;
                    newUser.PromotionDate = user.JoinedAt.Value.AddDays(30);
                    newUser.CanJoin = true;
                    newUser.JoinedFaction = true;
                    newUser.Verified = true;
                    db.Users.Add(newUser);
                    db.SaveChanges();
                    msgUser = GetUser(user, db);
                    await Context.Guild.GetTextChannel(Epsilon.ConfigFile.BotSpamChannelID).SendMessageAsync("The database was empty.  Populating now with the current members of " +
                        "the server.");
                }
                
            }
            /*while (DateTimeOffset.UtcNow.Subtract(Epsilon.LastCheck).Days > 2)
            {
                Epsilon.LastCheck = Epsilon.LastCheck.AddDays(2);
            }*/
            if (DateTimeOffset.UtcNow.Subtract(Epsilon.LastCheck).Days >= Epsilon.ConfigFile.HiatusCheckDays)
            {
                CheckHiatus(db);
                Epsilon.LastCheck = DateTimeOffset.UtcNow;
            }
            CommandService _service = null;
            int argPos = 0;
            /*var targetGuest = GetUser(user, db);
            if (targetGuest.JoinedFaction)
            {
                await CheckRank(msg);
                await CheckStanding(user, targetGuest);
            }
            targetGuest.NumberOfAttempts++;*/
            if (msg.HasCharPrefix('~', ref argPos))
            {
                if (msg.Channel.Id.Equals(Epsilon.ConfigFile.BotSpamChannelID) || msg.Channel.Id.Equals(Epsilon.ConfigFile.SecureSpamChannelID))
                {
                    _service = _responsesService;
                    errText = "Something has gone terribly wrong with user commands.  ";
                }
                else
                {
                    await msg.DeleteAsync();
                    /*await Context.Guild.GetTextChannel(Epsilon.ConfigFile.BotSpamChannelID).SendMessageAsync(msg.Author.Mention + ", please use commands in this channel.  " +
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
                        await Context.Guild.GetTextChannel(Epsilon.ConfigFile.BotSpamChannelID).SendMessageAsync(msg.Author.Mention + ", you have failed three times to send " +
                            "a command in the correct channel.  You now have " + targetGuest.NumberOfWarnings + " number of warnings against you.  " +
                            "If you reach three warnings, your standing with " + Epsilon.OrganizationName + " will drop.");
                    }*/
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
                if (!result.IsSuccess)
                {
                    if (result.ErrorReason.Equals("User not found.", StringComparison.OrdinalIgnoreCase))
                    {
                        await Context.Channel.SendMessageAsync("I'm sorry, but the person does not belong to this server.  " +
                            "I can only give you information on those who are in this server.");
                    }
                    else if (result.ErrorReason.Equals("The input text has too few parameters.", StringComparison.OrdinalIgnoreCase))
                    {
                        await Context.Channel.SendMessageAsync("You did not use this command correctly.  Please use the \"?Help {command name (optional)}\" if you require further assistance.");
                    }
                    else if (result.ErrorReason.Equals("Unknown command.", StringComparison.OrdinalIgnoreCase))
                    {
                        await Context.Channel.SendMessageAsync("This is not a command I know. Try using the command \"?Help\"");
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("You did something wrong.  Please use the command \"?Help {command name (optional)}\" if you need further assistance.");
                    }
                }
            }
        }
    //Methods
        private User GetUser(SocketGuildUser user, DatabaseContext db)
        {
            if (!user.IsBot)
            {
                try
                {
                    return db.Users.Single(x => x.DiscordId.Equals(user.Id));
                }
                catch (Exception e)
                {
                    if (!user.Id.Equals(Epsilon.ConfigFile.BotMasterID))
                    {
                        Context.Guild.GetTextChannel(Epsilon.ConfigFile.SecureSpamChannelID).SendMessageAsync(user.Mention + " Something has gone wrong with the Users " +
                            "database.  " + e.Message);
                        Console.WriteLine("I was unable to locate the Guest in the database. " + e.Message);
                    }
                    else if (user.Id.Equals(Epsilon.ConfigFile.BotMasterID))
                    {

                    }
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
        private void CheckHiatus(DatabaseContext db)
        {
            var hiatusList = db.Users.ToList();
            foreach (var member in hiatusList)
            {
                var users = Context.Guild.Users.ToList();
                var user = users.FirstOrDefault(x => x.Id.Equals(member.DiscordId));
                var guildUser = Context.Guild.GetUser(member.DiscordId);
                if (!member.LastMessageRecieved.Equals(null))
                {
                    var days = DateTimeOffset.UtcNow.Subtract(member.LastMessageRecieved.Value).Days;
                    if (days >= 30 && !guildUser.Roles.Any(x => x.Id.Equals(Epsilon.ConfigFile.DualUniverseHiatusID)) && guildUser.Roles.Any(x => x.Id.Equals(Epsilon.ConfigFile.DualUniverseActiveID)))
                    {
                        var currentRole = user.Roles.FirstOrDefault(x => x.Id.Equals(Epsilon.ConfigFile.DualUniverseActiveID));
                        var hiatusRole = user.Guild.Roles.FirstOrDefault(x => x.Id.Equals(Epsilon.ConfigFile.DualUniverseHiatusID));
                        user.RemoveRoleAsync(currentRole);
                        user.AddRoleAsync(hiatusRole);
                    }
                    else if (days < 30 && guildUser.Roles.Any(x => x.Id.Equals(Epsilon.ConfigFile.DualUniverseHiatusID)))
                    {
                        var currentRole = user.Guild.Roles.FirstOrDefault(x => x.Id.Equals(Epsilon.ConfigFile.DualUniverseHiatusID));
                        var activeRole = user.Roles.FirstOrDefault(x => x.Id.Equals(Epsilon.ConfigFile.DualUniverseActiveID));
                        user.RemoveRoleAsync(currentRole);
                        user.AddRoleAsync(activeRole);
                    }
                }
                else if (user != null)
                {                    
                    if (guildUser.Roles.Any(x => x.Id.Equals(Epsilon.ConfigFile.DualUniverseActiveID)))
                    {
                        var currentRole = user.Roles.FirstOrDefault(x => x.Id.Equals(Epsilon.ConfigFile.DualUniverseActiveID));
                        var hiatusRole = user.Guild.Roles.FirstOrDefault(x => x.Id.Equals(Epsilon.ConfigFile.DualUniverseHiatusID));
                        user.RemoveRoleAsync(currentRole);
                        user.AddRoleAsync(hiatusRole);
                    }
                }
            }
        }
        private void SetRank(User user, DatabaseContext db)
        {

        }
    }
}

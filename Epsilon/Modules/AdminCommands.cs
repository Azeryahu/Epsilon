using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Discord.Commands;
using Discord.WebSocket;
using Discord;
using Discord.Addons.Interactive;
using System.Globalization;
using Newtonsoft.Json;

namespace Epsilon.Modules
{
    public class AdminCommands : InteractiveBase<SocketCommandContext>
    {
        [Command("Update Database")]
        public async Task updateDatabase()
        {
            bool commandResult = false;
            var user = Context.User as SocketGuildUser;
            if (user.Equals(null)) return;
            var db = new DatabaseContext();
            if (user.Id.Equals(Epsilon.ConfigFile.BotMasterID) || user.Roles.Any(x => x.Id.Equals(Epsilon.ConfigFile.SeniorOfficerRoleID) || x.Id.Equals(Epsilon.ConfigFile.AdministratorRoleID)))
            {
                await ReplyAsync("Please wait while I process this request.");
                int n = 0;
                var guildUsers = Context.Guild.Users.ToList();
                var existingUsers = db.Users.ToList();
                while (n < Context.Guild.Users.Count)
                {
                    var addedUser = guildUsers[n];
                    if (!addedUser.IsBot)
                    {
                        if (existingUsers.Any(x => x.DiscordId.Equals(addedUser.Id)))
                        {
                            var existingUser = GetUser(addedUser);
                            existingUser.ServerJoinDate = addedUser.JoinedAt;
                            existingUser.DiscordUserID = addedUser.ToString();
                            existingUser.DiscordUsername = addedUser.Username;
                            await CheckStanding(addedUser, existingUser);
                            SaveUser(existingUser);
                            n++;
                        }
                        else
                        {
                            var newMember = new User();
                            newMember.ServerJoinDate = addedUser.JoinedAt;
                            newMember.DiscordId = addedUser.Id;
                            newMember.DiscordUserID = addedUser.ToString();
                            newMember.DiscordUsername = addedUser.Username;
                            await SetStandingRole(addedUser, "Neutral");
                            SaveUser(newMember);
                            n++;
                        }
                    }
                    else
                    {
                        n++;
                    }
                }
                await ReplyAsync("I have updated my databanks with the current members of the server.");
                commandResult = true;
            }
            else
            {
                var invalidUser = GetUser(user);
                invalidUser.NumberOfAttempts += 1;
                await CheckWarnings(user, invalidUser);
                SaveUser(invalidUser);
                await ReplyAsync("You do not have the authority to update the database as only Directors and Administrative Consultants are able to do so.  " +
                    "10 failed attempts in a row to use commands will result in a warning. You now have attempted " + invalidUser.NumberOfAttempts + " times now.");
            }
            ResetAttempts(user);
            await SendLogMessage(user, "Update Database", commandResult);
        }
        [Command("Clear Chat")]
        [Summary("Deletes set amount of messages.")]
        public async Task clearChat(ulong channelID)
        {
            bool commandResult = false;
            var user = Context.User as SocketGuildUser;
            if (user.Equals(null)) return;
            var db = new DatabaseContext();
            int number = int.MaxValue;
            if (user.Id.Equals(Epsilon.ConfigFile.BotMasterID) || user.Roles.Any(x => x.Id.Equals(Epsilon.ConfigFile.SeniorOfficerRoleID) || x.Id.Equals(Epsilon.ConfigFile.AdministratorRoleID)))
            {
                var messageChannel = Context.Guild.GetTextChannel(channelID);
                var messages = await messageChannel.GetMessagesAsync(number).FlattenAsync();
                foreach (var message in messages)
                {
                    await message.DeleteAsync();
                    await Task.Delay(900);
                }
                //await messageChannel.DeleteMessagesAsync(messages); //only deletes if messages are less than 14 days old.
                await ReplyAsync("The operation has completed.");
            }
            ResetAttempts(user);
            await SendLogMessage(user, "Clear Chat", commandResult);
        }
        [Command("Reload Config", RunMode = RunMode.Async)]
        [Alias("Update Config")]
        public async Task updateConfig()
        {
            bool commandResult = false;
            var user = Context.User as SocketGuildUser;
            if (user.Equals(null)) return;
            var db = new DatabaseContext();
            if (user.Id.Equals(Epsilon.ConfigFile.BotMasterID) || user.Roles.Any(x => x.Id.Equals(Epsilon.ConfigFile.SeniorOfficerRoleID) || x.Id.Equals(Epsilon.ConfigFile.AdministratorRoleID)))
            {
                await ReplyAsync("You are about to reload the config file because of a change. Do you wish to continue?");
                var response = await NextMessageAsync(true, true, Epsilon.WaitTime);
                if (response.Content.Equals("yes", StringComparison.OrdinalIgnoreCase))
                {
                    StreamReader configReader = new StreamReader("config\\config.json");
                    string configString = configReader.ReadToEnd();
                    Epsilon.ConfigFile = JsonConvert.DeserializeObject<Config>(configString);
                    await ReplyAsync("I have reloaded the config.");
                    commandResult = true;
                }
                else if (!response.Content.Equals(null))
                {

                }
                else
                {
                    await ReplyAsync("Response time expired. Please try again.");
                }
            }
            else
            {

            }
            ResetAttempts(user);
            await SendLogMessage(user, "Reload Config", commandResult);
        }
        //Standing Commands
        [Command("Post Standings")]
        public async Task postStandings()
        {
            bool commandResult = false;
            var user = Context.User as SocketGuildUser;
            if (user == null) return;
            if (user.Id.Equals(Epsilon.ConfigFile.BotMasterID) || user.Roles.Any(x => x.Name.Equals("Administrator", StringComparison.OrdinalIgnoreCase)))
            {
                GoogleSheets.StandingSheetUpdate();
                await ReplyAsync("I have updated the list.");
                commandResult = true;
            }
            else
            {
                var invalidUser = GetUser(user);
                invalidUser.NumberOfWarnings++;
                await CheckWarnings(user, invalidUser);
                SaveUser(invalidUser);
                await ReplyAsync("Silly, you do not have the authority to verify KOS requests as only Directors and " +
                    "Administrative Consultants are able to do so.  You have been warned, and you now have " + invalidUser.NumberOfWarnings + " warnings.");
            }
            ResetAttempts(user);
            await SendLogMessage(user, "Post Standings", commandResult);
        }
        [Command("Add Standing")]
        public async Task addStanding(int addedStanding, [Remainder] SocketGuildUser userId)
        {
            bool commandResult = false;
            var user = Context.User as SocketGuildUser;
            if (user == null) return;
            var channel = Context.Guild.GetTextChannel(Epsilon.ConfigFile.AnnouncementChannelID);
            if (user.Id.Equals(Epsilon.ConfigFile.BotMasterID) || user.Roles.Any(x => x.Name.Equals("Administrator", StringComparison.OrdinalIgnoreCase)))
            {
                try
                {
                    var targetUser = GetUser(userId);
                    if (targetUser == null)
                    {
                        await ReplyAsync("I am sorry, but you have failed to provide me with the person to whom you would like to add standing.");
                        return;
                    }
                    targetUser.PersonalStanding = ((10 - targetUser.PersonalStanding) * addedStanding / 100) + targetUser.PersonalStanding;
                    await CheckStanding(userId, targetUser);
                    if (targetUser.PersonalStanding >= 10)
                    {
                        await channel.SendMessageAsync("Congragulations " + targetUser.DiscordUsername + ", your standing with the " + Epsilon.ConfigFile.OrganizationName + " has increased to " + Math.Round(targetUser.PersonalStanding, 2) + " by " + user.Username + ", and you are trustworthy here.");
                    }
                    else if (targetUser.PersonalStanding >= 5 && targetUser.PersonalStanding < 10)
                    {
                        await channel.SendMessageAsync("Congragulations " + targetUser.DiscordUsername + ", your standing with the " + Epsilon.ConfigFile.OrganizationName + " has increased and is now at " + Math.Round(targetUser.PersonalStanding, 2) + ". You have proven you can be friendly with others.");
                    }
                    else if (targetUser.PersonalStanding >= 0 && targetUser.PersonalStanding < 5)
                    {
                        await channel.SendMessageAsync("Congragulations " + targetUser.DiscordUsername + ", your standing with the " + Epsilon.ConfigFile.OrganizationName + " has increased and is now at " + Math.Round(targetUser.PersonalStanding, 2) + ".  You are considered to be neutral.");
                    }
                    else if (targetUser.PersonalStanding < 0 && targetUser.PersonalStanding >= -5)
                    {
                        await channel.SendMessageAsync("Congragulations " + targetUser.DiscordUsername + ", your standing with the" + Epsilon.ConfigFile.OrganizationName + " has increased to a " + Math.Round(targetUser.PersonalStanding, 2) + ", but we are keeping a close eye on you as you are a very suspicious character.");
                    }
                    else if (targetUser.PersonalStanding < -5 && targetUser.PersonalStanding >= -10)
                    {
                        await channel.SendMessageAsync("Congragulations " + targetUser.DiscordUsername + ", your standing with the " + Epsilon.ConfigFile.OrganizationName + " has increased to a " + Math.Round(targetUser.PersonalStanding, 2) + ", but you are still considered abhorred by the Federation");
                    }
                    else if (targetUser.PersonalStanding < -10)
                    {
                        await channel.SendMessageAsync("Something has gone terribly wrong with your standing and you now have a standing of " + Math.Round(targetUser.PersonalStanding, 2) + " which shouldn't be possible.  Please contact Lambert to correct this.");
                    }
                    commandResult = true;
                    SaveUser(targetUser);
                }
                catch
                {
                    await ReplyAsync("I was unable to add standings.  Please see Lambert for further help.");
                }
            }
            else
            {
                var invalidUser = GetUser(user);
                invalidUser.NumberOfWarnings++;
                await CheckWarnings(user, invalidUser);
                SaveUser(invalidUser);
                await ReplyAsync("Silly, you do not have the authority to add standings as only Administrators are able to add standing.  You have been warned, and you now have " + invalidUser.NumberOfWarnings + " warnings.");
            }
            ResetAttempts(user);
            await SendLogMessage(user, "Add Standing", commandResult);
        }
        [Command("Remove Standing")]
        [Alias("Take Standing")]
        public async Task takeStanding(int takenStanding, [Remainder] SocketGuildUser userId)
        {
            bool commandResult = false;
            var user = Context.User as SocketGuildUser;
            if (user == null) return;
            var channel = Context.Guild.GetTextChannel(Epsilon.ConfigFile.SecureSpamChannelID);
            if (user.Id == Epsilon.ConfigFile.BotMasterID || user.Roles.Any(x => x.Name == "Administrator"))
            {
                try
                {
                    var targetUser = GetUser(userId);
                    if (targetUser == null) return;
                    targetUser.PersonalStanding = ((-10 - targetUser.PersonalStanding) * takenStanding / 100) + targetUser.PersonalStanding;
                    await CheckStanding(userId, targetUser);
                    if (targetUser.PersonalStanding >= 10)
                    {
                        await channel.SendMessageAsync(userId.Mention + ", I'm not sure what you did, but " + user.Username + " has reduced your standing to " + Math.Round(targetUser.PersonalStanding, 2) + ", but don't worry, you are still trusted.  I would just make sure you don't do what you did again.");
                    }
                    else if (targetUser.PersonalStanding >= 5 && targetUser.PersonalStanding < 10)
                    {
                        await channel.SendMessageAsync("I am sorry to inform you " + userId.Mention + ", but your standing with the " + Epsilon.ConfigFile.OrganizationName + " has decreased and is now at " + Math.Round(targetUser.PersonalStanding, 2) + ".  You have proven you are friendly.");
                    }
                    else if (targetUser.PersonalStanding >= 0 && targetUser.PersonalStanding < 5)
                    {
                        await channel.SendMessageAsync("I am sorry to inform you " + userId.Mention + ", your standing with the " + Epsilon.ConfigFile.OrganizationName + " has decreased and is now at " + Math.Round(targetUser.PersonalStanding, 2) + ".  You are considered to be neutral.");
                    }
                    else if (targetUser.PersonalStanding < 0 && targetUser.PersonalStanding >= -5)
                    {
                        await channel.SendMessageAsync("I am sorry to inform you " + userId.Mention + ", your standing with the " + Epsilon.ConfigFile.OrganizationName + " has decreased to a " + Math.Round(targetUser.PersonalStanding, 2) + ". We are keeping a close eye on you as you are a very suspicious character.");
                    }
                    else if (targetUser.PersonalStanding <= -5 && targetUser.PersonalStanding > -10)
                    {
                        await channel.SendMessageAsync("I am sorry to inform you " + userId.Mention + ", your standing with the " + Epsilon.ConfigFile.OrganizationName + " has decreased to a " + Math.Round(targetUser.PersonalStanding, 2) + ". You are abhorred by the Federation.");
                    }
                    else if (targetUser.PersonalStanding <= -10)
                    {
                        targetUser.PersonalStanding = -10;
                        await channel.SendMessageAsync(userId.Mention + ", your standing with the " + Epsilon.ConfigFile.OrganizationName + " is now at " + Math.Round(targetUser.PersonalStanding, 2) + " by " + user.Username + ", and this is the lowest standing possible.");
                    }
                    {
                        await channel.SendMessageAsync("You do not seem to have any standings... How strange.");
                    }
                    commandResult = true;
                    SaveUser(targetUser);
                }
                catch
                {
                    await ReplyAsync("I was unable to take standings.  Please see Lambert for further help.");
                }
            }
            else
            {
                var newMember = GetUser(user);
                newMember.NumberOfWarnings++;
                await CheckWarnings(userId, newMember);
                SaveUser(newMember);
                await ReplyAsync("Silly, you do not have the authority to add standings as only Directors and Administrative Consultants are able to add standing.  You have been warned, and you now have " + newMember.NumberOfWarnings + " warnings.");
            }
            ResetAttempts(user);
            await SendLogMessage(user, "Take Standing", commandResult);
        }
        [Command("Set Standing")]
        public async Task setStanding(float newStanding, [Remainder] SocketGuildUser userId)
        {
            bool commandResult = false;
            var user = Context.User as SocketGuildUser;
            if (user == null) return;
            var channel = Context.Guild.GetTextChannel(Epsilon.ConfigFile.SecureSpamChannelID);
            if (user.Id.Equals(Epsilon.ConfigFile.BotMasterID) || user.Roles.Any(x => x.Name.Equals("Administrator", StringComparison.OrdinalIgnoreCase)))
            {
                try
                {
                    var targetUser = GetUser(userId);
                    if (targetUser == null)
                    {
                        await ReplyAsync("I am sorry, but you have failed to provide me with the person to whom you would like to set standings.");
                        return;
                    }
                    if (newStanding >= -10 && newStanding <= 10)
                    {
                        targetUser.PersonalStanding = newStanding;
                    }
                    else
                    {
                        await ReplyAsync("I am sorry, but you have not provided me with a valid standing to change " + targetUser.DiscordUsername + "'s standing with.");
                    }
                    await CheckStanding(userId, targetUser);
                    if (targetUser.PersonalStanding >= 10)
                    {
                        await channel.SendMessageAsync(userId.Mention + ", your standing with the " + Epsilon.ConfigFile.OrganizationName + "has been set to:  " + Math.Round(targetUser.PersonalStanding, 2) + ", and you are trusted here.");
                    }
                    else if (targetUser.PersonalStanding >= 5 && targetUser.PersonalStanding < 10)
                    {
                        await channel.SendMessageAsync(userId.Mention + ", your standing with the " + Epsilon.ConfigFile.OrganizationName + " has been set to:  " + Math.Round(targetUser.PersonalStanding, 2) + ".  You have proven that you can be friendly with others.");
                    }
                    else if (targetUser.PersonalStanding >= 0 && targetUser.PersonalStanding < 5)
                    {
                        await channel.SendMessageAsync(userId.Mention + ", your standing with the " + Epsilon.ConfigFile.OrganizationName + " has been set to:  " + Math.Round(targetUser.PersonalStanding, 2) + ".  You are considered to be neutral.");
                    }
                    else if (targetUser.PersonalStanding < 0 && targetUser.PersonalStanding >= -5)
                    {
                        await channel.SendMessageAsync(userId.Mention + ", your standing with the" + Epsilon.ConfigFile.OrganizationName + " has been set to:  " + Math.Round(targetUser.PersonalStanding, 2) + ", and we are keeping a close eye on you as you are a very suspicious character.");
                    }
                    else if (targetUser.PersonalStanding < -5 && targetUser.PersonalStanding >= -10)
                    {
                        await channel.SendMessageAsync(userId.Mention + ", your standing with the " + Epsilon.ConfigFile.OrganizationName + " has been set to:  " + Math.Round(targetUser.PersonalStanding, 2) + ".  You are considered abhorred.");
                    }
                    else if (targetUser.PersonalStanding < -10)
                    {
                        await channel.SendMessageAsync("Something has gone terribly wrong with your standing and you now have a standing of " + Math.Round(targetUser.PersonalStanding, 2) + " which shouldn't be possible.  Please contact Lambert to correct this.");
                    }
                    commandResult = true;
                    SaveUser(targetUser);
                }
                catch
                {
                    await ReplyAsync("I was unable to add standings.  Please see Lambert for further help.");
                }
            }
            else
            {
                var invalidUser = GetUser(user);
                invalidUser.NumberOfWarnings++;
                await CheckWarnings(user, invalidUser);
                SaveUser(invalidUser);
                await ReplyAsync("Silly, you do not have the authority to add standings as only Administrators are able to add standing.  You have been warned, and you now have " + invalidUser.NumberOfWarnings + " warnings.");
            }
            ResetAttempts(user);
            await SendLogMessage(user, "Set Standing", commandResult);
        }
        [Command("Issue Warning")]
        public async Task issueWarning(SocketGuildUser userId, [Remainder] string warningReason = "")
        {
            bool commandResult = false;
            var user = Context.User as SocketGuildUser;
            if (user == null) return;
            var channel = Context.Guild.GetTextChannel(Epsilon.ConfigFile.AnnouncementChannelID);
            if (user.Id == 151043323379843073 || user.Roles.Any(x => x.Name == "Administrator"))
            {
                var targetUser = GetUser(userId);
                targetUser.NumberOfWarnings += 1;
                await CheckWarnings(userId, targetUser);
                if (warningReason == "")
                {
                    await channel.SendMessageAsync(userId.Mention + " you have been warned by " + user.Username + " and you now have " + targetUser.NumberOfWarnings + " warnings against you. " +
                        "You better watch it.");
                }
                else
                {
                    await channel.SendMessageAsync(userId.Mention + " you have been warned by " + user.Username + " and you now have " + targetUser.NumberOfWarnings + " warnings against you. " +
                        "The reason for the warning was: \n```" + warningReason + "```");
                }
                commandResult = true;
            }
            else
            {
                var invalidUser = GetUser(user);
                invalidUser.NumberOfWarnings++;
                await CheckWarnings(user, invalidUser);
                SaveUser(invalidUser);
                await ReplyAsync("Silly, you do not have the authority to issue warnings.  You have been warned, and you now have " + invalidUser.NumberOfWarnings + " warnings.  " +
                    "Please notify any admin if you wish to report an issue.");
            }
            ResetAttempts(user);
            await SendLogMessage(user, "Issue Warning", commandResult);
        }
        [Command("Remove Warning")]
        public async Task removeWarning(SocketGuildUser userId)
        {
            bool commandResult = false;
            var user = Context.User as SocketGuildUser;
            if (user == null) return;
            var channel = Context.Guild.GetTextChannel(Epsilon.ConfigFile.AnnouncementChannelID);
            if (user.Id == 151043323379843073 || user.Roles.Any(x => x.Name == "Administrator"))
            {
                var targetUser = GetUser(userId);
                if (targetUser == null)
                {
                    await ReplyAsync("I was unable to locate that user.  Try a database update.");
                }
                targetUser.NumberOfWarnings--;
                if (targetUser.NumberOfWarnings < 0)
                {
                    targetUser.NumberOfWarnings = 0;
                    SaveUser(targetUser);
                    await channel.SendMessageAsync(userId.Mention + ", I have removed a warning, and you now have " + targetUser.NumberOfWarnings + " warnings against you.");
                }
                else
                {
                    SaveUser(targetUser);
                    await channel.SendMessageAsync(userId.Mention + ", I have removed a warning, and you now have " + targetUser.NumberOfWarnings + " warnings against you.");
                }
                commandResult = true;
            }
            else
            {
                var invalidUser = GetUser(user);
                invalidUser.NumberOfWarnings++;
                await CheckWarnings(user, invalidUser);
                SaveUser(invalidUser);
                await ReplyAsync("Silly, you do not have the authority to remove a warning.  For this, you will recieve a warning, and you now have " + invalidUser.NumberOfWarnings + " warnings.");
            }
            ResetAttempts(user);
            await SendLogMessage(user, "Remove Warning", commandResult);
        }
        [Command("Reset Warnings")]
        public async Task resetWarnings(SocketGuildUser userId)
        {
            bool commandResult = false;
            var user = Context.User as SocketGuildUser;
            if (user == null) return;
            var channel = Context.Guild.GetTextChannel(468054006661644289);
            if (user.Id == 151043323379843073 || user.Roles.Any(x => x.Name == "Administrator"))
            {
                var targetUser = GetUser(userId);
                targetUser.NumberOfWarnings = 0;
                SaveUser(targetUser);
                await channel.SendMessageAsync(userId.Mention + ", I have reset your warnings to 0.");
                commandResult = true;
            }
            else
            {
                var invalidUser = GetUser(user);
                invalidUser.NumberOfWarnings++;
                await CheckWarnings(user, invalidUser);
                SaveUser(invalidUser);
                await ReplyAsync("Silly, you do not have the authority to issue warnings.  You have been warned, and you now have " + invalidUser.NumberOfWarnings + " warnings.  " +
                    "Please notify any admin if you wish to report an issue.");
            }
            ResetAttempts(user);
            await SendLogMessage(user, "Reset Warnings", commandResult);
        }
        [Command("Add Contact")]
        public async Task addContact(ulong ulongID, float standing, [Remainder] string dualUsername = null)
        {
            bool commandResult = false;
            var user = Context.User as SocketGuildUser;
            if (user.Equals(null)) return;
            if (user.Id.Equals(Epsilon.ConfigFile.BotMasterID) || user.Roles.Any(x => x.Id.Equals(Epsilon.ConfigFile.SeniorOfficerRoleID)))
            {
                var db = new DatabaseContext();
                var newUser = new User();
                newUser.DiscordId = ulongID;
                newUser.DualUsername = dualUsername;
                newUser.PersonalStanding = standing;
                SaveUser(newUser);
                await ReplyAsync("Update complete.");
                commandResult = true;
            }
            else
            {
                var invalidUser = GetUser(user);
                invalidUser.NumberOfWarnings++;
                await CheckWarnings(user, invalidUser);
                SaveUser(invalidUser);
                await ReplyAsync("Silly, you do not have the authority to add a contact as only Administrators are able to do so.  You have been warned, and you now have " + invalidUser.NumberOfWarnings + " warnings.");
            }
            ResetAttempts(user);
            await SendLogMessage(user, "Add Contact", commandResult);
        }
        [Command("Update Contact Dual Username")]
        public async Task updateContactDualUsername(SocketGuildUser socketGuildUser, [Remainder] string dualUsername)
        {
            bool commandResult = false;
            var user = Context.User as SocketGuildUser;
            if (user == null) return;
            if (user.Id == 151043323379843073 || user.Roles.Any(x => x.Name == "Administrator"))
            {
                var db = new DatabaseContext();
                var foundUser = GetUser(socketGuildUser);
                if (foundUser != null)
                {
                    foundUser.DualUsername = dualUsername;
                    SaveUser(foundUser);
                    await ReplyAsync("Update complete.");
                }
                else
                {
                    await ReplyAsync("The Dual username entered was not able to return any contacts.  Please add this contact first.");
                }
                commandResult = true;
            }
            else
            {
                var invalidUser = GetUser(user);
                invalidUser.NumberOfWarnings++;
                await CheckWarnings(user, invalidUser);
                SaveUser(invalidUser);
                await ReplyAsync("Silly, you do not have the authority to update a contact as only Administrators are able to do so.  You have been warned, and you now have " + invalidUser.NumberOfWarnings + " warnings.");
            }
            ResetAttempts(user);
            await SendLogMessage(user, "Update Contact Dual Username", commandResult);
        }
        [Command("Update Contact Standing")]
        public async Task updateContactstanding(SocketGuildUser socketGuildUser, int standing)
        {
            bool commandResult = false;
            var user = Context.User as SocketGuildUser;
            if (user == null) return;
            if (user.Id == 151043323379843073 || user.Roles.Any(x => x.Name == "Administrator"))
            {
                var db = new DatabaseContext();
                var foundUser = GetUser(socketGuildUser);
                if (foundUser != null)
                {
                    foundUser.PersonalStanding = standing;
                    SaveUser(foundUser);
                    await ReplyAsync("Update complete.");
                }
                else
                {
                    await ReplyAsync("The Dual username entered was not able to return any contacts.  Please add this contact first.");
                }
                commandResult = true;
            }
            else
            {
                var invalidUser = GetUser(user);
                invalidUser.NumberOfWarnings++;
                await CheckWarnings(user, invalidUser);
                SaveUser(invalidUser);
                await ReplyAsync("Silly, you do not have the authority to update a contact as only Administrators are able to do so.  You have been warned, and you now have " + invalidUser.NumberOfWarnings + " warnings.");
            }
            ResetAttempts(user);
            await SendLogMessage(user, "Update Contact Standing", commandResult);
        }
        //Faction Commands
        [Command("Enable Join")]
        public async Task enableJoin([Remainder] SocketGuildUser userId)
        {
            bool commandResult = false;
            var user = Context.User as SocketGuildUser;
            if (user == null) return;
            var channel = Context.Guild.GetTextChannel(Epsilon.ConfigFile.AnnouncementChannelID);
            if (user.Id == Epsilon.ConfigFile.BotMasterID || user.Roles.Any(x => x.Name == "Administrator"))
            {
                var updateUser = GetUser(userId);
                updateUser.CanJoin = true;
                SaveUser(updateUser);
                await channel.SendMessageAsync(userId.Username + " has been authorized and can now join " + Epsilon.ConfigFile.OrganizationName + ".");
                commandResult = true;
            }
            else
            {
                var invalidUser = GetUser(user);
                invalidUser.NumberOfWarnings++;
                await CheckWarnings(user, invalidUser);
                SaveUser(invalidUser);
                await ReplyAsync("Silly, you do not have the authority to modify guests as only Administrators are able to do so.  You have been warned, and you now have " + invalidUser.NumberOfWarnings + " warnings.");
            }
            ResetAttempts(user);
            await SendLogMessage(user, "Enable Join", commandResult);
        }
        [Command("Verify Member")]
        public async Task verifyMember(SocketGuildUser userId, string DualUniverseName)
        {
            bool commandResult = false;
            var user = Context.User as SocketGuildUser;
            if (user.Equals(null)) return;
            var channel = Context.Guild.GetTextChannel(Epsilon.ConfigFile.SecureSpamChannelID);
            if (user.Id.Equals(Epsilon.ConfigFile.BotMasterID) || user.Roles.Any(x => x.Id.Equals(Epsilon.ConfigFile.SeniorOfficerRoleID)))
            {
                var dualUser = GetUser(userId);
                if (!dualUser.Equals(null))
                {
                    dualUser.DualUsername = DualUniverseName;
                    SaveUser(dualUser);
                    await ReplyAsync("I have updated " + userId.Username + ", and they are now a full fledged memeber of " + Epsilon.ConfigFile.OrganizationName + ".");
                    commandResult = true;
                }
                else
                {
                    await ReplyAsync("This user does not exist in my database.");
                }
            }
            else
            {
                var invalidUser = GetUser(user);
                invalidUser.NumberOfWarnings++;
                await CheckWarnings(user, invalidUser);
                SaveUser(invalidUser);
                await ReplyAsync("Silly, you do not have the authority to verify new recruits as only Administrators are able to do so.  You have been warned, and you now have " + invalidUser.NumberOfWarnings + " warnings.");
            }
            ResetAttempts(user);
            await SendLogMessage(user, "Verify Member", commandResult);
        }
        [Command("Discharge Member")]
        public async Task dischargeMember(string type, [Remainder] SocketGuildUser socketGuildUser)
        {
            bool commandResult = false;
            var user = Context.User as SocketGuildUser;
            if (user == null) return;
            var channel = Context.Guild.GetTextChannel(468054006661644289);
            if (user.Roles.Any(x => x.Name == "Administrator") || user.Id == 151043323379843073)
            {
                var updateUser = GetUser(socketGuildUser);
                updateUser.PersonalStanding = 0;
                updateUser.FactionJoinDate = null;
                updateUser.PromotionDate = null;
                updateUser.PromotionPointBalance = 0;
                //updateUser.DaysUntilPromotion = 0;
                //updateUser.CompletedMissions = 0;
                updateUser.Grade = "E00";
                updateUser.Rank = type + " Discharge";
                updateUser.CanJoin = false;
                updateUser.JoinedFaction = false;
                SaveUser(updateUser);
                await ReplyAsync("I have discharged member " + socketGuildUser + " with a " + type + " discharge.");
                commandResult = true;
            }
            else
            {
                var invalidUser = GetUser(user);
                invalidUser.NumberOfWarnings++;
                await CheckWarnings(user, invalidUser);
                SaveUser(invalidUser);
                await ReplyAsync("Silly, you do not have the authority to modify a member as only Administrators are able to do so.  You have been warned, and you now have " + invalidUser.NumberOfWarnings + " warnings.");
            }
            ResetAttempts(user);
            await SendLogMessage(user, "Discharge Member", commandResult);
        }
        [Command("Add Points")]
        public async Task addPoints(int pointAmmount, SocketGuildUser socketGuildUser)
        {
            bool commandResult = false;
            var user = Context.User as SocketGuildUser;
            if (user == null) return;
            var targetUser = GetUser(socketGuildUser);
            targetUser.PromotionPointBalance += pointAmmount;
            SaveUser(targetUser);
            await ReplyAsync("I have added " + pointAmmount + " points to " + socketGuildUser.Username + ".");
            commandResult = true;
            await SendLogMessage(user, "Add Points", commandResult);
        }
        //Methods
        private void ResetAttempts(SocketGuildUser user)
        {
            var guest = GetUser(user);
            guest.NumberOfAttempts = 0;
            SaveUser(guest);
        }
        private User GetUser(SocketGuildUser user)
        {
            var db = new DatabaseContext();
            try
            {
                return db.Users.Single(x => x.DiscordId == user.Id);
            }
            catch (Exception e)
            {
                Console.WriteLine("User failed to retrieve from the database. " + e.Message);
                return null;
            }
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
        private async Task SetStandingRole(SocketGuildUser user, string roleTitle)
        {
            var currentStandingRole = user.Roles.FirstOrDefault(x => Epsilon.StandingRolesList.Any(t => t.Equals(x.Name, StringComparison.OrdinalIgnoreCase)));
            var newStandingRole = user.Guild.Roles.FirstOrDefault(x => x.Name.Equals(roleTitle, StringComparison.OrdinalIgnoreCase));
            if (currentStandingRole != null)
            {
                await user.RemoveRoleAsync(currentStandingRole);
            }
            await user.AddRoleAsync(newStandingRole);
        }
        private async Task SetRaiderRole(SocketGuildUser user, string grade)
        {
            var targetUser = GetUser(user);
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
                SaveUser(targetUser);
            }
        }
        private async Task SendLogMessage(SocketGuildUser userID, string commandName, bool commandResult)
        {
            var resultString = string.Empty;
            var channel = Context.Guild.GetTextChannel(Epsilon.ConfigFile.BotLogChannelID);
            if (commandResult)
            {
                resultString = "successfully";
                if (channel != null)
                {
                    await channel.SendMessageAsync("```" + userID.ToString() + " " + resultString + " used the command " + commandName + " at " + DateTimeOffset.UtcNow + " UTC. ```");
                }
            }
            else if (!commandResult)
            {
                resultString = "unsuccessfully";
                if (channel != null)
                {
                    await channel.SendMessageAsync("```" + userID.ToString() + " " + resultString + " used the command " + commandName + " at " + DateTimeOffset.UtcNow + " UTC. ```");
                }
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
        private async Task CheckWarnings(SocketGuildUser user, User guest)
        {
            if (guest.NumberOfWarnings >= 3)
            {
                guest.PersonalStanding = ((-10 - guest.PersonalStanding) * 0.1F) + guest.PersonalStanding;
                await CheckStanding(user, guest);
                guest.NumberOfWarnings = 0;
                await ReplyAsync(guest.DiscordUsername + ", you have reached 3 warnings and I dropped your standing.  Your warnings are back to 0.  You " +
                    "now have a standing of:  " + Math.Round(guest.PersonalStanding, 2) + ".");
                SaveUser(guest);
            }
            else if (guest.NumberOfWarnings < 3)
            {
                SaveUser(guest);
            }
        }
        private async Task CheckAttempts(SocketGuildUser user, User guest)
        {
            if (guest.NumberOfAttempts >= 10)
            {
                guest.NumberOfAttempts = 0;
                guest.NumberOfWarnings += 1;
                SaveUser(guest);
                await ReplyAsync(guest.DiscordUsername + ", you have failed to use a proper command 10 times in a row and you now have a warning against you.  " +
                    "Your amount of warnings is " + guest.NumberOfWarnings + ".  You " +
                    "now have a standing of:  " + Math.Round(guest.PersonalStanding, 2) + ".");
                SaveUser(guest);
            }
            else if (guest.NumberOfWarnings < 3)
            {
                SaveUser(guest);
            }
        }
    }
}

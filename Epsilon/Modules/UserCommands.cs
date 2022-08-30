using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord;
using Discord.Commands;
using Discord.Addons.Interactive;
using System.IO;
using System.Globalization;
using Newtonsoft.Json;

namespace Epsilon.Modules
{
    public class UserCommands : InteractiveBase
    {
        private List<string> JobTypeList = new List<string>();
        private List<string> JobClassList = new List<string>();
        private readonly DatabaseContext db = new DatabaseContext();
        private bool commandResult = false;
    //General User Commands
        [Command("Check Standing")]
        [Alias("Check Standings")]
        public async Task checkStanding([Remainder] SocketGuildUser targetUser = null)
        {
            var user = Context.User as SocketGuildUser;
            if (user == null) return;
            if (targetUser == null)
            {
                var askingUser = GetUser(user, db);
                await CheckStanding(user, askingUser);
                if (askingUser.PersonalStanding >= 10)
                {
                    await ReplyAsync("Your standing with " + Epsilon.ConfigFile.OrganizationName + " is:  " + Math.Round(askingUser.PersonalStanding, 2) + ".");
                }
                else if (askingUser.PersonalStanding >= 5 && askingUser.PersonalStanding < 10)
                {
                    await ReplyAsync("Your standing with " + Epsilon.ConfigFile.OrganizationName + " is:  " + Math.Round(askingUser.PersonalStanding, 2) + ".");
                }
                else if (askingUser.PersonalStanding >= 0 && askingUser.PersonalStanding < 5)
                {
                    await ReplyAsync("Your standing with " + Epsilon.ConfigFile.OrganizationName + " is:  " + Math.Round(askingUser.PersonalStanding, 2) + ".");
                }
                else if (askingUser.PersonalStanding < 0 && askingUser.PersonalStanding >= -5)
                {
                    await ReplyAsync("Your standing with " + Epsilon.ConfigFile.OrganizationName + " is:  " + Math.Round(askingUser.PersonalStanding, 2) + ".");
                }
                else if (askingUser.PersonalStanding < -5 && askingUser.PersonalStanding >= -10)
                {
                    await ReplyAsync("Your standing with " + Epsilon.ConfigFile.OrganizationName + " is:  " + Math.Round(askingUser.PersonalStanding, 2) + ".");
                }
                else if (askingUser.PersonalStanding < -10)
                {
                    await ReplyAsync("It appears something has gone terribly wrong with your standing.  Please see an administrator to correct this" +
                        " issue.  Your current standing is:  " + Math.Round(askingUser.PersonalStanding, 2) + ".");
                }
                SaveUser(askingUser, db);
            }
            else if (targetUser != null)
            {
                var lookUpUser = GetUser(targetUser, db);
                await CheckStanding(targetUser, lookUpUser);
                if (lookUpUser.PersonalStanding >= 10)
                {
                    await ReplyAsync(targetUser.Username + " has a standing of:  " + Math.Round(lookUpUser.PersonalStanding, 2) + ", and is trusted.");
                }
                else if (lookUpUser.PersonalStanding >= 5 && lookUpUser.PersonalStanding < 10)
                {
                    await ReplyAsync(targetUser.Username + " has a standing of:  " + Math.Round(lookUpUser.PersonalStanding, 2) + ", and is friendly.");
                }
                else if (lookUpUser.PersonalStanding >= 0 && lookUpUser.PersonalStanding < 5)
                {
                    await ReplyAsync(targetUser.Username + " has a standing of:  " + Math.Round(lookUpUser.PersonalStanding, 2) + ", and is neutral.");
                }
                else if (lookUpUser.PersonalStanding >= -5 && lookUpUser.PersonalStanding < 0)
                {
                    await ReplyAsync(targetUser.Username + " has a standing of:  " + Math.Round(lookUpUser.PersonalStanding, 2) + ", and is suspect.");
                }
                else if (lookUpUser.PersonalStanding >= -10 && lookUpUser.PersonalStanding < -5)
                {
                    await ReplyAsync(targetUser.Username + " has a standing of:  " + Math.Round(lookUpUser.PersonalStanding, 2) + ", and is a criminal.");
                }
                else if (lookUpUser.PersonalStanding < -10)
                {
                    await ReplyAsync("It appears something has gone terribly wrong with " + lookUpUser.DiscordUsername + "'s standing.  Please see an " +
                        "administrator to correct this issue.  Their current standing is:  " + Math.Round(lookUpUser.PersonalStanding, 2) + ".");
                }
            }
            commandResult = true;
            ResetAttempts(user, db);
            await SendLogMessage(user, "Check Standing", commandResult);
        }
        [Command("Check Warnings")]
        [Alias("Check Warning")]
        public async Task checkWarnings(SocketGuildUser userId = null)
        {
            var user = Context.User as SocketGuildUser;
            if (user == null) return;
            if (userId == null)
            {
                var asking = GetUser(user, db);
                await ReplyAsync("The amount of warnings against you is:  " + asking.NumberOfWarnings + ".");
            }
            else
            {
                var lookup = GetUser(userId, db);
                await ReplyAsync("The amount of warnins against " + userId.Username + " is:  " + lookup.NumberOfWarnings + ".");
            }
            commandResult = true;
            ResetAttempts(user, db);
            await SendLogMessage(user, "Check Warnings", commandResult);
        }
        [Command("Check Attempts")]
        public async Task checkAttempts(SocketGuildUser userId = null)
        {
            var user = Context.User as SocketGuildUser;
            if (user == null) return;
            if (userId == null)
            {
                var asking = GetUser(user, db);
                await ReplyAsync("I have the amount of command attempts you have tried to use so far as:  "
                    + asking.NumberOfAttempts + ".  Remember after 3 failed attempts a warning is issued.");
            }
            else
            {
                var lookup = GetUser(userId, db);
                await ReplyAsync(lookup.DiscordUsername + " has failed to type a proper command "
                    + lookup.NumberOfAttempts + "times.  After 3 failed attmeps I will issue a warning.");
            }
            commandResult = true;
            ResetAttempts(user, db);
            await SendLogMessage(user, "Check Attempts", commandResult);
        }
        //Faction Commands
        [Command("Enlist", RunMode = RunMode.Async)]
        public async Task enlist()
        {
            commandResult = false;
            var user = Context.User as SocketGuildUser;
            if (user.Equals(null)) return;
            await ReplyAsync(":warning: You are attempting to enlist into the " + Epsilon.ConfigFile.OrganizationName + "'s military. :warning:  \nDoing so may result in a loss of " +
                "standing with other Organizations.  By enlisting into the military, you also agree that by leaving the military and/or the Organization before the end of " +
                "your active duty is completed that this is a courtmartialiable offence and you will lose standing with " + Epsilon.ConfigFile.OrganizationName + ".  This results in a " +
                "dishonerable discharge, and you will not be allowed back.  Do you still wish to proceed?");
            var response = await NextMessageAsync();
            if (response != null && response.Content.Equals("yes", StringComparison.OrdinalIgnoreCase))
            {
                var newRecruit = GetUser(user, db);
                if (newRecruit.CanJoin && !newRecruit.JoinedFaction)
                {
                    var newKey = KeyGenerator.GetUniqueKey(32);
                    var roleMember = user.Guild.Roles.Where(x => x.Name.ToUpper() == "Member".ToUpper());
                    var roleGuest = user.Guild.Roles.Where(x => x.Name.ToUpper() == "Guest".ToUpper());
                    await user.AddRolesAsync(roleMember);
                    await user.RemoveRolesAsync(roleGuest);
                    var adminRole = user.Guild.GetRole(Epsilon.ConfigFile.AdministratorRoleID);
                    var channel = user.Guild.GetTextChannel(Epsilon.ConfigFile.SecureSpamChannelID);
                    await channel.SendMessageAsync(adminRole.Mention + $", {user.Username} has applied to the military, and their unique code is:  \n" +
                        "```" + newKey + "``` .");
                    var dMChannel = await user.CreateDMChannelAsync();
                    await dMChannel.SendMessageAsync(user.Username + " your unique verification code is:  \n" +
                        "```" + newKey + "``` \n" +
                        "Please send this to an Administrator in a private message for verification.");
                    newRecruit.VerificationKey = newKey;
                    newRecruit.FactionJoinDate = DateTimeOffset.UtcNow;
                    newRecruit.PromotionDate = DateTimeOffset.UtcNow.AddDays(30);
                    newRecruit.PromotionPointBalance = 0;
                    newRecruit.PersonalStanding = 10;
                    newRecruit.DiscordId = user.Id;
                    newRecruit.DiscordUserID = user.ToString();
                    newRecruit.DiscordUsername = user.Username;
                    newRecruit.Rank = "Recruit";
                    newRecruit.Grade = "E00";
                    newRecruit.JoinedFaction = true;
                    SaveUser(newRecruit, db);
                    await SetRank(user, newRecruit.Grade, db);
                    await SetStandingRole(user, "Trusted");
                    await ReplyAsync("Congragulations " + user.Username + "!  " +
                        "When your 30 day probationary period is over, you will be a full member of " + Epsilon.ConfigFile.OrganizationName + ".  " +
                        "Your grade and rank is " + newRecruit.Grade + " " + newRecruit.Rank + ".  Welcome!");
                }
                else if (newRecruit.CanJoin && newRecruit.JoinedFaction)
                {
                    await ReplyAsync("It seems you have already joined the military and there is nothing more I can do at this time for you. :face_palm:");
                }
                else
                {
                    await ReplyAsync("You have not been authorized to enlist at this time.  If you beilieve this to be in error, please contact " + Context.Guild.Owner.Username + ".");
                }
                commandResult = true;
            }
            else if (response != null && response.Content.Equals("no", StringComparison.OrdinalIgnoreCase))
            {
                await ReplyAsync("Okay, you will NOT be enlisted into the military for " + Epsilon.ConfigFile.OrganizationName + ".  Perhaps you wish to join their civilian secor " +
                    "instead?  If you wish to do this you can use the command \"~join\" instead of \"enlist\".");
                commandResult = true;
            }
            else
            {
                await ReplyAsync("Your response is invalid.  Please try again later when you have aquired enough brain cells to use this command correctly.");
            }
            ResetAttempts(user, db);
            await SendLogMessage(user, "Enlist", commandResult);
        }
        [Command("Join", RunMode = RunMode.Async)]
        public async Task join()
        {
            commandResult = false;
            var user = Context.User as SocketGuildUser;
            if (user == null) return;
            await ReplyAsync(":warning: You are about to officially submit your application for citizenship :warning: of " + Epsilon.ConfigFile.OrganizationName + ".  In doing so, your standing " +
                "with other groups may decrease.  If you choose to revoke your citizenship, you may do so at any time.  This will obviously result " +
                "in a negative standing with " + Epsilon.ConfigFile.OrganizationName + ".  Do you wish to proceed?");
            var response = await NextMessageAsync(true, true, TimeSpan.FromSeconds(Epsilon.ConfigFile.ResponseTimeTimeout));
            if (response != null && response.Content.Equals("yes", StringComparison.OrdinalIgnoreCase))
            {
                var newCitizen = GetUser(user, db);
                if (newCitizen.CanJoin)
                {
                    if (!newCitizen.JoinedFaction)
                    {
                        var newKey = KeyGenerator.GetUniqueKey(32);
                        var roleMember = user.Guild.Roles.Where(x => x.Name.Equals("member", StringComparison.OrdinalIgnoreCase));
                        var roleGuest = user.Guild.Roles.Where(x => x.Name.Equals("guest", StringComparison.OrdinalIgnoreCase));
                        await user.AddRolesAsync(roleMember);
                        await user.RemoveRolesAsync(roleGuest);
                        await ReplyAsync("Congragulations " + user.Username + "!  " +
                            "When your probationary period is over, you will be a member of " + Epsilon.ConfigFile.OrganizationName + ".  " +
                            "Your rank is \"Trainee\".  Welcome!");
                        var adminRole = user.Guild.GetRole(Epsilon.ConfigFile.AdministratorRoleID);
                        var channel = user.Guild.GetTextChannel(Epsilon.ConfigFile.SecureSpamChannelID);
                        await channel.SendMessageAsync(adminRole.Mention + $", {user.Username} has applied, and their unique code is:  \n" +
                            "```" + newKey + "```");
                        var dMChannel = await user.CreateDMChannelAsync();
                        await dMChannel.SendMessageAsync(user.Username + " your unique verification code is:  ```" + newKey + "```  please private message " +
                            "this to Lambert in game. :warning: DO NOT SEND THIS IN A DISCORD DM!!! :warning:");
                        newCitizen.VerificationKey = newKey;
                        newCitizen.FactionJoinDate = DateTimeOffset.UtcNow;
                        newCitizen.PromotionDate = DateTimeOffset.UtcNow.AddDays(30);
                        newCitizen.PromotionPointBalance = 0;
                        //newCitizen.DaysUntilPromotion = 30;
                        newCitizen.PersonalStanding = 10;
                        newCitizen.DiscordUserID = user.ToString();
                        newCitizen.DiscordUsername = user.Username;
                        newCitizen.Rank = "Trainee";
                        newCitizen.Grade = "C00";
                        newCitizen.JoinedFaction = true;
                        SaveUser(newCitizen, db);
                        await SetRank(user, newCitizen.Grade, db);
                        await SetStandingRole(user, "Trusted");
                    }
                    else
                    {
                        await ReplyAsync("You are already a member of " + Epsilon.ConfigFile.OrganizationName + ".  :man_facepalming:");
                    }
                    commandResult = true;
                }
                else
                {
                    await ReplyAsync("You are not authorized to join " + Epsilon.ConfigFile.OrganizationName + ".  If you beilieve this to be in error, please contact " + Context.Guild.Owner.Username + ".");
                }
            }
            else
            {
                await ReplyAsync("Timeout error:  No response found.");
            }
            await SendLogMessage(user, "Join", commandResult);
        }
    //Minecraft
        [Command("Minecraft Add Location")]
        public async Task MCAddLocation(int xCoordinate, int zCoordinate, int yCoordinate, [Remainder] string thingFound)
        {
            commandResult = false;
            var user = Context.User as SocketGuildUser;
            if (user.Equals(null)) return;
            var discoverer = GetUser(user, db);
            var newLocation = new MCLocation();
            newLocation.DateCreated = DateTimeOffset.UtcNow;
            newLocation.XCoordinate = xCoordinate;
            newLocation.ZCoordinate = zCoordinate;
            newLocation.YCoordinate = yCoordinate;
            newLocation.Discoverer = user.Id;
            newLocation.ThingFound = thingFound;
            SaveMCLocation(newLocation, db);
            await ReplyAsync("The location was successfully added to the database.");
            commandResult = true;
            await SendLogMessage(user, "Minecraft Add Location", commandResult);
        }
    //Methods
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
            else if (guest.PersonalStanding >= 5 && guest.PersonalStanding < 10)
            {
                await SetStandingRole(user, "Friendly");
            }
            else if (guest.PersonalStanding >= 0 && guest.PersonalStanding < 5)
            {
                await SetStandingRole(user, "Neutral");
            }
            else if (guest.PersonalStanding >= -5 && guest.PersonalStanding < 0)
            {
                await SetStandingRole(user, "Suspect");
            }
            else if (guest.PersonalStanding >= -10 && guest.PersonalStanding < -5)
            {
                await SetStandingRole(user, "Criminal");
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
        private void SaveUser(User guest , DatabaseContext db)
        {
            try
            {
                db.Users.Update(guest);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine("User failed to retrieve. " + e.Message);
            }
        }
        private void SaveMCLocation(MCLocation mCLocation, DatabaseContext db)
        {
            try
            {
                db.MCLocations.Update(mCLocation);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine("Minecraft Locations failed to save an instance to the database.  " + e.Message);
                var role = Context.Guild.GetRole(Epsilon.ConfigFile.SeniorOfficerRoleID);
                Context.Channel.SendMessageAsync(role.Mention + ", please check the console for the issue with the database.");
            }
        }
        private User GetUser(SocketGuildUser userID, DatabaseContext db)
        {
            try
            {
                return db.Users.Single(x => x.DiscordId == userID.Id);
            }
            catch (Exception e)
            {
                Console.WriteLine("The database failed to return an instance of an User with the username of " + userID.Username + ". " + e.Message +
                    "\n" + "No further action is required at this time.");
                return null;
            }
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
                if (targetUser.Grade[0].Equals('C'))
                {
                    targetUser.Branch = "Civilian";
                }
                else
                {
                    targetUser.Branch = "Military";
                }
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
        private async Task CheckWarnings(SocketGuildUser user, User guest, DatabaseContext db)
        {
            if (guest.NumberOfWarnings >= 3)
            {
                guest.PersonalStanding = ((-10 - guest.PersonalStanding) * 0.1F) + guest.PersonalStanding;
                await CheckStanding(user, guest);
                guest.NumberOfWarnings = 0;
                await ReplyAsync(user.Username + ", you have reached 3 warnings and I dropped your standing.  Your warnings are back to 0.  You " +
                    "now have a standing of:  " + Math.Round(guest.PersonalStanding, 2) + ".");
                SaveUser(guest, db);
            }
            else if (guest.NumberOfWarnings < 3)
            {
                guest.NumberOfWarnings++;
                SaveUser(guest, db);
            }
        }
        private void ResetAttempts(SocketGuildUser user, DatabaseContext db)
        {
            var guest = GetUser(user, db);
            guest.NumberOfAttempts = 0;
            SaveUser(guest, db);
        }
    }
}

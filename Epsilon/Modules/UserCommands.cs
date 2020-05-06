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

namespace Epsilon.Modules
{
    public class UserCommands : InteractiveBase
    {
        private List<string> GameNamesList = new List<string>();
        private List<string> JobTypeList = new List<string>();
        private List<string> JobClassList = new List<string>();
        //Channel Permission Commands
        [Command("Get Access")]
        public async Task gainAccess([Remainder] string gameName)
        {
            var user = Context.User as SocketGuildUser;
            if (user == null) return;
            if (gameName == null)
            {
                await ReplyAsync("You have not provided me with the name to which channels you would like to access.");
            }
            else
            {
                GetGameList();
                if (GameNamesList.Any(x => x.Equals(gameName, StringComparison.OrdinalIgnoreCase)))
                {
                    var gameRole = user.Guild.Roles.FirstOrDefault(x => x.Name.Equals(gameName, StringComparison.OrdinalIgnoreCase));
                    await user.AddRoleAsync(gameRole);
                    await ReplyAsync("I have given you access.");
                }
            }
            ResetAttempts(user);
            await SendLogMessage(user, "Get Access");
        }
        //General User Commands
        [Command("Check Standing")]
        [Alias("Check Standings")]
        public async Task checkStanding([Remainder] SocketGuildUser targetUser = null)
        {
            var user = Context.User as SocketGuildUser;
            if (user == null) return;
            if (targetUser == null)
            {
                var askingUser = GetUser(user);
                await CheckStanding(user, askingUser);
                if (askingUser.PersonalStanding >= 10)
                {
                    await ReplyAsync("Your standing with " + Epsilon.OrganizationName + " is:  " + Math.Round(askingUser.PersonalStanding, 2) + ".");
                }
                else if (askingUser.PersonalStanding >= 5 && askingUser.PersonalStanding < 10)
                {
                    await ReplyAsync("Your standing with " + Epsilon.OrganizationName + " is:  " + Math.Round(askingUser.PersonalStanding, 2) + ".");
                }
                else if (askingUser.PersonalStanding >= 0 && askingUser.PersonalStanding < 5)
                {
                    await ReplyAsync("Your standing with " + Epsilon.OrganizationName + " is:  " + Math.Round(askingUser.PersonalStanding, 2) + ".");
                }
                else if (askingUser.PersonalStanding < 0 && askingUser.PersonalStanding >= -5)
                {
                    await ReplyAsync("Your standing with " + Epsilon.OrganizationName + " is:  " + Math.Round(askingUser.PersonalStanding, 2) + ".");
                }
                else if (askingUser.PersonalStanding < -5 && askingUser.PersonalStanding >= -10)
                {
                    await ReplyAsync("Your standing with " + Epsilon.OrganizationName + " is:  " + Math.Round(askingUser.PersonalStanding, 2) + ".");
                }
                else if (askingUser.PersonalStanding < -10)
                {
                    await ReplyAsync("It appears something has gone terribly wrong with your standing.  Please see an administrator to correct this" +
                        " issue.  Your current standing is:  " + Math.Round(askingUser.PersonalStanding, 2) + ".");
                }
                SaveUser(askingUser);
            }
            else if (targetUser != null)
            {
                var lookUpUser = GetUser(targetUser);
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
                    await ReplyAsync("It appears something has gone terribly wrong with " + lookUpUser.Username + "'s standing.  Please see an " +
                        "administrator to correct this issue.  Their current standing is:  " + Math.Round(lookUpUser.PersonalStanding, 2) + ".");
                }
            }
            ResetAttempts(user);
            await SendLogMessage(user, "Check Standing");
        }
        [Command("Check Warnings")]
        [Alias("Check Warning")]
        public async Task checkWarnings(SocketGuildUser userId = null)
        {
            var user = Context.User as SocketGuildUser;
            if (user == null) return;
            if (userId == null)
            {
                var asking = GetUser(user);
                await ReplyAsync("The amount of warnings against you is:  " + asking.NumberOfWarnings + ".");
            }
            else
            {
                var lookup = GetUser(userId);
                await ReplyAsync("The amount of warnins against " + userId.Username + " is:  " + lookup.NumberOfWarnings + ".");
            }
            ResetAttempts(user);
            await SendLogMessage(user, "Check Warnings");
        }
        [Command("Check Attempts")]
        public async Task checkAttempts(SocketGuildUser userId = null)
        {
            var user = Context.User as SocketGuildUser;
            if (user == null) return;
            if (userId == null)
            {
                var asking = GetUser(user);
                await ReplyAsync("I have the amount of command attempts you have tried to use so far as:  "
                    + asking.NumberOfAttempts + ".  Remember after 3 failed attempts a warning is issued.");
            }
            else
            {
                var lookup = GetUser(userId);
                await ReplyAsync(lookup.Username + " has failed to type a proper command "
                    + lookup.NumberOfAttempts + "times.  After 3 failed attmeps I will issue a warning.");
            }
            ResetAttempts(user);
            await SendLogMessage(user, "Check Attempts");
        }
        [Command("Time")]
        public async Task time(string time = null)
        {
            var user = Context.User as SocketGuildUser;
            if (user == null) return;
            if (time != null)
            {
                try
                {
                    
                    await ReplyAsync("The time entered is:  UTC.");
                }
                catch (Exception e)
                {
                    await ReplyAsync("Unable to complete command \"Time\" properly." + e.Message);
                }
            }
            else
            {
                await ReplyAsync("The current time on deck is:  " + DateTimeOffset.UtcNow.TimeOfDay + " UTC");
            }
            ResetAttempts(user);
            await SendLogMessage(user, "Time");
        }
        [Command("Flight Time")]
        [Alias("FT")]
        public async Task flightTime(float distance, string distanceMeasurement, float rate)
        {
            var user = Context.User as SocketGuildUser;
            if (user == null) return;
            float time = 0;
            if (distance > 0 && rate > 0 && distanceMeasurement.Equals("SU", StringComparison.OrdinalIgnoreCase))
            {
                distance = distance * 200;
                time = distance / rate;
                if (time < 1)
                {
                    double minuets = Math.Floor(time * 60);
                    double seconds = Math.Round((time - (minuets/60)) * 3600);
                    await ReplyAsync("You will arrive in aproximately " + minuets + " minuets, and " + seconds + " seconds.  Fly safe!");
                }
                else
                {
                    double hours = Math.Floor(time);
                    double minuets = Math.Floor((time - hours) * 60);
                    double seconds = Math.Round(((time - hours) * 60 - Math.Floor(minuets)) * 60);
                    await ReplyAsync("You will arrive in aproximately " + hours + " hours, " + minuets + " minuets, and " + seconds + " seconds.  Fly safe!");
                }
            }
            else if (distance > 0 && rate > 0 && distanceMeasurement.Equals("KM", StringComparison.OrdinalIgnoreCase))
            {
                time = distance / rate;
                if (time < 1)
                {

                }
            }
        }
        //Faction Commands
        /*[Command("Enlist", RunMode = RunMode.Async)]
        public async Task enlist()
        {
            var user = Context.User as SocketGuildUser;
            if (user == null) return;
            await ReplyAsync(":warning: You are about to enlist into the " + Epsilon.OrganizationName + "'s military. :warning:  Doing so may result in a loss of " +
                "standing with other groups.  By enlisting you also agree that leaving the military before the term of your active duty is completed " +
                "is a courtmartialiable offence and will result in a loss of standing with " + Epsilon.OrganizationName + " and a dishonerable discharge.  Do you still " +
                "wish to proceed?");
            var response = await NextMessageAsync();
            if (response != null && response.Content.Equals("yes", StringComparison.OrdinalIgnoreCase))
            {
                var newRecruit = GetUser(user);
                if (newRecruit.JoinedFaction == false)
                {
                    var newKey = KeyGenerator.GetUniqueKey(32);
                    var roleMember = user.Guild.Roles.Where(x => x.Name.ToUpper() == "Member".ToUpper());
                    var roleGuest = user.Guild.Roles.Where(x => x.Name.ToUpper() == "Guest".ToUpper());
                    await user.AddRolesAsync(roleMember);
                    await user.RemoveRolesAsync(roleGuest);
                    await ReplyAsync("Congragulations " + user.Username + "!  " +
                        "When your probationary period is over, you will be a full member of " + Epsilon.OrganizationName + ".  " +
                        "Your rank is \"Recruit\".  Welcome!");
                    var adminRole = user.Guild.GetRole(Epsilon.AdminID);
                    var channel = user.Guild.GetTextChannel(Epsilon.SecureChannelID);
                    await channel.SendMessageAsync(adminRole.Mention + $", {user.Username} has applied to the military, and their unique code is:  \n" +
                        "```" + newKey + "``` .");
                    var dMChannel = await user.GetOrCreateDMChannelAsync();
                    await dMChannel.SendMessageAsync(user.Username + " your unique verification code is:  \n" +
                        "```" + newKey + "``` \n" +
                        "Please send this to an Administrator in a private message for verification.");
                    newRecruit.VerificationKey = newKey;
                    newRecruit.FactionJoinDate = DateTimeOffset.UtcNow;
                    newRecruit.PromotionDate = DateTimeOffset.UtcNow;
                    newRecruit.PointBalance = 0;
                    newRecruit.DaysUntilPromotion = 30;
                    newRecruit.PersonalStanding = 10;
                    newRecruit.DiscordId = user.Id;
                    newRecruit.UserID = user.ToString();
                    newRecruit.Username = user.Username;
                    newRecruit.Branch = "Military";
                    newRecruit.Rank = "Recruit";
                    newRecruit.Grade = "E00";
                    newRecruit.JoinedFaction = true;
                    await SetStandingRole(user, "Trusted");
                    SaveUser(newRecruit);
                }
                else
                {
                    await ReplyAsync("You have already joined and there is nothing more I can do at this time for you. :face_palm:");
                }
            }
            else if (response != null && response.Content.Equals("no", StringComparison.OrdinalIgnoreCase))
            {
                await ReplyAsync("Okay, I will not enlist you into the military for " + Epsilon.OrganizationName + ".  Perhaps you wish to join their civilian secor " +
                    "instead?  If you wish to do this you simply need to 'join' instead of 'enlist'.");
            }
            else
            {
                await ReplyAsync("Your response is invalid.  Please try again later when you have aquired enough brain cells to try again.");
            }
            ResetAttempts(user);
            await SendLogMessage(user, "Enlist");
        }*/
        [Command("Join", RunMode = RunMode.Async)]
        public async Task join()
        {
            var user = Context.User as SocketGuildUser;
            if (user == null) return;
            await ReplyAsync(":warning: You are about to officially submit your application for citizenship :warning: of " + Epsilon.OrganizationName + ".  In doing so, your standing " +
                "with other groups may decrease.  If you choose to revoke your citizenship, you may do so at any time.  This will obviously result " +
                "in a negative standing with " + Epsilon.OrganizationName + ".  Do you wish to proceed?");
            var response = await NextMessageAsync(true, true, TimeSpan.FromSeconds(Epsilon.TimeoutTimeLimit));
            if (response != null && response.Content.Equals("yes", StringComparison.OrdinalIgnoreCase))
            {
                var newCitizen = GetUser(user);
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
                            "When your probationary period is over, you will be a member of " + Epsilon.OrganizationName + ".  " +
                            "Your rank is \"Trainee\".  Welcome!");
                        var adminRole = user.Guild.GetRole(Epsilon.AdminID);
                        var channel = user.Guild.GetTextChannel(Epsilon.SecureChannelID);
                        await channel.SendMessageAsync(adminRole.Mention + $", {user.Username} has applied, and their unique code is:  \n" +
                            "```" + newKey + "``` .");
                        var dMChannel = await user.GetOrCreateDMChannelAsync();
                        await dMChannel.SendMessageAsync(user.Username + " your unique verification code is:  ```" + newKey + "```  please private message " +
                            "this to an Administrator for processing.");
                        newCitizen.VerificationKey = newKey;
                        newCitizen.FactionJoinDate = DateTimeOffset.UtcNow;
                        newCitizen.PromotionDate = DateTimeOffset.UtcNow;
                        newCitizen.PointBalance = 0;
                        newCitizen.DaysUntilPromotion = 30;
                        newCitizen.PersonalStanding = 10;
                        newCitizen.UserID = user.ToString();
                        newCitizen.Username = user.Username;
                        newCitizen.Branch = "Civilian";
                        newCitizen.Rank = "Trainee";
                        newCitizen.Grade = "C00";
                        newCitizen.JoinedFaction = true;
                        await SetStandingRole(user, "Trusted");
                        SaveUser(newCitizen);
                    }
                    else
                    {
                        await ReplyAsync("You are already a member of " + Epsilon.OrganizationName + ".  :face_palm:");
                    }
                }
                else
                {
                    await ReplyAsync("You are not able to join " + Epsilon.OrganizationName + ".  Please wait while this issue is corrected.");
                }
            }
            else
            {
                await ReplyAsync("Timeout error:  No response found.");
            }
        }
        [Command("DU Queue Part")]
        public async Task queuePart(int qty, [Remainder] string partName)
        {
            var user = Context.User as SocketGuildUser;
            if (user == null) return;
            var askingUser = GetUser(user);
            var existingIndustry = CheckIndustry(partName);
            if (/*askingUser.JoinedFaction == true && */askingUser.NDAVerified == true)
            {
                if (existingIndustry == null)
                {
                    var newIndustryRequest = new Industry();
                    newIndustryRequest.Username = user.Username;
                    newIndustryRequest.Quantity = qty;
                    newIndustryRequest.PartName = partName;
                    updateIndustry(newIndustryRequest);
                    await ReplyAsync("I have queued " + qty + " amount of " + partName + ".");
                }
                else
                {
                    existingIndustry.Quantity += qty;
                    updateIndustry(existingIndustry);
                    await ReplyAsync("I have queued " + qty + " amount of " + partName + ".");
                }
                IndustrySheetUpdate.runUpdate();
            }
            else
            {
                await ReplyAsync("This feature is unavailable to you at this time.");
            }
            await SendLogMessage(user, "DU Queue Part");
            ResetAttempts(user);
        }
        [Command("DU Crafted Part")]
        public async Task craftedPart(int qty, [Remainder] string partName)
        {
            var user = Context.User as SocketGuildUser;
            if (user == null) return;
            var askingUser = GetUser(user);
            var existingIndustry = CheckIndustry(partName);
            if (askingUser.NDAVerified == true)
            {
                if (existingIndustry != null)
                {
                    existingIndustry.Quantity -= qty;
                    updateIndustry(existingIndustry);
                    await ReplyAsync("I have updated the queue of " + partName + " to a quantity of " + existingIndustry.Quantity + ".");
                }
                else
                {

                }
            }
        }
        //Jobs
        [Command("Create Job", RunMode = RunMode.Async)]
        public async Task createJob(string jobType, string jobClass)
        {
            var user = Context.User as SocketGuildUser;
            if (user == null) return;
            var askingUser = GetUser(user);
            if (askingUser.JoinedFaction == true)
            {
                char gradeLetter = askingUser.Grade[0];
                if (gradeLetter == 'O' || gradeLetter == 'M')
                {
                    var secureChannel = Context.Guild.GetTextChannel(Epsilon.SecureChannelID);
                    var announceChannel = Context.Guild.GetTextChannel(Epsilon.AnnounceChannelID);

                    var existingJob = GetJob(jobType, jobClass);
                    if (existingJob == null && (JobClassList.Any(x => x.Equals(jobClass,StringComparison.OrdinalIgnoreCase))))
                    {

                    }
                }
            }
            ResetAttempts(user);
            await SendLogMessage(user, "Create Job");
        }
        //Methods
        private void ResetAttempts(SocketGuildUser user)
        {
            var guest = GetUser(user);
            guest.NumberOfAttempts = 0;
            SaveUser(guest);
        }
        private User GetUser(SocketGuildUser userID)
        {
            var db = new DatabaseContext();
            try
            {
                return db.Users.Single(x => x.DiscordId == userID.Id);
            }
            catch (Exception e)
            {
                Console.WriteLine("User failed to retrieve from the database. " + e.Message);
                return null;
            }
        }
        private Industry CheckIndustry(string partName)
        {
            var db = new DatabaseContext();
            try
            {
                return db.Indy.Single(x => x.PartName.Equals(partName, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception e)
            {
                Console.WriteLine("Industry failed to retrieve from the database. " + e.Message);
                return null;
            }
        }
        private void updateIndustry(Industry indy)
        {
            var db = new DatabaseContext();
            try
            {
                db.Indy.Update(indy);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine("Unabe to update Industry. " + e.Message);
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
        private async Task SendLogMessage(SocketGuildUser userID, string commandName)
        {
            var channel = Context.Guild.GetTextChannel(Epsilon.LogChannelID);
            await channel.SendMessageAsync("```" + userID.ToString() + " used the command " + commandName + " at " + DateTimeOffset.UtcNow + " UTC. ```");
        }
        private async Task CheckWarnings(SocketGuildUser user, User guest)
        {
            if (guest.NumberOfWarnings >= 3)
            {
                guest.PersonalStanding = ((-10 - guest.PersonalStanding) * 0.1F) + guest.PersonalStanding;
                await CheckStanding(user, guest);
                guest.NumberOfWarnings = 0;
                await ReplyAsync(user.Username + ", you have reached 3 warnings and I dropped your standing.  Your warnings are back to 0.  You " +
                    "now have a standing of:  " + Math.Round(guest.PersonalStanding, 2) + ".");
                SaveUser(guest);
            }
            else if (guest.NumberOfWarnings < 3)
            {
                guest.NumberOfWarnings++;
                SaveUser(guest);
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
        private async Task SetRank(SocketGuildUser user, string grade)
        {
            var db = new DatabaseContext();
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
                Context.Channel.SendMessageAsync("Failed to get Game Name List.  " + e.Message);
            }
        }
        private void GetJobTypeList()
        {
            try
            {
                StreamReader jobTypeReader = new StreamReader("JobTypes.txt");
                while (!jobTypeReader.EndOfStream)
                {
                    string jobType = jobTypeReader.ReadLine();
                    JobTypeList.Add(jobType);
                }
                jobTypeReader.Close();
            }
            catch (Exception e)
            {
                Context.Channel.SendMessageAsync("Failed to get list of job types.  " + e.Message);
            }
        }
        private void GetJobClassList()
        {
            try
            {
                StreamReader jobClassReader = new StreamReader("JobClasses.txt");
                while (!jobClassReader.EndOfStream)
                {
                    string jobClass = jobClassReader.ReadLine();
                    JobClassList.Add(jobClass);
                }
                jobClassReader.Close();
            }
            catch (Exception e)
            {
                Context.Channel.SendMessageAsync("Failed to get list of job classes.  " + e.Message);
            }
        }
        private Job GetJob(string jobType, string jobClass)
        {
            var db = new DatabaseContext();
            try
            {
                return db.Jobs.Single(x => x.JobType.Equals(jobType, StringComparison.OrdinalIgnoreCase) && x.JobClass.Equals(jobClass, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception e)
            {
                Context.Channel.SendMessageAsync("Op failed to retrieve an instance from the database. " + e.Message);
                return null;
            }
        }
        private Job.Group GetGroup(string jobType, string jobClass)
        {
            var db = new DatabaseContext();
            try
            {
                return db.Groups.Single(x => x.JobType.Equals(jobType, StringComparison.OrdinalIgnoreCase) && x.JobClass.Equals(jobClass, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception e)
            {
                Context.Channel.SendMessageAsync("Group failed to retrieve an instance from the database.  " + e.Message);
                return null;
            }
        }
        private Job.Group.Team GetTeam(string jobType, string jobClass)
        {
            var db = new DatabaseContext();
            try
            {
                return db.Teams.Single(x => x.JobType.Equals(jobType, StringComparison.OrdinalIgnoreCase) && x.JobClass.Equals(jobClass, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception e)
            {
                Context.Channel.SendMessageAsync("Team failed to retrieve an instance from the database.  " + e.Message);
                return null;
            }
        }
    }
}

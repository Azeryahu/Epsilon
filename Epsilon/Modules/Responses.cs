using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord.Commands;
using System.Threading.Tasks;
using Discord.WebSocket;
using System;
using Discord;

namespace Epsilon.Modules
{
    public class Responses : ModuleBase<SocketCommandContext>
    {
        [Command("Enlist")]
        public async Task enlist(SocketGuildUser userID = null)
        {
            var knight = Context.User as SocketGuildUser;
            if (knight == null) return;
            var db = new DatabaseContext();
            var newKnight = new Knight();
            var targetKnight = GetDBUser(knight);
            var rolePrivate = (knight.Guild.Roles.Where(x => x.Name.ToUpper() == "Recruit".ToUpper()));
            var roleGuest = (knight.Guild.Roles.Where(x => x.Name.ToUpper() == "Guest".ToUpper()));
            if (db.Knights.Any(x => x.UserID == knight.ToString()))
            {
                await ReplyAsync("You have already enlisted into the Quantum Force Knights, and since you are already enlisted, I cannot enlist you a second time.");
            }
            else if (targetKnight.Verified == true)
            {
                newKnight.ServerJoinDate = knight.JoinedAt;
                newKnight.EnlistmentDate = DateTimeOffset.UtcNow;
                newKnight.OfficerGradDate = null;
                newKnight.PointBalance = 0;
                newKnight.DaysUntilPromotion = 0;
                newKnight.CompletedMissions = 0;
                newKnight.PersonalStanding = 10;
                newKnight.OrganizationStanding = 10;
                newKnight.AllianceStanding = 10;
                newKnight.UserID = knight.ToString();
                newKnight.Username = knight.Username;
                newKnight.RankTitle = "Recruit";
                newKnight.OrganizationName = "Quantum Force Knights";
                newKnight.AllianceName = "Aeonian Federation";
                SaveKnight(newKnight);
                await knight.AddRolesAsync(rolePrivate);
                await knight.RemoveRolesAsync(roleGuest);
                await ReplyAsync("Congragulations " + knight.Username + " as you have now joined an elite organization.  The Quantum Force Knights!  Welcome and I wish you the best.");
            }
            else if (userID.ToString() != "")
            {
                if(knight.ToString() == "Lambert#5479")
                {
                    newKnight.ServerJoinDate = userID.JoinedAt;
                    newKnight.EnlistmentDate = DateTimeOffset.UtcNow;
                    newKnight.OfficerGradDate = null;
                    newKnight.PointBalance = 0;
                    newKnight.DaysUntilPromotion = 0;
                    newKnight.CompletedMissions = 0;
                    newKnight.PersonalStanding = 10;
                    newKnight.OrganizationStanding = 10;
                    newKnight.AllianceStanding = 10;
                    newKnight.UserID = userID.ToString();
                    newKnight.Username = userID.Username;
                    newKnight.RankTitle = "Recruit";
                    newKnight.OrganizationName = "Quantum Force Knights";
                    newKnight.AllianceName = "Aeonian Federation";
                    SaveKnight(newKnight);
                    await knight.AddRolesAsync(rolePrivate);
                    await knight.RemoveRolesAsync(roleGuest);
                    await ReplyAsync("Congragulations " + userID.Username + " as you have now joined an elite organization.  The Quantum Force Knights!  Welcome and I wish you the best.");
                }
            }
            else
            {
                await ReplyAsync("You are not authorized to enlist in the organization right now.  Please apply online @ https://community.dualthegame.com/organization/quantum-force-knights#tab-applications." +
                    "\nOnce you have been accepted, we will then process your authorization.");
            }
            await SendLogMessage(knight, "Enlist");
        }
        [Command("Add User")]
        public async Task addUser(SocketGuildUser userID)
        {
            var user = Context.User as SocketGuildUser;
            var newUser = new Visitor();
            if (user == null) return;
            var db = new DatabaseContext();
            var role = (user.Guild.Roles.Where(x => x.Name.ToUpper() == "Guests".ToUpper()));
            if (user.Roles.Any(x => x.Name == "High King"))
            {
                if (db.Visitors.Any(x => x.UserID == userID.ToString()))
                {
                    await ReplyAsync(userID.Username + " has already been added to the database, and I cannot add " + userID.Username + " again as that would not be acceptable by my creator.");
                }
                else
                {
                    newUser.ServerJoinDate = userID.JoinedAt;
                    newUser.CompletedMissions = 0;
                    newUser.PersonalStanding = 0;
                    newUser.OrganizationStanding = 0;
                    newUser.AllianceStanding = 0;
                    newUser.UserID = userID.ToString();
                    newUser.OrganizationName = "";
                    newUser.AllianceName = "";
                    newUser.Title = "Guest";
                    newUser.Verified = false;
                    newUser.Joined = false;
                    SaveUser(newUser);
                    await user.AddRolesAsync(role);
                    await ReplyAsync(userID.Username + " has been added as a basic user and is now considered a Guest on the server.");
                }
            }
            await SendLogMessage(user, "Add User");
        }
        [Command("Add Alliance")]
        [Alias("Add Ally", "add ally")]
        public async Task addAlliance(DateTimeOffset foundedDate, int orgCount, int memberCount, string allianceID, [Remainder] string allianceName)
        {
            var user = Context.User as SocketGuildUser;
            if (user == null) return;
            var newAlliance = new Alliance();
            var targetAlliance = GetAlliance(allianceID);
            var db = new DatabaseContext();
            if (user.Roles.Any(x => x.Name == "High King"))
            {
                if (db.Alliances.Any(x =>x.AllianceID == allianceID) && db.Alliances.Any(x => x.AllianceName == allianceName))
                {
                    await ReplyAsync(targetAlliance.AllianceName + " has already been added to my databanks, and I cannot add another instance of this alliance.");
                }
                else
                {
                    newAlliance.AllianceFoundedDate = foundedDate;
                    newAlliance.OrganizationCount = orgCount;
                    newAlliance.MemberCount = memberCount;
                    newAlliance.AllianceStanding = 0;
                    newAlliance.AllianceID = allianceID;
                    newAlliance.AllianceName = allianceName;
                    SaveAlliance(newAlliance);
                    await ReplyAsync(newAlliance.AllianceName + " has been added to my databanks.");
                }
            }
            await SendLogMessage(user, "Add Alliance");
        }
        [Command("Add Organization")]
        [Alias("Add Org", "add org")]
        public async Task addOrganization(DateTimeOffset foundedDate, int memberCount, string organizationID, string allianceID, [Remainder] string organizationName)
        {
            var user = Context.User as SocketGuildUser;
            if (user == null) return;
            var newOrganization = new Organization();
            var targetOrganization = GetOrganization(organizationID);
            var db = new DatabaseContext();
            if (user.Roles.Any(x => x.Name == "High King"))
            {
                if (db.Organizations.Any(x => x.OrganizationID == organizationID) && db.Organizations.Any(x => x.OrganiztionName == organizationName))
                {
                    await ReplyAsync(targetOrganization.OrganiztionName + " has already been added to my databanks, and I cannot add another instance of this alliance.");
                }
                else
                {
                    newOrganization.OrganizationFoundedDate = foundedDate;
                    newOrganization.MemberCount = memberCount;
                    newOrganization.OrganizationStanding = 0;
                    newOrganization.OrganiztionName = organizationName;
                    newOrganization.OrganizationID = organizationID;
                    newOrganization.AllianceID = allianceID;
                    SaveOrganization(newOrganization);
                    await ReplyAsync(newOrganization.OrganiztionName + " has been added to my databanks.");
                }
            }
            await SendLogMessage(user, "Add Organization");

        }
        [Command("Change Joined Date")]//Used if enlist date does not match organaization date.
        public async Task changeJoinedDate(DateTimeOffset date, [Remainder] SocketGuildUser userID)
        {
            var db = new DatabaseContext();
            var user = Context.User as SocketGuildUser;
            if (user.Roles.Any(x => x.Name.ToUpper() == "High King".ToUpper()))
            {
                try
                {
                    var targetUser = db.Visitors.Single(x => x.UserID == userID.ToString());
                    targetUser.ServerJoinDate = date;
                    await ReplyAsync(userID.Username + "'s joined date has been changed to " + date.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("User failed to retrieve. " + e.Message);
                }
            }
            await SendLogMessage(user, "Change Joined Date");
        }
        [Command("Lookup")]
        public async Task lookup([Remainder] SocketGuildUser inputID)
        {
            var askingUser = Context.User as SocketGuildUser;
            var user = GetDBUser(inputID);
            var knight = GetDBKnight(inputID);
            if (user != null && knight == null)
            {
                if (user.AllianceName != "" && user.OrganizationName != "")
                {
                    await ReplyAsync("I have found " + inputID.Username + " and here is what I know:" +
                        "\n" + inputID.Username + " joined the server on " + user.ServerJoinDate.ToString() + "." +
                        "\nThey have a standing of " + user.AllianceStanding + " based on their alliance's standing with Quantum Force Knights." +
                        "\nThey belong to " + user.OrganizationName + "." +
                        "\nThey are known here as a " + user.Title + "." +
                        "\nThey have completed " + user.CompletedMissions + " missions for the Quantum Force Knights.");
                }
                else if (user.AllianceName == "" && user.OrganizationName != "")
                {
                    await ReplyAsync("I have found " + inputID.Username + " and here is what I know:" +
                        "\n" + inputID.Username + " joined the server on " + user.ServerJoinDate.ToString() + "." +
                        "\nThey have a standing of " + user.OrganizationStanding + " based on their organization's standing with Quantum Force Knights." +
                        "\nThey belong to " + user.OrganizationName + "." +
                        "\nThey are known here as a " + user.Title + "." +
                        "\nThey have completed " + user.CompletedMissions + " missions for the Quantum Force Knights.");
                }
                else if (user.AllianceName == "" && user.OrganizationName == "")
                {
                    await ReplyAsync("I have found " + inputID.Username + " and here is what I know :" +
                        "\n" + inputID.Username + " joined the server on " + user.ServerJoinDate.ToString() + "." +
                        "\nThey have a standing of " + user.AllianceStanding + " based on their personal standing with Quantum Force Knights." +
                        "\nThey currently do not belong to any organization and may be considered a 'Free Agent'." +
                        "\nThey are here as a " + user.Title + "." +
                        "\nThey have completed " + user.CompletedMissions + " missions for the Quantum Force Knights.");
                }
            }
            else if (user != null && knight != null)
            {
                if (knight.AllianceName != "" && knight.OrganizationName != "")
                {
                    await ReplyAsync("I have found " + knight.Username + ", and here is what I know:" +
                        "\n" + knight.Username + " enlisted on " + knight.EnlistmentDate + "." +
                        "\nThey have are at a standing of " + knight.PersonalStanding + " as they are a member of the Quantum Force Knights" +
                        "\nThey are known here as a " + knight.RankTitle + "." +
                        "\nThey have completed " + knight.CompletedMissions + " missions for the Quantum Force Knights");
                }
                else if (knight.AllianceName == "" && knight.OrganizationName != "")
                {
                    await ReplyAsync("I have found " + knight.Username + ", and here is what I know:" +
                        "\n" + knight.Username + " enlisted on " + knight.EnlistmentDate + "." +
                        "\nThey have are at a standing of " + knight.PersonalStanding + " as they are a member of the Quantum Force Knights" +
                        "\nThey are known here as a " + knight.RankTitle + "." +
                        "\nThey have completed " + knight.CompletedMissions + " missions for the Quantum Force Knights");
                }
                else
                {
                    await ReplyAsync("There is no knight here that does not belong to an organization as all knights here must belong to the Quantum Force Knights.");
                }
            }
            else
            {
                await ReplyAsync("I am sorry " + askingUser.Username + ", but I was unable to find the person you are looking for.  " +
                        "They are not in my databanks, and may not have regesterd yet.  I would take great care in dealing with " + inputID.Username +
                        " until I can further assess the situation.");
            }
            await SendLogMessage(askingUser, "Lookup");
        }
        [Command("Alliance Lookup")]
        public async Task allianceLookup(string allianceID)
        {
            var user = Context.User as SocketGuildUser;
            var db = new DatabaseContext();
            var targetAlliance = GetAlliance(allianceID);
            if (targetAlliance != null)
            {
                if (targetAlliance.AllianceDisbandedDate == null)
                {
                    targetAlliance.MemberCount = db.Organizations.Sum(x => x.MemberCount);
                    targetAlliance.MemberCount = db.Organizations.Count(x => x.AllianceID == targetAlliance.AllianceID);
                    await ReplyAsync("I was able to find an alliance with the tag " + allianceID + ", and here is what I know about them:" +
                        "\nThe " + targetAlliance.AllianceName + " was founded on " + targetAlliance.AllianceFoundedDate + "." +
                        "\nThis alliance still exists today." +
                        "\nThey have a total of " + targetAlliance.OrganizationCount + " organizations who are members of this alliance." +
                        "\nThere is a total of " + targetAlliance.MemberCount + " members of this alliance." +
                        "\nThey currently have a standing of " + targetAlliance.AllianceStanding + " with the Quantum Force Knights." +
                        "\nIf you would like any further information, please ask a High King.");
                    SaveAlliance(targetAlliance);
                }
                else
                {
                    await ReplyAsync("I was able to find an alliance with the tag " + allianceID + ", and here is what I know about them:" +
                        "\nThe " + targetAlliance.AllianceName + " was founded on " + targetAlliance.AllianceFoundedDate + "." +
                        "\nThis alliance disbanded and ended on " + targetAlliance.AllianceDisbandedDate + "." +
                        "\nThey have a total of " + targetAlliance.OrganizationCount + " organizations who are members of this alliance." +
                        "\nThere is a total of " + targetAlliance.MemberCount + " members of this alliance." +
                        "\nThey currently have a standing of " + targetAlliance.AllianceStanding + " with the Quantum Force Knights." +
                        "\nIf you would like any further information, please ask a High King.");
                }
            }
            await SendLogMessage(user, "Alliance Lookup");
        }
        [Command("Organization Lookup")]
        [Alias("Org Lookup", "org lookup")]
        public async Task organizationLookup(string organizationID)
        {
            var user = Context.User as SocketGuildUser;
            var targetOrg = GetOrganization(organizationID);
            var targetAlliance = GetAlliance(targetOrg.AllianceID);
            if (targetOrg != null)
            {
                if (targetOrg.OrganizationDisbandedDate == null && targetAlliance != null)
                {
                    await ReplyAsync("I was able to find an organization with the tag " + organizationID + ", and here is what I know about them:" +
                        "\nThe " + targetOrg.OrganiztionName + " was founded on " + targetOrg.OrganizationFoundedDate + "." +
                        "\nThis organization still exists today." +
                        "\nThey are a member of the " + targetAlliance.AllianceName + " alliance." +
                        "\nThere is a total of " + targetOrg.MemberCount + " members in this organization." +
                        "\nThey currently have a standing of " + targetOrg.OrganizationStanding + " with the Quantum Force Knights." +
                        "\nIf you would like any further information, please ask a High King.");
                }
                else if (targetOrg.OrganizationDisbandedDate == null && targetAlliance == null)
                {
                    await ReplyAsync("I was able to find an organization with the tag " + organizationID + ", and here is what I know about them:" +
                        "\nThe " + targetOrg.OrganiztionName + " was founded on " + targetOrg.OrganizationFoundedDate + "." +
                        "\nThis organization still exists today." +
                        "\nThey are currently not part of any alliance." +
                        "\nThere is a total of " + targetOrg.MemberCount + " members in this organization." +
                        "\nThey currently have a standing of " + targetOrg.OrganizationStanding + " with the Quantum Force Knights." +
                        "\nIf you would like any further information, please ask a High King.");
                }
                else if (targetOrg.OrganizationDisbandedDate != null && targetAlliance != null)
                {
                    await ReplyAsync("I was able to find an organization with the tag " + organizationID + ", and here is what I know about them:" +
                        "\nThe " + targetOrg.OrganiztionName + " was founded on " + targetOrg.OrganizationFoundedDate + "." +
                        "\nThis organization disbanded on " + targetOrg.OrganizationDisbandedDate + "." +
                        "\nThey were in the " + targetAlliance.AllianceName +" alliance." +
                        "\nThey had a total of " + targetOrg.MemberCount + " members in this organization." +
                        "\nTheir last known standing was " + targetOrg.OrganizationStanding + " with the Quantum Force Knights." +
                        "\nIf you would like any further information, please ask a High King.");
                }
                else if (targetOrg.OrganizationDisbandedDate != null && targetAlliance == null)
                {
                    await ReplyAsync("I was able to find an organization with the tag " + organizationID + ", and here is what I know about them:" +
                        "\nThe " + targetOrg.OrganiztionName + " was founded on " + targetOrg.OrganizationFoundedDate + "." +
                        "\nThis organization disbanded on " + targetOrg.OrganizationDisbandedDate + "." +
                        "\nThey were not part of any alliance when they disbanded." +
                        "\nThey had a total of " + targetOrg.MemberCount + " members in this organization." +
                        "\nTheir last known standing was " + targetOrg.OrganizationStanding + " with the Quantum Force Knights." +
                        "\nIf you would like any further information, please ask a High King.");
                }
            }
            await SendLogMessage(user, "Organization Lookup");
        }
        [Command("Add Standing")]
        public async Task addStanding(decimal addedStanding, [Remainder] SocketGuildUser userID)
        {
            var user = Context.User as SocketGuildUser;
            if (user == null) return;

            if (user.Roles.Any(x => x.Name == "High King"))
            {
                try
                {
                    var targetUser = GetDBUser(userID);
                    targetUser.PersonalStanding = Math.Round(targetUser.PersonalStanding = ((10 - targetUser.PersonalStanding) * addedStanding) + targetUser.PersonalStanding, 2);

                    if (targetUser.PersonalStanding >= 10)
                    {
                        targetUser.PersonalStanding = 10;
                        await SetStandingRole(userID, targetUser, "Trusted");
                        await ReplyAsync("Congragulations " + targetUser.UserID + ".  You now have a standing of " + targetUser.PersonalStanding + " with Scabra Metallum, and are trusted among your peers.");
                    }
                    else if (targetUser.PersonalStanding < 10 && targetUser.PersonalStanding >= 5)
                    {
                        await SetStandingRole(userID, targetUser, "Friendly");
                        await ReplyAsync("Congragulations " + targetUser.UserID + ".  You now have a standing of " + targetUser.PersonalStanding + " with Scabra Metallum, and now are friendly.");
                    }
                    else if (targetUser.PersonalStanding < 5 && targetUser.PersonalStanding >= 0)
                    { 
                        await SetStandingRole(userID, targetUser, "Neutral");
                        await ReplyAsync("Congragulations " + targetUser.UserID + ".  You now have a standing of " + targetUser.PersonalStanding + " with Scabra Metallum, but you are considered neutral.");
                    }
                    else if (targetUser.PersonalStanding < 0 && targetUser.PersonalStanding >= -5)
                    {
                        await SetStandingRole(userID, targetUser, "Suspicious");
                        await ReplyAsync("Congragulations " + targetUser.UserID + ".  You now have a standing of " + targetUser.PersonalStanding + " with Scabra Metallum, but you are considered suspicious.");
                    }
                    else if (targetUser.PersonalStanding < -5 && targetUser.PersonalStanding >= -10)
                    {
                        await SetStandingRole(userID, targetUser, "Abhorred");
                        await ReplyAsync("Congragulations " + targetUser.UserID + ".  You now have a standing of " + targetUser.PersonalStanding + " with Scabra Metallum, but you are still considered abhorred.");
                    }
                    else if (targetUser.PersonalStanding < -10)
                    {
                        targetUser.PersonalStanding = -10;
                    }
                    else
                    {
                        await ReplyAsync("You don't seam to have any standing.  How very strange.");
                    }
                    SaveUser(targetUser);
                }
                catch
                {
                    await ReplyAsync("User was not found. Try adding a username you would like to give these points.");
                }
            }
            else
            {
                await ReplyAsync("Silly human. You do not have permission to add points to yourself.");
            }
        }
        [Command("Take Standing")]
        public async Task takeStanding(decimal takeingStanding, [Remainder] SocketGuildUser userID)
        {
            var user = Context.User as SocketGuildUser;
            if (user == null) return;

            if (user.Roles.Any(x => x.Name == "High King"))
            {
                var db = new DatabaseContext();
                var targetUser = GetDBUser(userID);
                try
                {
                    Math.Round(targetUser.PersonalStanding = ((-10 - targetUser.PersonalStanding) * takeingStanding / 100) + targetUser.PersonalStanding, 2);
                    if (targetUser.PersonalStanding >= 10)
                    {
                        targetUser.PersonalStanding = 10;
                        await SetStandingRole(userID, targetUser, "Trusted");
                        await ReplyAsync("I am sorry to inform you " + targetUser.UserID + ", but your PersonalStanding has dropped, and you now have a PersonalStanding of " + Math.Round(targetUser.PersonalStanding, 2) + " with Scabra Metallum, but you are still trusted among your peers.");
                    }
                    else if (targetUser.PersonalStanding < 10 && targetUser.PersonalStanding >= 5)
                    {
                        await SetStandingRole(userID, targetUser, "Friendly");
                        await ReplyAsync("I am sorry to inform you " + targetUser.UserID + ", but your PersonalStanding has dropped, and you now have a PersonalStanding of " + Math.Round(targetUser.PersonalStanding, 2) + " with Scabra Metallum, and you are friendly.");
                    }
                    else if (targetUser.PersonalStanding < 5 && targetUser.PersonalStanding >= 0)
                    {
                        await SetStandingRole(userID, targetUser, "Neutral");
                        await ReplyAsync("I am sorry to inform you " + targetUser.UserID + ", but your PersonalStanding has dropped, and you now have a PersonalStanding of " + Math.Round(targetUser.PersonalStanding, 2) + " with Scabra Metallum, and you are neutral.");
                    }
                    else if (targetUser.PersonalStanding < 0 && targetUser.PersonalStanding >= -5)
                    {
                        await SetStandingRole(userID, targetUser, "Suspicious");
                        await ReplyAsync("I am sorry to inform you " + targetUser.UserID + ", but your PersonalStanding has dropped, and you now have a PersonalStanding of " + Math.Round(targetUser.PersonalStanding, 2) + " with Scabra Metallum, and you are suspicious.");
                    }
                    else if (targetUser.PersonalStanding < -5 && targetUser.PersonalStanding >= -10)
                    {
                        await SetStandingRole(userID, targetUser, "Abhorred");
                        await ReplyAsync("I am sorry to inform you " + targetUser.UserID + ", but your PersonalStanding has dropped, and you now have a PersonalStanding of " + Math.Round(targetUser.PersonalStanding, 2) + " with Scabra Metallum.  They view you as abhorred.");
                    }
                    else if (targetUser.PersonalStanding < -10)
                    {
                        targetUser.PersonalStanding = -10;
                    }
                    else
                    {
                        await ReplyAsync("You don't seam to have any PersonalStanding.  How very strange.");
                    }
                    SaveUser(targetUser);
                }
                catch
                {
                    await ReplyAsync("User was not found.  Try adding the username you would like to take these points from.");
                }
            }
            else
            {
                await ReplyAsync("Silly human.  I will not allow you to take standings.");
            }
        }
        [Command("Check Standing")]
        public async Task checkStanding([Remainder] SocketGuildUser userID = null)
        {
            var user = Context.User as SocketGuildUser;
            if (user == null) return;
            var askingUser = GetDBUser(user);
            if (userID.ToString() == "")
            {
                if (askingUser.OrganizationName != "" && askingUser.AllianceName != "")
                {
                    await ReplyAsync("Your standing with the Quantum Force Knights is a" + askingUser.AllianceStanding + " as you belong to the " + askingUser.AllianceName + " alliance.");
                }
                else if(askingUser.OrganizationName != "" && askingUser.AllianceName == "")
                {
                    await ReplyAsync("Your standing with the Quantum Force Knights is a " + askingUser.OrganizationStanding + " as you are in the " + askingUser.OrganizationName + " organization.");
                }
                else if (askingUser.OrganizationName == "" && askingUser.AllianceName == "")
                {
                    await ReplyAsync("Your standing with the Quantum Force Knights is a " + askingUser.PersonalStanding + " as you are not part of an organization.");
                }
                else
                {
                    await ReplyAsync("You do not seam to have any standing with the Quantum Force Knights.  How very odd.");
                }
            }
            else if (userID.ToString() != "")
            {
                var targetUser = GetDBUser(userID);
                if (targetUser.OrganizationName != "" && targetUser.AllianceName != "")
                {
                    await ReplyAsync(userID.Username + "'s standing with the Quantum Force Knights is a" + targetUser.AllianceStanding + " as you belong to the " + targetUser.AllianceName + " alliance.");
                }
                else if (targetUser.OrganizationName != "" && targetUser.AllianceName == "")
                {
                    await ReplyAsync(userID.Username + "'s standing with the Quantum Force Knights is a " + targetUser.OrganizationStanding + " as you are in the " + targetUser.OrganizationName + " organization.");
                }
                else if (targetUser.OrganizationName == "" && targetUser.AllianceName == "")
                {
                    await ReplyAsync(userID.Username + "'s standing with the Quantum Force Knights is a " + targetUser.PersonalStanding + " as you are not part of an organization.");
                }
                else
                {
                    await ReplyAsync("You do not seam to have any standing with the Quantum Force Knights.  How very odd.");
                }
            }
            else
            {
                await ReplyAsync("Something went wrong.  Are you sure you used the right command?");
            }
            await SendLogMessage(user, "Check Standing");
        }
        [Command("Verify User")]
        public async Task verifyUser(SocketGuildUser userID)
        {
            var user = Context.User as SocketGuildUser;
            if (user == null) return;
            var targetUser = GetDBUser(userID);
            if (user.Roles.Any(x => x.Name == "High King"))
            {
                targetUser.Verified = true;
                SaveUser(targetUser);
            }
            await SendLogMessage(user, "Verify User");
        }
        [Command("")]
        private void SaveUser(Visitor user)
        {
            var db = new DatabaseContext();
            try
            {
                db.Visitors.Update(user);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine("User failed to retrieve. " + e.Message);
            }
        }
        private void SaveKnight(Knight knight)
        {
            var db = new DatabaseContext();
            try
            {
                db.Knights.Update(knight);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine("User failed to retrieve. " + e.Message);
            }
        }
        private void SaveAlliance(Alliance alliance)
        {
            var db = new DatabaseContext();
            try
            {
                db.Alliances.Update(alliance);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine("Alliance failed to retrieve. " + e.Message);

            }
        }
        private void SaveOrganization(Organization organization)
        {
            var db = new DatabaseContext();
            try
            {
                db.Organizations.Update(organization);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine("Organization failed to retrieve. " + e.Message);
            }
        }
        private async Task SetStandingRole(IGuildUser username, Visitor user, string userStandingRoleTitle)
        {
            var roleTrusted = username.Guild.Roles.Where(x => x.Name.ToUpper() == "Trusted".ToUpper());
            var roleFriendly = username.Guild.Roles.Where(x => x.Name.ToUpper() == "Friendly".ToUpper());
            var roleNeutral = username.Guild.Roles.Where(x => x.Name.ToUpper() == "Neutral".ToUpper());
            var roleSuspicious = username.Guild.Roles.Where(x => x.Name.ToUpper() == "Suspicious".ToUpper());
            var roleAbhorred = username.Guild.Roles.Where(x => x.Name.ToUpper() == "Abhorred".ToUpper());
            await username.RemoveRolesAsync(roleTrusted);
            await username.RemoveRolesAsync(roleFriendly);
            await username.RemoveRolesAsync(roleNeutral);
            await username.RemoveRolesAsync(roleSuspicious);
            await username.RemoveRolesAsync(roleAbhorred);

            switch (userStandingRoleTitle)
            {
                case "Trusted":
                    await username.AddRolesAsync(roleTrusted);
                    break;
                case "Friendly":
                    await username.AddRolesAsync(roleFriendly);
                    break;
                default:
                case "Neutral":
                    await username.AddRolesAsync(roleNeutral);
                    break;
                case "Suspicious":
                    await username.AddRolesAsync(roleSuspicious);
                    break;
                case "Abhorred":
                    await username.AddRolesAsync(roleAbhorred);
                    break;
            }
            user.Title = userStandingRoleTitle;
        }
        private async Task SetKnightRole(IGuildUser username, Knight knight, string knightRankRoleTitle)
        {
            var roleRecruit = username.Guild.Roles.Where(x => x.Name.ToUpper() == "Recruit".ToUpper());
            var rolePvt = username.Guild.Roles.Where(x => x.Name.ToUpper() == "Private".ToUpper());
            var rolePFC = username.Guild.Roles.Where(x => x.Name.ToUpper() == "Private First Class".ToUpper());
            var roleLCpl = username.Guild.Roles.Where(x => x.Name.ToUpper() == "Lance Corporal".ToUpper());
            var roleCpl = username.Guild.Roles.Where(x => x.Name.ToUpper() == "Corporal".ToUpper());
            var roleSgt = username.Guild.Roles.Where(x => x.Name.ToUpper() == "Sergeant".ToUpper());
            var roleSSgt = username.Guild.Roles.Where(x => x.Name.ToUpper() == "Staff Sergeant".ToUpper());
            var roleGnySgt = username.Guild.Roles.Where(x => x.Name.ToUpper() == "Gunnery Sergeant".ToUpper());
            var roleMstrSgt = username.Guild.Roles.Where(x => x.Name.ToUpper() == "Master Sergeant".ToUpper());
            var roleSMaj = username.Guild.Roles.Where(x => x.Name.ToUpper() == "Sergeant Major".ToUpper());
            await username.RemoveRolesAsync(roleRecruit);
            await username.RemoveRolesAsync(rolePvt);
            await username.RemoveRolesAsync(rolePFC);
            await username.RemoveRolesAsync(roleLCpl);
            await username.RemoveRolesAsync(roleCpl);
            await username.RemoveRolesAsync(roleSgt);
            await username.RemoveRolesAsync(roleSSgt);
            await username.RemoveRolesAsync(roleGnySgt);
            await username.RemoveRolesAsync(roleMstrSgt);
            await username.RemoveRolesAsync(roleSMaj);
            switch (knightRankRoleTitle)
            {
                default:
                case "Recruit":
                    await username.AddRolesAsync(roleRecruit);
                    break;
                case "Private":
                    await username.AddRolesAsync(rolePvt);
                    break;
                case "Private First Class":
                    await username.AddRolesAsync(rolePFC);
                    break;
                case "Lance Corporal":
                    await username.AddRolesAsync(roleLCpl);
                    break;
                case "Corporal":
                    await username.AddRolesAsync(roleCpl);
                    break;
                case "Sergeant":
                    await username.AddRolesAsync(roleSgt);
                    break;
                case "Staff Sergeant":
                    await username.AddRolesAsync(roleSSgt);
                    break;
                case "Gunnery Sergeant":
                    await username.AddRolesAsync(roleGnySgt);
                    break;
                case "Master Sergeant":
                    await username.AddRolesAsync(roleMstrSgt);
                    break;
                case "Sergeant Major":
                    await username.AddRolesAsync(roleSMaj);
                    break;
            }
            knight.RankTitle = knightRankRoleTitle;
        }
        private Visitor GetDBUser(SocketGuildUser userID)
        {
            var db = new DatabaseContext();
            try
            {
                return db.Visitors.Single(x => x.UserID == userID.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("User failed to retrieve from the database. " + e.Message);
                return null;
            }
        }
        private Knight GetDBKnight(SocketGuildUser userID)
        {
            var db = new DatabaseContext();
            try
            {
                return db.Knights.Single(x => x.UserID == userID.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Knight failed to retrieve from the database. " + e.Message);
                return null;
            }
        }
        private Alliance GetAlliance(string allianceID)
        {
            var db = new DatabaseContext();
            try
            {
                return db.Alliances.Single(x => x.AllianceID == allianceID);
            }
            catch (Exception e)
            {
                Console.WriteLine("Alliance failed to retrieve from the database. " + e.Message);
                return null;
            }
        }
        private Organization GetOrganization(string OrgID)
        {
            var db = new DatabaseContext();
            try
            {
                return db.Organizations.Single(x => x.OrganizationID == OrgID);
            }
            catch (Exception e)
            {
                Console.WriteLine("Organization failed to retrieve from the database. " + e.Message);
                return null;
            }
        }
        /*private Alliance ReverseGetAlliance(string allianceID)
        {
            var db = new DatabaseContext();
            try
            {
                return db.Organizations.Single(x => x.OrganizationID == or)
            }
            catch (Exception e)
            {
                Console.WriteLine("I was unable to retrieve the proper information from the database. " + e.Message);
                return null;
            }
        }*/
        private async Task SendLogMessage(SocketGuildUser userID, string commandName)
        {
            var channel = Context.Guild.GetTextChannel(400816839967375361);
            await channel.SendMessageAsync("```" + userID.ToString() + " used the command " + commandName + " at " + DateTimeOffset.UtcNow + " UTC. ```");
        }

    }
}

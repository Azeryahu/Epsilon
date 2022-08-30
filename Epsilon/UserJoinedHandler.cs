using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace Epsilon
{
    class UserJoinedHandler : EventHandler
    {
        private DatabaseContext _db;
        public UserJoinedHandler(DiscordSocketClient client) : base(client)
        {
            base._client.UserJoined += OnEvent;
            _db = new DatabaseContext();
        }
        private Task OnEvent(SocketGuildUser user)
        {
            return Task.Run(async () =>
            {
                var newMember = new User();
                var adminRole = user.Guild.GetRole(Epsilon.ConfigFile.AdministratorRoleID);
                var channel = user.Guild.GetTextChannel(Epsilon.ConfigFile.AnnouncementChannelID);
                _db = new DatabaseContext();
                var foundUser = GetUser(user);
                await channel.SendMessageAsync(adminRole.Mention + $", {user.Username} has joined the server.");
                if (foundUser != null && foundUser.DiscordId.Equals(user.Id))
                {
                    await channel.SendMessageAsync(user.Username + " was found in the database.");
                    foundUser.ServerJoinDate = user.JoinedAt;
                    foundUser.Rank = "Guest";
                    foundUser.DiscordUserID = user.ToString();
                    foundUser.DiscordUsername = user.Username;
                    SaveUser(foundUser, _db);
                    await channel.SendMessageAsync("I have updated " + user.Username + ".");
                }
                else
                {
                    newMember.ServerJoinDate = user.JoinedAt;
                    newMember.DiscordId = user.Id;
                    newMember.DiscordUserID = user.ToString();
                    newMember.DiscordUsername = user.Username;
                    newMember.Rank = "Guest";
                    newMember.JoinedFaction = false;
                    _db.Users.Add(newMember);
                    _db.SaveChanges();
                    await channel.SendMessageAsync(user.Username + " was successfully added as a Guest.");
                }
                var role = user.Guild.Roles.FirstOrDefault(x => x.Id.Equals(Epsilon.ConfigFile.GuestRoleID));
                await user.AddRoleAsync(role);
                role = user.Guild.Roles.FirstOrDefault(x => x.Name.Equals("Neutral", StringComparison.OrdinalIgnoreCase));
                await user.AddRoleAsync(role);
                await user.Guild.GetTextChannel(Epsilon.ConfigFile.BotSpamChannelID).SendMessageAsync("Welcome " + user.Mention + ".  " +
                    "If you are interested in joining " + Epsilon.ConfigFile.OrganizationName + " please contact an admin to assist you in getting " +
                    "authorized to join.");
            });
        }
        //Methods
        private void SaveUser(User guest, DatabaseContext db)
        {
            try
            {
                db.Users.Update(guest);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(guest.DiscordUsername + " failed to retrieve from the database. " + e.Message);
            }
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
                Console.WriteLine("User joined, yet I was unable to locate " + user.Username + "." + e.Message);
                return null;
            }
        }
    }
}

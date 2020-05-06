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
                var adminRole = user.Guild.GetRole(378974754884943882);
                var channel = user.Guild.GetTextChannel(370226644574666755);
                _db = new DatabaseContext();
                var foundUser = SearchGuest(user);
                await channel.SendMessageAsync(adminRole.Mention + $", {user.Username} has joined the server.");
                if (foundUser != null && foundUser.DiscordId == user.Id)
                {
                    await channel.SendMessageAsync("I found a user:  " + user.Username + ".");
                    foundUser.ServerJoinDate = user.JoinedAt;
                    foundUser.Rank = "Guest";
                    foundUser.UserID = user.ToString();
                    foundUser.Username = user.Username;
                    SaveGuest(foundUser, _db);
                    await channel.SendMessageAsync("I have updated the user.");
                }
                else
                {
                    newMember.ServerJoinDate = user.JoinedAt;
                    newMember.DiscordId = user.Id;
                    newMember.UserID = user.ToString();
                    newMember.Username = user.Username;
                    newMember.Rank = "Guest";
                    newMember.JoinedFaction = false;
                    _db.Users.Add(newMember);
                    _db.SaveChanges();
                }
                var role = user.Guild.Roles.Where(x => x.Name.ToUpper() == "Guest".ToUpper());
                await user.AddRolesAsync(role);
            });
        }
        private void SaveGuest(User guest, DatabaseContext db)
        {
            try
            {
                db.Users.Update(guest);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine("Guest Failed to retrieve. " + e.Message);
            }
        }
        private User SearchGuest(SocketGuildUser user)
        {
            var db = new DatabaseContext();
            try
            {
                return db.Users.Single(x => x.DiscordId == user.Id);
            }
            catch (Exception e)
            {
                Console.WriteLine("User joined, but was unable to search guest. " + e.Message);
                return null;
            }
        }
    }
}

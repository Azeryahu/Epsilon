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
            return Task.Run(() =>
            {
                var role = user.Guild.Roles.Where(has => has.Name.ToUpper() == "Neutral".ToUpper());
                var newUser = new Visitor();

                newUser.ServerJoinDate = user.JoinedAt;
                newUser.CompletedMissions = 0;
                newUser.PersonalStanding = 0;
                newUser.OrganizationStanding = 0;
                newUser.AllianceStanding = 0;
                newUser.UserID = user.Username;
                newUser.OrganizationName = "";
                newUser.AllianceName = "";
                newUser.Title = "Neutral";

                _db.Visitors.Add(newUser);
                _db.SaveChanges();
            });
        }
    }
}

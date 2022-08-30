using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epsilon
{
    public class Config
    {
        public string Token { get; set; }
        public ulong BotSpamChannelID { get; set; }
        public ulong AnnouncementChannelID { get; set; }
        public ulong SecureSpamChannelID { get; set; }
        public ulong BotLogChannelID { get; set; }
        public ulong BotMasterID { get; set; }
        public ulong SeniorOfficerRoleID { get; set; }
        public ulong OfficerRoleID { get; set; }
        public ulong CaptainRoleID { get; set; }
        public ulong AdministratorRoleID { get; set; }
        public ulong ModeratorRoleID { get; set; }
        public ulong DiscordStaffID { get; set; }
        public ulong DualUniverseActiveID { get; set; }
        public ulong DualUniverseHiatusID { get; set; }
        public ulong OrganizationMemberID { get; set; }
        public ulong BotID { get; set; }
        public ulong GuestRoleID { get; set; }
        public string OrganizationName { get; set; }
        public int HiatusCheckDays { get; set; }
        public int HiatusDays { get; set; }
        public string DatabaseString { get; set; }
        public int ResponseTimeTimeout { get; set; }
        public List<string> StandingRolesList { get; set; }
        public Dictionary<string, string> RanksDictionary { get; set; }
    }
}

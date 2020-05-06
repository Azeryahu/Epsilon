using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epsilon
{
    public class User
    {
        //User properties
        public DateTimeOffset? ServerJoinDate { get; set; }
        public int ID { get; set; }
        public int NumberOfWarnings { get; set; }
        public int NumberOfAttempts { get; set; }
        public float PersonalStanding { get; set; }
        public ulong DiscordId { get; set; }
        public string DualUsername { get; set; }
        public string SteamID { get; set; }
        public string SteamUsername { get; set; }
        public string UserID { get; set; }
        public string Username { get; set; }
        public string VerificationKey { get; set; }
        //Faction poperties
        public DateTimeOffset? FactionJoinDate { get; set; }
        public DateTimeOffset? PromotionDate { get; set; }
        public int PointBalance { get; set; }
        public int DaysUntilPromotion { get; set; }
        public int CompletedMissions { get; set; }
        public string Branch { get; set; }
        public string Rank { get; set; }
        public string Grade { get; set; }
        public bool NDAVerified { get; set; }
        public bool CanJoin { get; set; }
        public bool JoinedFaction { get; set; }
        //Op properties
        public string CurrentOpName { get; set; }
        public string CurrentOpType { get; set; }
    }
}

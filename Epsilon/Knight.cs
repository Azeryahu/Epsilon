using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epsilon
{
    public class Knight
    {
        public DateTimeOffset? ServerJoinDate { get; set; }
        public DateTimeOffset? EnlistmentDate { get; set; }
        public DateTimeOffset? OfficerGradDate { get; set; }
        public int ID { get; set; }
        public int PointBalance { get; set; }
        public int DaysUntilPromotion { get; set; }
        public int CompletedMissions { get; set; }
        public decimal PersonalStanding { get; set; }
        public decimal OrganizationStanding { get; set; }
        public decimal AllianceStanding { get; set; }
        public string UserID { get; set; }
        public string Username { get; set; }
        public string RankTitle { get; set; }
        public string OrganizationName { get; set; }
        public string AllianceName { get; set; }
        public bool Verified { get; set; }
    }
}

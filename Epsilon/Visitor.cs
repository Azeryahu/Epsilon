using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epsilon
{
    public class Visitor
    {
        public DateTimeOffset? ServerJoinDate { get; set; }
        public int ID { get; set; }
        public int CompletedMissions { get; set; }
        public decimal PersonalStanding { get; set; }
        public decimal OrganizationStanding { get; set; }
        public decimal AllianceStanding { get; set; }
        public string UserID { get; set; }
        public string Username { get; set; }
        public string OrganizationName { get; set; }
        public string AllianceName { get; set; }
        public string Title { get; set; }
        public bool Verified { get; set; }
        public bool Joined { get; set; }
    }
}

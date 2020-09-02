using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epsilon
{
    public class Mission
    {
        public int ID { get; set; }
        public DateTimeOffset? DateFormed { get; set; }
        public ulong MissionLeadID { get; set; }
        public string MissionLeadUsername { get; set; }
        public bool MissionActiveStatus { get; set; }
        public bool MissionStartedStatus { get; set; }
        public int NumberOfMembers { get; set; }
        public DateTimeOffset? StartDateTime { get; set; }
        public DateTimeOffset? EndDateTime { get; set; }
        public string MissionName { get; set; }
        public string MissionType { get; set; }
        public string Description { get; set; }
        public string Coordinates { get; set; }
    }
}

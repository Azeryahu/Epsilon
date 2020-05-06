using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epsilon
{
    public class Job
    {
        public int ID { get; set; }
        public DateTimeOffset? DateCreated { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public string JobClass { get; set; }
        public string JobType { get; set; }
        public string Description { get; set; }
        public string Coordinates { get; set; }
        public ulong JobHeadID { get; set; }
        public string JobHeadUsername { get; set; }
        public bool JobStatus { get; set; }
        public int NumberOfGroups { get; set; }
        public int TotalNumberOfSlots { get; set; }

        public class Group
        {
            public int ID { get; set; }
            public ulong GroupLeadID { get; set; }
            public string GroupLeadUsername { get; set; }
            public int NumberOfTeams { get; set; }
            public bool GroupStatus { get; set; }
            public string GroupName { get; set; }
            public string JobClass { get; set; }
            public string JobType { get; set; }

            public class Team
            {
                public int ID { get; set; }
                public ulong TeamLeadID { get; set; }
                public string TeamLeadUsername { get; set; }
                public int NumbeOfSlots { get; set; }
                public bool TeamStatus { get; set; }
                public string TeamName { get; set; }
                public string JobClass { get; set; }
                public string JobType { get; set; }
            }
        }
    }
}

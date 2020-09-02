using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epsilon
{
    public class Op
    {
        public int ID { get; set; }
        public DateTimeOffset? DateFormed { get; set; }
        public ulong OpLeadID { get; set; }
        public string OpLeadUsername { get; set; }
        public bool OpActiveStatus { get; set; }
        public bool OpStartedStatus { get; set; }
        public int NumberOfMembers { get; set; }
        public double Duration { get; set; }
        public DateTimeOffset? StartDateTime { get; set; }
        public DateTimeOffset? EndDateTime { get; set; }
        public string OpName { get; set; }
        public string OpType { get; set; }
        public string Description { get; set; }
        public string Coordinates { get; set; }

        public class Group
        {
            public int ID { get; set; }
            public int OpID { get; set; }
            public ulong GroupLeadID { get; set; }
            public string GroupLeadUsername { get; set; }
            public string GroupName { get; set; }
            public bool GroupStatus { get; set; }
            public string OpName { get; set; }
            public string OpType { get; set; }
            public int NumberOfMembers { get; set; }

            public class Team
            {
                public int ID { get; set; }
                public int OpID { get; set; }
                public ulong TeamLeadID { get; set; }
                public string TeamLeadUsername { get; set; }
                public string TeamName { get; set; }
                public bool TeamStatus { get; set; }
                public string OpName { get; set; }
                public string OpType { get; set; }
                public int NumberOfMembers { get; set; }
            }
        }
    }
}

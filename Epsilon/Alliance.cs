using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epsilon
{
    public class Alliance
    {
        public DateTimeOffset? AllianceFoundedDate { get; set; }
        public DateTimeOffset? AllianceDisbandedDate { get; set; }
        public int ID { get; set; }
        public int OrganizationCount { get; set; }
        public int MemberCount { get; set; }
        public decimal AllianceStanding { get; set; }
        public string AllianceName { get; set; }
        public string AllianceID { get; set; }
    }
}

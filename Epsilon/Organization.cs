using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epsilon
{
    public class Organization
    {
        public DateTimeOffset? OrganizationFoundedDate { get; set; }
        public DateTimeOffset? OrganizationDisbandedDate { get; set; }
        public int ID { get; set; }
        public int MemberCount { get; set; }
        public decimal OrganizationStanding { get; set; }
        public string OrganiztionName { get; set; }
        public string OrganizationID { get; set; }
        public string AllianceID { get; set; }
    }
}

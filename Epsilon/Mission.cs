using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epsilon
{
    public class Mission
    {
        public DateTimeOffset? MissionStartDateTime { get; set; }
        public DateTimeOffset? BonusEndDateTime { get; set; }
        public DateTimeOffset? MissionCompletedDateTime { get; set; }
        public DateTimeOffset? TennetiveCompletionDateTime { get; set; }
        public int ID { get; set; }
        public decimal StandingPercentIncrease { get; set; }
        public string UserID { get; set; }
        public string MissionName { get; set; }
        public string MissionReward { get; set; }
    }
}

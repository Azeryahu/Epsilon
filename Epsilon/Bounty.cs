using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epsilon
{
    public class Bounty
    {
        public int ID { get; set; }
        public DateTimeOffset? DateTimeCreated { get; set; }
        public DateTimeOffset? DateTimeCompleted { get; set; }
        public DateTimeOffset? TimeoutDateTime { get; set; }
        public bool Completed { get; set; }
        public string TargetName { get; set; }
        public float Payout { get; set; }
        public int PointValue { get; set; }
        public string BonusPayout { get; set; }
    }
}

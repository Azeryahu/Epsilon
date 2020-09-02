using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epsilon
{
    public class Crew
    {
        public int ID { get; set; }
        public DateTimeOffset? DateTimeFormed { get; set; }
        public DateTimeOffset? DateTimeDisbanded { get; set; }
        public bool CrewStatus { get; set; }
        public ulong CaptainID { get; set; }
        public string CaptainUsername { get; set; }
        public int NumberOfCrew { get; set; }
        public string CrewName { get; set; }
    }
}

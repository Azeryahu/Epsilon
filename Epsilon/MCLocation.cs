using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epsilon
{
    public class MCLocation
    {
        public int ID { get; set; }
        public DateTimeOffset? DateCreated { get; set; }
        public DateTimeOffset? DateLooted { get; set; }
        public ulong Discoverer { get; set; }
        public ulong Looter { get; set; }
        public int XCoordinate { get; set; }
        public int YCoordinate { get; set; }
        public int ZCoordinate { get; set; }
        public string ThingFound { get; set; }
        public string MinecraftID { get; set; }

    }
}

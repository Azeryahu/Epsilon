using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epsilon
{
    public class MissionXRef
    {
        public int ID { get; set; }
        public int MissionID { get; set; }
        public ulong UserID { get; set; }
        public string Username { get; set; }
        public bool Valid { get; set; }
        public bool Ready { get; set; }
    }
}

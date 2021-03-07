using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelineCacher.Entities
{
    public class User
    {
        public string id { get; set; }
        public string Fullname { get; set; }
        public AadUser[] AadUsers { get; set; }
        public List<Group> GroupMemberships { get; set; }
    }
}

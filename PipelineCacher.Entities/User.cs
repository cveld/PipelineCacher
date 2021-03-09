using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelineCacher.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Fullname { get; set; }
        public ICollection<AadUser> AadUsers { get; set; }
        public ICollection<Group> GroupMemberships { get; set; }
    }
}

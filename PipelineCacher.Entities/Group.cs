using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelineCacher.Entities
{
    /// <summary>
    /// Represents a group which can have users and other groups as member
    /// </summary>
    public class Group
    {
        public int Id { get; set; }
        public List<Group> Groups { get; set; }
        public List<User> Users { get; set; }
    }
}

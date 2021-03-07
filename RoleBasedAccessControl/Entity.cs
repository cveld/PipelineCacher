using System;
using System.Collections.Generic;
using System.Text;

namespace RoleBasedAccessControl
{
    public class Entity
    {
        public int Id { get; set; }
        public int EntityId { get; set; }
        public string EntityType { get; set; }
    }
}

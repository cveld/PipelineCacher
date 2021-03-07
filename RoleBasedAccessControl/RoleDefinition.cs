using System;
using System.Collections.Generic;
using System.Text;

namespace RoleBasedAccessControl
{
    public class RoleDefinition
    {
        public int Id { get; set; }
        public List<Action> Actions { get; set; }
        public List<Action> NotActions { get; set; }
    }
}

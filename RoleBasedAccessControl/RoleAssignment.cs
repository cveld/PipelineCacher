using System;

namespace RoleBasedAccessControl
{
    public class RoleAssignment
    {
        public int Id { get; set; }
        public Entity Entity { get; set; }
        public RoleDefinition RoleDefinition { get; set; }
        public Principal Principal { get; set; }
    }
}

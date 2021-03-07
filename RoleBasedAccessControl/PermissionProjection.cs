using System;
using System.Collections.Generic;
using System.Text;

namespace RoleBasedAccessControl
{
    public class PermissionProjection
    {
        public int Id { get; set; }
        public Action Action { get; set; }
        public Permission Permission { get; set; }
    }
}

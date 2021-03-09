using System;
using System.Collections.Generic;
using System.Text;

namespace RoleBasedAccessControl
{
    public class RoleDefinition
    {
        public int Id { get; set; }
        public ICollection<RoleDefinitionAction> RoleDefinitionActions { get; set; }        
    }
}

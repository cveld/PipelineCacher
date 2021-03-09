using System;
using System.Collections.Generic;
using System.Text;

namespace RoleBasedAccessControl
{
    public class RoleDefinitionAction
    {
        public int Id { get; set; }
        public int RoleDefinitionId { get; set; }
        public RoleDefinition RoleDefinition { get; set; }
        public int ActionId { get; set; }
        public Action Action { get; set; }
        /// <summary>
        /// True when the Action is meant as negating
        /// </summary>
        public bool Not { get; set; }
    }
}

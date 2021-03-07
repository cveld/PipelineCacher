using System;
using System.Collections.Generic;
using System.Text;

namespace RoleBasedAccessControl
{
    public class EntityAncestor
    {
        public int Id { get; set; }
        public int EntityId { get; set; }
        public int AncestorEntityId { get; set; }
        public string EntityType { get; set; }
        public string AncestorEntityType { get; set; }
        /// <summary>
        /// 0 is closest, i.e. its immediate parent. higher is farther away, i.e. 1 is its parent parent
        /// </summary>
        public int Level { get; set; }
    }
}

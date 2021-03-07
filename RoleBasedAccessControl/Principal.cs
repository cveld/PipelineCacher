using System;
using System.Collections.Generic;
using System.Text;

namespace RoleBasedAccessControl
{
    /// <summary>
    /// Principal object
    /// </summary>
    public class Principal
    {
        public int Id { get; set; }
        public int PrincipalId { get; set; }
        /// <summary>
        /// PrincipalType can be either User, or Group
        /// </summary>
        public string PrincipalType { get; set; }
    }
}

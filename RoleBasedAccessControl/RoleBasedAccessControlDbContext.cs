using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoleBasedAccessControl
{
    public class RoleBasedAccessControlDbContext : DbContext
    {
        public RoleBasedAccessControlDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<RoleAssignment> RoleAssignments { get; set; }
        public DbSet<RoleDefinition> RoleDefinitions { get; set; }
        public DbSet<Principal> Principals { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<PermissionProjection> PermissionProjections { get; set; }
        public DbSet<EntityAncestor> EntityAncestors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}

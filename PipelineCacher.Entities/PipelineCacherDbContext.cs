using Microsoft.EntityFrameworkCore;
using RoleBasedAccessControl;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace PipelineCacher.Entities
{
    public class PipelineCacherDbContext : RoleBasedAccessControlDbContext
    {
        public PipelineCacherDbContext(DbContextOptions<PipelineCacherDbContext> options) : base(options)
        {
        }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Pat> Pats { get; set; }
        public DbSet<Pipeline> Pipelines { get; set; }
        public DbSet<PipelineContext> PipelineContexts { get; set; }
        public DbSet<PipelineState> PipelineStates { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<AuditLog> AuditLog { get; set; }
        public DbSet<Sourcecode> Sourcecode { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<AadUser> AadUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            Assembly assemblyWithConfigurations = GetType().Assembly;
            modelBuilder.ApplyConfigurationsFromAssembly(assemblyWithConfigurations);
        }
    }
}

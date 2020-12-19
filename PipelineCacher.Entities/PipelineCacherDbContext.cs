using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace PipelineCacher.Entities
{
    public class PipelineCacherDbContext : DbContext
    {
        public PipelineCacherDbContext(DbContextOptions<PipelineCacherDbContext> options) : base(options)
        {
        }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Pat> Pats { get; set; }
        public DbSet<Pipeline> Pipelines { get; set; }
        public DbSet<PipelineContext> PipelineContexts { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<AuditLog> AuditLog { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            Assembly assemblyWithConfigurations = GetType().Assembly;
            modelBuilder.ApplyConfigurationsFromAssembly(assemblyWithConfigurations);
        }
    }
}

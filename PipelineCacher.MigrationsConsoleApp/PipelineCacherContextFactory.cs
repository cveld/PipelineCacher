using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using PipelineCacher.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineCacher.MigrationsConsoleApp
{
    public class PipelineCacherContextFactory //: IDesignTimeDbContextFactory<PipelineCacherContext>
    {
        public PipelineCacherDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PipelineCacherDbContext>();
            //optionsBuilder.UseSqlServer(Configuration.GetConnectionString("PipelineCacherDatabase")));
            return new PipelineCacherDbContext(optionsBuilder.Options);
        }
    }
}

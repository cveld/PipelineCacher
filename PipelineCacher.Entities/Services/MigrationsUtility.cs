using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineCacher.Entities.Services
{
    public class MigrationsUtility
    {
        private readonly PipelineCacherDbContext context;
        bool tested = false;
        public MigrationsUtility(PipelineCacherDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<string>> GetPendingMigrations()
        {
            var result = await context.Database.GetPendingMigrationsAsync();
            return result;
        }
    }
}

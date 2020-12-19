using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PipelineCacher.Entities;
using PipelineCacher.Entities.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipelineCacher.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class MigrationsUtilityController : Controller
    {
        private readonly PipelineCacherDbContext context;

        public MigrationsUtilityController(PipelineCacherDbContext context)
        {
            this.context = context;
        }
        [HttpGet]
        public async Task<IEnumerable<string>> Index()
        {
            return await context.Database.GetPendingMigrationsAsync();
        }

        [HttpPost("ApplyMigrations")]
        public async Task<IEnumerable<string>> ApplyMigrations()
        {
            context.Database.Migrate();
            return await context.Database.GetPendingMigrationsAsync();
        }
    }
}

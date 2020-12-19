using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineCacher.Entities
{
    static public class DbInitializer
    {
        public static void Initialize(PipelineCacherDbContext context)
        {
            //context.Database.EnsureCreated();
            //context.Database.Migrate();
        }
    }
}

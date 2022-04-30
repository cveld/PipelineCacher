using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PipelineCacher.Entities;
using RoleBasedAccessControl;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PipelineCacher.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoleAssignmentsController : ControllerBase
    {
        private readonly PipelineCacherDbContext context;

        public RoleAssignmentsController(PipelineCacherDbContext pipelineCacherDbContext)
        {
            this.context = pipelineCacherDbContext;
        }

        [HttpGet]
        public IEnumerable<RoleAssignment> GetRoleAssignments()
        {
            return context.RoleAssignments;
        }
    }
}

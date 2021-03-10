﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PipelineCacher.Entities;
using PipelineCacher.Server.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace PipelineCacher.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UserProfileController : ControllerBase
    {
        private PipelineCacherDbContext context;

        public UserProfileController(PipelineCacherDbContext context)
        {
            this.context = context;
        }
        [HttpPost]
        [Authorize]
        public async Task<string> GetOrCreateUserProfile()
        {
            //var cp = this.User.Identity as ClaimsIdentity;
            var aaduser = context.AadUsers.Include(a => a.User).SingleOrDefault(a => a.Name == this.User.Identity.Name);            
            if (aaduser == null)
            {
                var newuser = new User
                {
                    AadUsers = new List<AadUser>()
                };
                var newaaduser = new AadUser
                {
                    Name = this.User.Identity.Name,
                    //User = newuser
                };
                newuser.AadUsers.Add(newaaduser);
                context.Users.Add(newuser);
                //context.AadUsers.Add(newaaduser);
                await context.SaveChangesAsync();
                var json = Serializers.ObjectToJson(newuser);
                return json;
            }
            //var json = Serializers.ObjectToJson(cp.Claims);
            return Serializers.ObjectToJson(aaduser.User);
        }
        

    }
}
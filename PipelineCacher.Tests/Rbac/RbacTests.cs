using PipelineCacher.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PipelineCacher.Tests.Rbac
{
    public class RbacTests
    {
        [Fact]
        public void GroupHierarchicalMembership()
        {
            var user1 = new User
            {
                Fullname = "user1",
                GroupMemberships = new List<Group>()
            };

            var group1 = new Group
            {
                Users = new List<User> { user1 }
            };
            user1.GroupMemberships.Add(group1);

            var group2 = new Group
            {
                Groups = new List<Group> { group1 }
            };




        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipelineCacher.Client.Features.User
{
    using BlazorState;

    public partial class UserState
    {
        public class UpdateUserAction : IAction
        {
            public Entities.User User{ get; set; }
        }
    }
}

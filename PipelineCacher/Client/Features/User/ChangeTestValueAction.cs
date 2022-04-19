using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipelineCacher.Client.Features.User
{
    using BlazorState;

    public partial class UserState
    {
        public class ChangeTestValueAction : IAction
        {
            public string Value { get; set; }
        }
    }
}

using BlazorState;
using PipelineCacher.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipelineCacher.Client.Features.User
{
    public partial class UserState : State<UserState>
    {      
        public event Action OnChange;
        private void NotifyStateChanged() => OnChange?.Invoke();

        public override void Initialize()
        {
            Test = "This is before logging in";
        }

        public string Test { get; private set; }

        public Entities.User User { get; private set; }
    }
}

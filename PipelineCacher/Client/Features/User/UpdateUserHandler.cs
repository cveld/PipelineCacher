using BlazorState;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PipelineCacher.Client.Features.User
{
    public partial class UserState
    {
        public class UpdateUserHandler : ActionHandler<UpdateUserAction>
        {
            public UpdateUserHandler(IStore aStore) : base(aStore) { }

            UserState UserState => Store.GetState<UserState>();

            public override Task<Unit> Handle(UpdateUserAction changeTestValueAction, CancellationToken aCancellationToken)
            {
                UserState.User = changeTestValueAction.User;
                return Unit.Task;
            }
        }
    }
}

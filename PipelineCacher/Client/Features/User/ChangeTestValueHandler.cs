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
        public class ChangeTestValueHandler : ActionHandler<ChangeTestValueAction>
        {
            public ChangeTestValueHandler(IStore aStore) : base(aStore) { }

            UserState UserState => Store.GetState<UserState>();

            public override Task<Unit> Handle(ChangeTestValueAction changeTestValueAction, CancellationToken aCancellationToken)
            {
                UserState.Test = changeTestValueAction.Value;
                return Unit.Task;
            }
        }
    }
}

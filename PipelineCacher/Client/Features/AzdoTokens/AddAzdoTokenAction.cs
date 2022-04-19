using BlazorState;
using MediatR;
using PipelineCacher.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PipelineCacher.Client.Features.AzdoTokens
{
    public partial class AzdoTokenState {
        public class AddAzdoTokenHandler : ActionHandler<AddAzdoTokenAction>
        {
            public AddAzdoTokenHandler(IStore aStore) : base(aStore) { }
            AzdoTokenState AzdoTokenState => Store.GetState<AzdoTokenState>();

            public override Task<Unit> Handle(AddAzdoTokenAction aAction, CancellationToken aCancellationToken)
            {
                AzdoTokenState.AzdoTokens.Add(aAction.AzdoTokenProjection);
                return Unit.Task;
            }
        }
    }
}

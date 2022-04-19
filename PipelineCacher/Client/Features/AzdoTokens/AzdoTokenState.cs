using BlazorState;
using PipelineCacher.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipelineCacher.Client.Features.AzdoTokens
{
    public partial class AzdoTokenState : State<AzdoTokenState>
    {
        public List<AzdoTokenProjection> AzdoTokens { get; private set; }

        public override void Initialize()
        {
            AzdoTokens = new List<AzdoTokenProjection>();
        }
    }
}

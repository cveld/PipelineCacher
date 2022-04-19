using BlazorState;
using PipelineCacher.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipelineCacher.Client.Features.AzdoTokens
{
    public partial class AzdoTokenState {
        public class AddAzdoTokenAction : IAction
        {
            public AzdoTokenProjection AzdoTokenProjection { get; set; }
        }
    }
}

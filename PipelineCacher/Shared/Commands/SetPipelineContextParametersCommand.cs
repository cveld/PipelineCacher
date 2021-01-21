using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelineCacher.Shared.Commands
{
    public class SetPipelineContextParametersCommand
    {
        public Dictionary<string, string> Parameters { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineCacher.Entities
{
    public class PipelineAutomationContext
    {
        public int Id { get; set; }
        public ICollection<PipelineContext> PipelineContexts { get; set; }
    }
}

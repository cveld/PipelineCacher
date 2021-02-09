using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineCacher.Entities
{
    public class PipelineAutomation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> Environments { get; set; }
        /// <summary>
        /// One PipelineAutomationContext object per environment
        /// </summary>
        public ICollection<PipelineAutomationContext> PipelineAutomationContexts { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineCacher.Entities
{
    [Keyless]
    public class Stage
    {
        public string Content { get; set; }
        public int? PipelineRunId { get; set; }
        public DateTime? ValidationTimestamp { get; set; }
        public StatusEnum Status { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineCacher.Entities
{
    public record Stage
    { 
        public string Content { get; init; }
        public int? PipelineRunId { get; init; }
        public DateTime? ValidationTimestamp { get; init; }
        public StatusEnum Status { get; init; }
    }
}

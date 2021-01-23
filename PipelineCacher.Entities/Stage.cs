using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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
        [JsonConverter(typeof(StringEnumConverter))]
        public StatusEnum Status { get; init; }
    }
}

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
        public int Position { get; init; }
        public string Name { get; init; }
        public string DisplayName { get; init; }
        public string Content { get; init; }
        public int? PipelineRunId { get; init; }
        public DateTime? ValidationTimestamp { get; init; }
        //[JsonConverter(typeof(StringEnumConverter))]
        public StatusEnum Status { get; init; }

        public string CalculateDisplayName()
        {
            if (!string.IsNullOrEmpty(DisplayName))
            {
                return DisplayName;
            }
            if (!string.IsNullOrEmpty(Name))
            {
                return Name;
            }

            return $"Stage {Position + 1}";
        }
    }
}

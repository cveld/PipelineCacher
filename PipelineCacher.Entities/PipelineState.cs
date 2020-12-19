using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineCacher.Entities
{
    /// <summary>
    /// A specific snapshot of a pipeline
    /// </summary>
    public class PipelineState : IEntityTypeConfiguration<PipelineState>
    {
        public int Id { get; set; }
        public Pipeline Pipeline { get; set; }
        public List<string> Stages { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        /// <summary>
        /// Revision number of the Azure Pipelines pipeline definition
        /// </summary>
        public int Revision { get; set; }
        public int AzdoBuildId { get; set; }

        public void Configure(EntityTypeBuilder<PipelineState> builder)
        {
            builder.ToTable("PipelineState");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Parameters)
                .HasConversion(v => JsonConvert.SerializeObject(v),
                v => v == null
                    ? new Dictionary<string, string>() // fallback
                    : JsonConvert.DeserializeObject<Dictionary<string, string>>(v));
            builder.Property(e => e.Stages)
                .HasConversion(v => JsonConvert.SerializeObject(v),
                v => v == null
                    ? new List<string>() // fallback
                    : JsonConvert.DeserializeObject<List<string>>(v));

        }
    }
}

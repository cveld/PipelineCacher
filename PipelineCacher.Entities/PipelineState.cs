using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace PipelineCacher.Entities
{
    /// <summary>
    /// A specific snapshot of a pipeline run. There is a precise relationship one-to-one with an Azure Pipelines run
    /// </summary>
    public class PipelineState : IEntityTypeConfiguration<PipelineState>
    {
        public int Id { get; set; }
        public Pipeline Pipeline { get; set; }        
        public string Branch { get; set; }
        public string Commit { get; set; }
        public string YamlPath { get; set; }
        public ImmutableList<Stage> Stages { get; set; }
        public SourcecodeTree SourcecodeTree { get; set; }
        public ImmutableDictionary<string, string> Parameters { get; set; }
        /// <summary>
        /// Revision number of the Azure Pipelines pipeline definition when the pipeline was run
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
                    ? ImmutableDictionary<string, string>.Empty // fallback
                    : JsonConvert.DeserializeObject<ImmutableDictionary<string, string>>(v));
            builder.Property(e => e.Stages)
                .HasConversion(v => JsonConvert.SerializeObject(v),
                v => v == null
                    ? ImmutableList<Stage>.Empty // fallback
                    : JsonConvert.DeserializeObject<ImmutableList<Stage>>(v));
            builder.Property(e => e.SourcecodeTree)
                .HasConversion(v => JsonConvert.SerializeObject(v),
                v => v == null
                    ? new SourcecodeTree() // fallback
                    : JsonConvert.DeserializeObject<SourcecodeTree>(v));
        }
    }
}

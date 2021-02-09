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
    /// PipelineContext represents a single run of a Pipeline against a particular environment
    /// Has access to:
    /// - The actual state of the pipeline (yaml, dependencies)
    /// - The last validated state
    /// </summary>
    public class PipelineContext : IEntityTypeConfiguration<PipelineContext>
    {
        public int Id { get; set; }
        public Pipeline Pipeline { get; set; }
        // TODO; refactor stuff from PipelineContext into PipelineState
        public PipelineState PipelineState { get; set; }
        public SourcecodeTree SourcecodeTree { get; set; }
        public ImmutableList<Stage> Stages { get; set; }
        public string CommitId { get; set; }
        public string Environment { get; set; }
        /// <summary>
        /// Inputs for the pipeline. ultimately steered from PipelineAutomation through PipelineAutomationContext mapping
        /// For now hard coded
        /// </summary>
        public ImmutableDictionary<string, string> Parameters { get; set; }
        public DateTime? LastChecked { get; set; }
        /// <summary>
        /// Ultimately taken from the PipelineAutomation and/or -Context objects
        /// </summary>
        public string TargetBranch { get; set; }
        public void Configure(EntityTypeBuilder<PipelineContext> builder)
        {
            builder.ToTable("PipelineContext");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Parameters)
                .HasConversion(v => JsonConvert.SerializeObject(v),
                v => v == null
                    ? ImmutableDictionary<string, string>.Empty // fallback
                    : JsonConvert.DeserializeObject<ImmutableDictionary<string, string>>(v));
            builder.Property(e => e.SourcecodeTree)
                .HasConversion(v => JsonConvert.SerializeObject(v),
                v => v == null
                    ? new SourcecodeTree() // fallback
                    : JsonConvert.DeserializeObject<SourcecodeTree>(v));
            builder.Property(e => e.Stages)
                .HasConversion(v => JsonConvert.SerializeObject(v),
                v => v == null
                    ? ImmutableList<Stage>.Empty // fallback
                    : JsonConvert.DeserializeObject<ImmutableList<Stage>>(v));
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineCacher.Entities
{
    /// <summary>
    /// PipelineContext represents a single run of a Pipeline against a particular environment
    /// 
    /// </summary>
    public class PipelineContext : IEntityTypeConfiguration<PipelineContext>
    {
        public int Id { get; set; }
        public Pipeline Pipeline { get; set; }
        public PipelineState PipelineState { get; set; }
        public string Environment { get; set; }
        /// <summary>
        /// Inputs for the pipeline. ultimately steered from PipelineAutomation through PipelineAutomationContext mapping
        /// For now hard coded
        /// </summary>
        public Dictionary<string, string> Parameters { get; set; }
        public DateTime LastChecked { get; set; }
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
                    ? new Dictionary<string, string>() // fallback
                    : JsonConvert.DeserializeObject<Dictionary<string, string>>(v));
        }
    }
}

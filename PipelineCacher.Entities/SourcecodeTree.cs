using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace PipelineCacher.Entities
{
    public class SourcecodeTree
    {
        /// <summary>
        /// Maps files in the pipeline repository to sourcecode content entries
        /// </summary>
        public ImmutableDictionary<string, string> Pipeline { get; set; } = ImmutableDictionary<string, string>.Empty;
    }
}

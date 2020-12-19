using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PipelineCacher.Shared
{
    public class PipelinePostModel
    {
        [Required]
        public string OrganizationName { get; set; }

        [Required]
        public string ProjectName { get; set; }
        [Required]
        public int? AzdoPipelineId { get; set; }
    }
}

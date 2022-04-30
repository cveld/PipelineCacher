using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelineCacher.Shared.Models
{
    public class AzdoPipelineRun
    {
        public int Id { get; set; }
        public string BuildNumber { get; set; }
        public AzdoBuildStatus Status { get; set; }
        public AzdoBuildResult Result { get; set; }

    }
}

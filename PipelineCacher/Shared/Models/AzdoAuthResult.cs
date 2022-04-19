using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelineCacher.Shared.Models
{
    public class AzdoAuthResult
    {
        public string Error { get; set; }
        public AzdoTokenProjection AzdoTokenProjection { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelineCacher.Shared.Models
{
    public class AzdoTokenProjection
    {
        public int Id { get; set; }
        public DateTime? Expires { get; set; }
        public int? ExpiresIn { get; set; }
    }
}

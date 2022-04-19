using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelineCacher.Entities
{
    public class AuthorizationRequest
    {
        public Guid guid { get; set; }
        public AzdoToken TokenModel { get; set; }
        public DateTime? Created { get; set; }
    }
}

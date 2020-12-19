using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipelineCacher.Client.Models
{
    public enum RestCallStatusEnum
    {
        NotDefined,
        NotStarted,
        Getting,
        Putting,
        Deleting,
        Posting,
        Ok,
        Failed
    }
}

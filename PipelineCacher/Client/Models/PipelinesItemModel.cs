using PipelineCacher.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipelineCacher.Client.Models
{
    public class PipelinesItemModel
    {
        public RestCallStatusEnum LoadingStatus { get; set; } = RestCallStatusEnum.Ok;
        public Pipeline Pipeline { get; set; }       
    }
}

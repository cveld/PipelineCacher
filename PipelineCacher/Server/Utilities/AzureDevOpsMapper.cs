using Microsoft.TeamFoundation.Build.WebApi;
using PipelineCacher.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelineCacher.Server.Utilities
{
    public class AzureDevOpsMapper
    {
        static public StatusEnum MapStatus(TaskResult taskResult)
        {
            return taskResult switch
            {
                TaskResult.Succeeded => StatusEnum.RunSuccessfully,
                TaskResult.SucceededWithIssues => StatusEnum.RunSuccessfullyWithIssues,
                TaskResult.Failed => StatusEnum.RunFailed,
                TaskResult.Canceled => StatusEnum.RunCanceled,
                TaskResult.Skipped => StatusEnum.NotRun,
                TaskResult.Abandoned => StatusEnum.RunAbandoned,
                _ => throw new ArgumentOutOfRangeException(nameof(taskResult)),
            };
        }
    }
}

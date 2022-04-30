using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PipelineCacher.Shared.Models
{
    public enum AzdoBuildStatus
    {
        //
        // Summary:
        //     No status.
        [EnumMember]
        None = 0,
        //
        // Summary:
        //     The build is currently in progress.
        [EnumMember]
        InProgress = 1,
        //
        // Summary:
        //     The build has completed.
        [EnumMember]
        Completed = 2,
        //
        // Summary:
        //     The build is cancelling
        [EnumMember]
        Cancelling = 4,
        //
        // Summary:
        //     The build is inactive in the queue.
        [EnumMember]
        Postponed = 8,
        //
        // Summary:
        //     The build has not yet started.
        [EnumMember]
        NotStarted = 0x20,
        //
        // Summary:
        //     All status.
        [EnumMember]
        All = 47
    }
}

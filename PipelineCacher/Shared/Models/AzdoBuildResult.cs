using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PipelineCacher.Shared.Models
{
    public enum AzdoBuildResult
    {
        None = 0,
        //
        // Summary:
        //     The build completed successfully.
        [EnumMember]
        Succeeded = 2,
        //
        // Summary:
        //     The build completed compilation successfully but had other errors.
        [EnumMember]
        PartiallySucceeded = 4,
        //
        // Summary:
        //     The build completed unsuccessfully.
        [EnumMember]
        Failed = 8,
        //
        // Summary:
        //     The build was canceled before starting.
        [EnumMember]
        Canceled = 0x20
    }
}

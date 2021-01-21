using Microsoft.TeamFoundation.Build.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace PipelineCacher.Server.AzureDevOps
{
    public class CustomBuild : Build
    {
		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		public IDictionary<string, string> TemplateParameters
		{
			get;
			set;
		}
	}
}

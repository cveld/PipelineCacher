using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineCacher.Entities
{
    /// <summary>
    /// A Pipeline object represents the latest state of an Azure Pipelines pipeline
    /// - yaml file
    /// - stages
    /// - parameters
    /// - dependencies
    /// </summary>
    public class Pipeline
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string OrganizationName { get; set; }
        public string ProjectName { get; set; }
        public int AzdoId { get; set; }        
        public string RepositoryId { get; set; }
        public string YamlPath { get; set; }       
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineCacher.Entities
{
    public class Sourcecode
    {
        public int Id { get; set; }
        public ProviderType ProviderType { get; set; }
        public string RepositoryId { get; set; }
        public string CommitId { get; set; }
        public string Content { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineCacher.Entities
{
    /// <summary>
    /// Stores an action with a Pat
    /// </summary>
    public class AuditLog
    {
        public int Id { get; set; }
        public int PatId { get; set; }
        public string Action { get; set; }
        public int? SubjectEntityId { get; set; }
        public string SubjectEntityName { get; set; }
        public int? ObjectEntityId { get; set; }
        public string ObjectEntityName { get; set; }
    }
}

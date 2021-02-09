using System.ComponentModel.DataAnnotations;

namespace PipelineCacher.Shared.Commands
{
    public class CreatePipelineContextCommand
    {
        [Required]
        public int? PatId { get; set; }
        [Required]
        public string Environment { get; set; }
        [Required]
        public int? PipelineId { get; set; }
        [Required]
        public string TargetBranch { get; set; }
    }
}
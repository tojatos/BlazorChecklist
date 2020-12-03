using System.ComponentModel.DataAnnotations;

namespace BlazorChecklist.Shared
{
    public class Checklist
    {
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
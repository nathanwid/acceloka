using System.ComponentModel.DataAnnotations;

namespace Acceloka.Models
{
    public class CategoryModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}

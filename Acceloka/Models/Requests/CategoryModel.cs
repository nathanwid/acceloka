using System.ComponentModel.DataAnnotations;

namespace Acceloka.Models.Requests
{
    public class CategoryModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}

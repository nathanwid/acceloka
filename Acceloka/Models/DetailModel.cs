using System.ComponentModel.DataAnnotations;

namespace Acceloka.Models
{
    public class DetailModel
    {
        [Required]
        public string TicketCode { get; set; } = string.Empty;
        [Required]
        public int Quantity { get; set; }
    }
}

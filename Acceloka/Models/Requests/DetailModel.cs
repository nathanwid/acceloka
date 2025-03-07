using System.ComponentModel.DataAnnotations;

namespace Acceloka.Models.Requests
{
    public class DetailModel
    {
        [Required]
        public string TicketCode { get; set; } = string.Empty;
        [Required]
        public int Quantity { get; set; }
    }
}

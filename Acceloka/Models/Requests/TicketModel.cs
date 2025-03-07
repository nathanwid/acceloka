using System.ComponentModel.DataAnnotations;

namespace Acceloka.Models.Requests
{
    public class TicketModel
    {
        [Required]
        public string ticketCode { get; set; } = string.Empty;
        [Required]
        public string ticketName { get; set; } = string.Empty;
        [Required]
        public int Price { get; set; }
        [Required]
        public int Quota { get; set; }
        [Required]
        public DateTime eventDate { get; set; }
        [Required]
        public string CategoryName { get; set; } = string.Empty;
    }
}

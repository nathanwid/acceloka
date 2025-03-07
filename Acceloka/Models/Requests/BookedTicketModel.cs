using System.ComponentModel.DataAnnotations;

namespace Acceloka.Models.Requests
{
    public class BookedTicketModel
    {
        [Required]
        public DateTime BookingDate { get; set; }
        [Required]
        public List<DetailModel> Tickets { get; set; } = new List<DetailModel>();
    }
}

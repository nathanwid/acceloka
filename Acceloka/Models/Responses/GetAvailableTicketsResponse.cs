
namespace Acceloka.Models.Responses
{
    public class GetAvailableTicketsResponse
    {
        public String EventDate { get; set; } = string.Empty;
        public int Quota { get; set; }
        public string TicketCode { get; set; } = string.Empty;
        public string TicketName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int Price { get; set; }
    }
}

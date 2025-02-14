namespace Acceloka.Models.Responses
{
    public class GetBookedTicketsResponse
    {
        public int QtyPerCategory { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public List<TicketDetailResponse> Tickets { get; set; } = new List<TicketDetailResponse>();
    }
}

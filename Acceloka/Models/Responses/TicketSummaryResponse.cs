namespace Acceloka.Models.Responses
{
    public class TicketSummaryResponse
    {
        public string TicketCode { get; set; } = string.Empty;
        public string TicketName { get; set; } = string.Empty;
        public int Price { get; set; }
    }
}

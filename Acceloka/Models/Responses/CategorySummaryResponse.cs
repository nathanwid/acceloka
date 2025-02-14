namespace Acceloka.Models.Responses
{
    public class CategorySummaryResponse
    {
        public string CategoryName { get; set; } = string.Empty;
        public int SummaryPrice { get; set; }
        public List<TicketSummaryResponse> Tickets { get; set; } = new List<TicketSummaryResponse>();
    }
}

namespace Acceloka.Models.Responses
{
    public class BookTicketsResponse
    {
        public int PriceSummary { get; set; }
        public List<CategorySummaryResponse> TicketsPerCategories { get; set; } = new List<CategorySummaryResponse>();
    }
}

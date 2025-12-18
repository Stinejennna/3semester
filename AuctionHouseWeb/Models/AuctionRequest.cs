public class AuctionRequest {
    public string Title { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string LotTitle { get; set; }
    public string LotDescription { get; set; }
    public decimal? EstimatedValue { get; set; }
    public decimal? StartPrice { get; set; }
    public List<IFormFile> Images { get; set; }
}
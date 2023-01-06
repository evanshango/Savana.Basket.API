namespace Savana.Basket.API.Dtos;

public class RecentDto {
    public string? HistoryId { get; set; }

    public List<Product> Items { get; set; } = new();
}
namespace Savana.Basket.API.Dtos; 

public class BasketDto {
    public string? BuyerId { get; set; }
    public List<BasketItem> Items { get; set; } = new();
}       
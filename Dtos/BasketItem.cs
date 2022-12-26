namespace Savana.Basket.API.Dtos; 

public class BasketItem {
    public int ProductId { get; set; }
    public string? Name { get; set; }
    public double Price { get; set; }
    public string? ImageUrl { get; set; }
    public string? Brand { get; set; }
    public int Quantity { get; set; }
}
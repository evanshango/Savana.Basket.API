using System.ComponentModel.DataAnnotations;

namespace Savana.Basket.API.Dtos;

public class BasketDto {
    [Required(ErrorMessage = "BuyerId is required")]
    public string? BuyerId { get; set; }

    public double SubTotal { get; set; }

    [Required(ErrorMessage = "Basket items are required")]
    public List<Product> Items { get; set; } = new();
}
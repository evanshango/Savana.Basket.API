using System.ComponentModel.DataAnnotations;

namespace Savana.Basket.API.Dtos;

public class Product {
    [Required(ErrorMessage = "ProductId is required")]
    public string? ProductId { get; set; }

    [Required(ErrorMessage = "Product Name is required")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Product Price is required")]
    public double Price { get; set; }

    [Required(ErrorMessage = "Product Image is required")]
    public string? ImageUrl { get; set; }

    [Required(ErrorMessage = "Product Brand is required")]
    public string? Brand { get; set; }

    [Required(ErrorMessage = "Product Quantity is required")]
    public int Quantity { get; set; }
}
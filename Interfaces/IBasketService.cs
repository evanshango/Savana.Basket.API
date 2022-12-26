using Savana.Basket.API.Dtos;

namespace Savana.Basket.API.Interfaces; 

public interface IBasketService {
    Task<BasketDto?> AddBasket(BasketDto basketDto);
    Task<BasketDto?> GetBasket(string basketId);
    Task<bool> DeleteBasket(BasketDto basket, int productId, int quantity);
}
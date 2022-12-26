using Savana.Basket.API.Dtos;
using Savana.Basket.API.Interfaces;
using Treasures.Common.Interfaces;

namespace Savana.Basket.API.Services;

public class BasketService : IBasketService {
    private readonly ICacheService<BasketDto> _cacheService;
    private readonly int _timeSpan;

    public BasketService(ICacheService<BasketDto> cacheService, IConfiguration config) {
        _cacheService = cacheService;
        _timeSpan = int.Parse(config["Cache:TimeSpan"] ?? "15");
    }

    public async Task<BasketDto?> AddBasket(BasketDto basketDto) {
        return await _cacheService.UpsertItem(id: basketDto.BuyerId!, timeSpan: _timeSpan, entity: basketDto);
    }

    public async Task<BasketDto?> GetBasket(string basketId) => await _cacheService.GetItem(id: basketId);

    public async Task<bool> DeleteBasket(BasketDto basket, int productId, int quantity) {
        var updated = PrepareBasket(basket, productId, quantity);
        if (updated.Items.Count <= 0) {
            return await _cacheService.DeleteItem(id: updated.BuyerId!);
        }

        var update = await AddBasket(updated);
        return update != null;
    }

    private static BasketDto PrepareBasket(BasketDto basket, int productId, int quantity) {
        if (basket.Items.Count <= 0) return basket;
        var item = basket.Items.FirstOrDefault(i => i.ProductId.Equals(productId));
        if (item == null) return basket;
        
        item.Quantity = quantity;
        if (item.Quantity <= 0) basket.Items.Remove(item);
        else basket.Items[basket.Items.IndexOf(item)] = item;

        return basket;
    }
}
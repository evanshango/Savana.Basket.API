using MassTransit;
using Savana.Basket.API.Dtos;
using Treasures.Common.Events;
using Treasures.Common.Interfaces;

namespace Savana.Basket.API.Consumers; 

public class BasketConsumer : IConsumer<BasketEvent> {
    private readonly ICacheService<BasketDto> _cacheService;
    private readonly ILogger<BasketConsumer> _logger;

    public BasketConsumer(ICacheService<BasketDto> cacheService, ILogger<BasketConsumer> logger) {
        _cacheService = cacheService;
        _logger = logger;
    }
    public async Task Consume(ConsumeContext<BasketEvent> context) {
        var message = context.Message;

        var existingBasket = await _cacheService.GetItem(id: message.BuyerId);

        if (existingBasket != null) {
            await _cacheService.DeleteItem(id: message.BuyerId);
            _logger.LogInformation("Basket for buyer with id {BuyerId} deleted successfully", message.BuyerId);
        }
    }
}
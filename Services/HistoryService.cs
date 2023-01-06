using Savana.Basket.API.Dtos;
using Savana.Basket.API.Interfaces;
using Treasures.Common.Interfaces;

namespace Savana.Basket.API.Services;

public class HistoryService : IHistoryService {
    private readonly ICacheService<RecentDto?> _cacheService;
    private readonly int _timeSpan;

    public HistoryService(ICacheService<RecentDto?> cacheService, IConfiguration config) {
        _cacheService = cacheService;
        _timeSpan = int.Parse(config["Cache:TimeSpan"] ?? "15");
    }

    public async Task<RecentDto?> GetRecentItems(string historyId) => await _cacheService.GetItem(id: historyId);

    public async Task<RecentDto?> UpsertHistory(Product product, RecentDto? existing, string historyId) {
        RecentDto? response;
        if (existing != null) {
            var existingProduct = existing.Items.FirstOrDefault(i => i.ProductId!.Equals(product.ProductId));

            if (existingProduct == null) existing.Items.Add(product);
            else existing.Items[existing.Items.IndexOf(existingProduct)] = product;

            response = await _cacheService.UpsertItem(id: historyId, timeSpan: _timeSpan, entity: existing);
        } else {
            var recent = new RecentDto {
                HistoryId = historyId, Items = new List<Product> { product }
            };
            response = await _cacheService.UpsertItem(id: historyId, timeSpan: _timeSpan, entity: recent);
        }

        return response;
    }

    public async Task<bool> DeleteHistory(string historyId) => await _cacheService.DeleteItem(id: historyId);
}
using Savana.Basket.API.Dtos;

namespace Savana.Basket.API.Interfaces;

public interface IHistoryService {
    Task<RecentDto?> GetRecentItems(string historyId);
    Task<RecentDto?> UpsertHistory(Product product, RecentDto? existing, string historyId);
    Task<bool> DeleteHistory(string historyId);
}   
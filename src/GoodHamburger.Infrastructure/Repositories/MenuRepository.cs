namespace GoodHamburger.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Repositories;
using GoodHamburger.Infrastructure.Data;

public class MenuRepository(AppDbContext context, IDistributedCache cache) : IMenuRepository
{
    private readonly AppDbContext _context = context;
    private readonly IDistributedCache _cache = cache;
    private const string ALL_ITEMS_CACHE_KEY = "menu:all";
    private const string SANDWICHES_CACHE_KEY = "menu:sandwiches";
    private const string EXTRAS_CACHE_KEY = "menu:extras";
    private const string ITEM_CACHE_KEY_PREFIX = "menu:item:";
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromHours(24);

    public async Task<List<MenuItem>> GetAllItemsAsync()
    {
        var cachedData = await _cache.GetStringAsync(ALL_ITEMS_CACHE_KEY);

        if (!string.IsNullOrEmpty(cachedData))
        {
            return JsonSerializer.Deserialize<List<MenuItem>>(cachedData) ?? new List<MenuItem>();
        }

        var items = await _context.MenuItems.ToListAsync();

        await CacheMenuItems(ALL_ITEMS_CACHE_KEY, items);

        return items;
    }

    public async Task<List<MenuItem>> GetSandwichesAsync()
    {
        var cachedData = await _cache.GetStringAsync(SANDWICHES_CACHE_KEY);

        if (!string.IsNullOrEmpty(cachedData))
        {
            return JsonSerializer.Deserialize<List<MenuItem>>(cachedData) ?? new List<MenuItem>();
        }

        var sandwiches = await _context.MenuItems
            .Where(m => m.Type == "sandwich")
            .ToListAsync();

        await CacheMenuItems(SANDWICHES_CACHE_KEY, sandwiches);

        return sandwiches;
    }

    public async Task<List<MenuItem>> GetExtrasAsync()
    {
        var cachedData = await _cache.GetStringAsync(EXTRAS_CACHE_KEY);

        if (!string.IsNullOrEmpty(cachedData))
        {
            return JsonSerializer.Deserialize<List<MenuItem>>(cachedData) ?? new List<MenuItem>();
        }

        var extras = await _context.MenuItems
            .Where(m => m.Type == "extra")
            .ToListAsync();

        await CacheMenuItems(EXTRAS_CACHE_KEY, extras);

        return extras;
    }

    public async Task<MenuItem?> GetByIdAsync(int id)
    {
        var cacheKey = $"{ITEM_CACHE_KEY_PREFIX}{id}";
        var cachedData = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedData))
        {
            return JsonSerializer.Deserialize<MenuItem>(cachedData);
        }

        var item = await _context.MenuItems.FindAsync(id);

        if (item is not null)
        {
            await CacheMenuItem(cacheKey, item);
        }

        return item;
    }

    public async Task SeedDataAsync()
    {
        if (!await _context.MenuItems.AnyAsync())
        {
            var items = MenuItem.GetAllInitialItems();
            await _context.MenuItems.AddRangeAsync(items);
            await _context.SaveChangesAsync();
        }
    }

    private async Task CacheMenuItems(string key, List<MenuItem> items)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _cacheExpiration
        };

        var serialized = JsonSerializer.Serialize(items);
        await _cache.SetStringAsync(key, serialized, options);
    }

    private async Task CacheMenuItem(string key, MenuItem item)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _cacheExpiration
        };

        var serialized = JsonSerializer.Serialize(item);
        await _cache.SetStringAsync(key, serialized, options);
    }
}
namespace GoodHamburger.Domain.Repositories;

using GoodHamburger.Domain.Entities;

public interface IMenuRepository
{
    Task<List<MenuItem>> GetAllItemsAsync();
    Task<List<MenuItem>> GetSandwichesAsync();
    Task<List<MenuItem>> GetExtrasAsync();
    Task<MenuItem?> GetByIdAsync(int id);
    Task SeedDataAsync();
}
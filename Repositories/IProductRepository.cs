using WebQuanLySanPhamKhoHang.Models;

namespace WebQuanLySanPhamKhoHang.Repositories
{
    public interface IProductRepository
    {
        Task<IQueryable<Product>> GetAllAsync();
        Task<IEnumerable<Product>> GetAllSPAsync(string category);
        Task<Product> GetByIdAsync(Guid id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<Product>> GetProductsSortedByExpiryAsync();
    }
}

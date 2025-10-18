using WebQuanLySanPhamKhoHang.Models;

namespace WebQuanLySanPhamKhoHang.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();
        IEnumerable<Category> GetAllDanhMucAsync();
        Task<Category> GetByIdAsync(int id);
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(int id);
    }
}

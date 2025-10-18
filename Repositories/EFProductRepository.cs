using WebQuanLySanPhamKhoHang.Data;
using WebQuanLySanPhamKhoHang.Models;
using Microsoft.EntityFrameworkCore;

namespace WebQuanLySanPhamKhoHang.Repositories
{
    public class EFProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public EFProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IQueryable<Product>> GetAllAsync()
        {
            return _context.Products.Include(p => p.Category);
        }

        //public async Task<IEnumerable<Product>> GetAllAsync()
        //{
        //    return await _context.Products.Include(p => p.Category).ToListAsync();
        //}
        public async Task<IEnumerable<Product>> GetAllSPAsync(string category)
        {
            //return await _context.Products.Include(p => p.CategoryId).Where(p => p.Category.Name == category).OrderBy(p => p.Name).ToListAsync();
            return await _context.Products.AsNoTracking().Where(p => p.Category.Name == category).OrderBy(p => p.Name).ToListAsync();

        }
        public async Task<Product> GetByIdAsync(Guid id)
        {
            return await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Product>> GetProductsSortedByExpiryAsync()
        {
            return await _context.Products.OrderBy(p => p.EXP).ToListAsync(); // Sắp xếp theo ngày hết hạn (EXP)
        }

    }
}

using System.ComponentModel.DataAnnotations;

namespace WebQuanLySanPhamKhoHang.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; }
        public int Quantity { get; set; }
        [Range(0.01, 1000000000000.00)]
        public decimal Price { get; set; }
        public DateTime DateIn {  get; set; }
        public DateTime? MFG { get; set; }
        public DateTime? EXP {  get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public string? Status { get; set; } // Thêm trạng thái
    }
}

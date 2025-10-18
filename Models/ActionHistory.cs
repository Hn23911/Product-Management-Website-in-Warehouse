namespace WebQuanLySanPhamKhoHang.Models
{
    public class ActionHistory
    {
        public int Id { get; set; }
        public string ActionType { get; set; }
        public string ProductName { get; set; }
        public Guid ProductId { get; set; }
        public string UserId { get; set; }
        //public string RoleUser {  get; set; }
        public DateTime ActionDate { get; set; }
    }

}

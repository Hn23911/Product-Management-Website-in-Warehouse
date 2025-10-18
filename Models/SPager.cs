namespace WebQuanLySanPhamKhoHang.Views.Shared.Components.SearchBar
{
    public class SPager
    {
        public SPager()
        {
        }
        public string SearchText { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Category { get; set; }
        public string Sort { get; set; }
        public int TotalProducts { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int StartPage { get; set; }
        public int EndPage { get; set; }
        public SPager(int totalProducts, int page, int pageSize)
        {
            int totalPages = (int)Math.Ceiling((decimal)totalProducts / (decimal)pageSize);
            int currentPage = page;
            int startPage = currentPage - 5;
            int endPage = currentPage + 4;
            if (startPage <= 0)
            {
                endPage = endPage - (startPage - 1);
                startPage = 1;
            }
            if (endPage > totalPages)
            {
                endPage = totalPages;
                if (endPage > 10)
                    startPage = endPage - 9;
            }

            TotalProducts = totalProducts;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = totalPages;
            StartPage = startPage;
            EndPage = endPage;
        }

    }

}

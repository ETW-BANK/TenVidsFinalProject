

namespace TenVids.Models.Pagination
{
   public class PaginatedResult<T> where T : class
    {
        public PaginatedResult(IReadOnlyList<T> items, int totalCount, int pageSize, int pageNumber, int totalPages)
        {
            Items = items;
            TotalCount = totalCount;
            PageSize = pageSize;
            PageNumber = pageNumber;
            TotalPages = totalPages;
        }

        public IReadOnlyList<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
    }
    
}

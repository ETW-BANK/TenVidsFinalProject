
namespace TenVids.Models.Pagination
{
 public class BaseParams
    {
        public int PageNumber { get; set; } = 1;
        public int MaxPageSize { get; set; } = 100;

        private int _pageSize ;
        private string _sortBy;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize || value < 0? MaxPageSize:value;
        }
        public string SortBy
        {
            get => _sortBy;
            set => _sortBy = string.IsNullOrEmpty(value)? "" : value.ToLower();
            
        }
    }
}

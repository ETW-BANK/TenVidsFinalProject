
namespace TenVids.Models.Pagination
{
  public class HomeParameters:BaseParams
    {
        private string _searchBy;

        public string SearchBy
        {
            get => _searchBy;   
            set=>_searchBy=string.IsNullOrEmpty(value) ? "" : value.ToLower();

        }
        public int CategoryId { get; set; }
    }
}

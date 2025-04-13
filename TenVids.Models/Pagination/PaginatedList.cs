
namespace TenVids.Models.Pagination
{
   public class PaginatedList<T> : List<T>
    {
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get;  set; }
        public int TotalPages { get; private set; }


        public PaginatedList(IEnumerable<T> items, int totalpages, int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalPages = totalpages;
            AddRange(items);

        }

    //public static async Task< PaginatedList<T>> CreateAsync(IQueryable<T> query,int pageNumber, int pageSize)
    //    {
    //      var totalitemcount= await query.CountAsync();
    //        var totalpages = (int)Math.Ceiling(totalitemcount / (double)pageSize);
    //        if (pageNumber > totalpages && totalpages>0)
    //        {
    //            pageNumber = totalpages;
    //        }

    //        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

    //        return new PaginatedList<T>(items, totalpages, totalitemcount, pageNumber, pageSize);

    //    }
    }
}

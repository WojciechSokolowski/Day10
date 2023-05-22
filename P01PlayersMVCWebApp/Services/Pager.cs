using static System.Runtime.InteropServices.JavaScript.JSType;

namespace P01PlayersMVCWebApp.Services
{
    public class Pager<T>
    {
        public int TotalCount { get; }
        public int PageSize { get; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public int CurrentPage { get; }
        public IEnumerable<T> Data { get; } // Add the Data property of type IEnumerable<T>

        public Pager(int totalCount, int pageSize, int currentPage, IEnumerable<T> data)
        {
            TotalCount = totalCount;
            PageSize = pageSize;
            CurrentPage = currentPage;
            Data = data; // Assign the provided data to the Data property
        }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}

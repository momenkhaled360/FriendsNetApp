using Microsoft.EntityFrameworkCore;

namespace API.Helpers
{
    public class PaginatedResult<T>
    {
        public PaginationMetadata Metadata { get; set; } = default!;
        public List<T> Items { get; set; } = [];
    };

    public class PaginationMetadata
    {
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

    };

    public class PaginationHelper
    {
        public static async Task<PaginatedResult<T>> CreateAsync<T>(IQueryable<T> query, int pageNumber, int pageSize)
        {
            var count = await query.CountAsync();
            var skip = Math.Max(0, (pageNumber - 1) * pageSize);

            var items = await query.Skip(skip).Take(pageSize).ToListAsync();

            return new PaginatedResult<T>
            {
                Metadata = new PaginationMetadata
                {
                    PageNumber = pageNumber,
                    TotalPages = (int)Math.Ceiling(count / (double)pageSize),
                    PageSize = pageSize,
                    TotalCount = count,
                },
                Items = items
            };
        }

    }
}


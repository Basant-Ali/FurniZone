using FurniZone.BLL.ModelVM.Common;

namespace FurniZone.BLL.Helpers
{
    public interface IPaginationHelper
    {
        PagedResponse<T> CreatePagedResponse<T>(List<T> data, int pageNumber, int pageSize, int totalCount);
    }

    public class PaginationHelper : IPaginationHelper
    {
        public PagedResponse<T> CreatePagedResponse<T>(List<T> data, int pageNumber, int pageSize, int totalCount)
        {
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return new PagedResponse<T>
            {
                Data = data,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }
    }
}

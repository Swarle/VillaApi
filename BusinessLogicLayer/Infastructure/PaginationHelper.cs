using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.Dto;
using BusinessLogicLayer.Services.Interfaces;

namespace BusinessLogicLayer.Infastructure
{
    public static class PaginationHelper
    {
        public static PagedDto<T> CreatePagedResponse<T>(List<T> pagedData, PaginationFilter filter, int totalRecords, IUriService uriService, string route)
        {
            var response = new PagedDto<T>
            {
                Data = pagedData,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
            };

            var totalPages = ((double)totalRecords / (double)filter.PageSize);

            int roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));

            response.NextPage =
                (filter.PageNumber >= 1 && filter.PageNumber < roundedTotalPages
                    ? uriService.GetPageUri(new PaginationFilter(filter.PageNumber + 1, filter.PageSize), route)
                    : null)!;

            response.PreviousPage =
                (filter.PageNumber - 1 >= 1 && filter.PageNumber <= roundedTotalPages
                    ? uriService.GetPageUri(new PaginationFilter(filter.PageNumber - 1, filter.PageSize), route)
                    : null)!;

            response.FirstPage = uriService.GetPageUri(new PaginationFilter(1, filter.PageSize), route);
            response.LastPage = uriService.GetPageUri(new PaginationFilter(roundedTotalPages, filter.PageSize), route);
            response.TotalPages = roundedTotalPages;
            response.TotalRecords = totalRecords;

            return response;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agile.Common.Data;
using Agile.Common.Exceptions;

namespace Agile.UI.Mvc
{
    public class Pagination<T>
    {
        public const int DefaultPageSize = 10;
        public Pagination(IEnumerable<T> list, int totalCount, int? skip, int? take)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }
            if (totalCount < 0)
            {
                throw new ArgumentException("totalCount");
            }

            List = list;
            TotalItemCount = totalCount;
            PageSize = take ?? DefaultPageSize;
            if (PageSize <= 0)
            {
                PageSize = DefaultPageSize;
            }
            Skip = skip ?? 0;
            PageCount = (TotalItemCount + PageSize - 1) / PageSize;
            CurrentPage = Skip / PageSize + 1;
            if (CurrentPage > 1)
            {
                HasPreviousPage = true;
            }
            if (CurrentPage < PageCount)
            {
                HasNextPage = true;
            }
        }

        public Pagination(QueryResult<T> result, BaseQuery query) :
            this(result.List, result.Count, query.Skip, query.Take)
        {
            Query = query;
        }
        public object Query { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public IEnumerable<T> List { get; }
        public int Count => List.Count();
        public int PageCount { get; }
        public int TotalItemCount { get; }
        public int CurrentPage { get; }
        public int PageSize { get; }
        public int Skip { get; }
        public bool HasPreviousPage { get; private set; }
        public bool HasNextPage { get; private set; }
    }
}

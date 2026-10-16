using System.Linq.Expressions;
using Framwork.PagedList;
using Mapster;
using Microsoft.EntityFrameworkCore;



namespace Infrastructure.Extensions;

public static class PagedListExtensions
{
    public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate)
        => condition ? query.Where(predicate) : query;
    public static async Task<PagedList<TDest>> ToPagedList<TDest>
        (this IQueryable<TDest> source, int? page = 1, int? pageSize = 50, CancellationToken ct = default)
    {
        page ??= 1;
        pageSize ??= 50;

        int count = await source.CountAsync(ct);

        var paginationMetadata = new PagedListInfo
        {
            PageNumber = page.Value,
            TotalCount = count,
            PageSize = pageSize
        };

        int totalPages = (int)Math.Ceiling(count / (double)paginationMetadata!.PageSize);

        paginationMetadata.TotalPages = totalPages;

        var data = await source
            .Skip((page.Value - 1) * paginationMetadata.PageSize.Value)
            .Take(paginationMetadata.PageSize.Value).ToListAsync(ct);

        return new PagedList<TDest>
        {
            List = data,
            Pagination = paginationMetadata
        };
    }


    public static async Task<PagedList<TDest>> ToPagedListAsync<TSource, TDest>
    (this IQueryable<TSource> source, Expression<Func<TSource, TDest>> selector,
        int? pageNumber = 1, int? pageSize = 50, CancellationToken ct = default)
    {
        pageNumber ??= 1;
        pageSize ??= 50;

        var count = await source.CountAsync(ct);

        var paginationMetadata = new PagedListInfo
        {
            PageNumber = pageNumber.Value,
            TotalCount = count,
            PageSize = pageSize
        };

        var totalPages = (int)Math.Ceiling(count / (double)paginationMetadata!.PageSize);

        paginationMetadata.TotalPages = totalPages;

        var data = await source
            .Select(selector)
            .Skip((pageNumber.Value - 1) * paginationMetadata.PageSize.Value)
            .Take(paginationMetadata.PageSize.Value).ToListAsync(ct);

        return new PagedList<TDest>
        {
            List = data.Adapt<List<TDest>>(),
            Pagination = paginationMetadata
        };
    }


    public static async Task<PagedList<TDest>> ToPagedList<TSource, TDest>
    (this IQueryable<TSource> source,
        int? page = 1, int? pageSize = 50, CancellationToken ct = default)
    {
        page ??= 1;
        pageSize ??= 50;

        var count = await source.CountAsync();

        var paginationMetadata = new PagedListInfo
        {
            PageNumber = page.Value,
            TotalCount = count,
            PageSize = pageSize
        };

        var totalPages = (int)Math.Ceiling(count / (double)paginationMetadata!.PageSize);

        paginationMetadata.TotalPages = totalPages;

        var data = await source
            .Skip((page.Value - 1) * paginationMetadata.PageSize.Value)
            .Take(paginationMetadata.PageSize.Value).ToListAsync(ct);

        return new PagedList<TDest>
        {
            List = data.Adapt<List<TDest>>(),
            Pagination = paginationMetadata
        };
    }
    public static async Task<PagedList<TDest, TExtra>> ToPagedList<TSource, TDest, TExtra>
(
    this IQueryable<TSource> source,
    int? page = 1,
    int? pageSize = 50,
    CancellationToken ct = default
)
    {

        page ??= 1;
        pageSize ??= 50;

        var count = await source.CountAsync(ct);

        var paginationMetadata = new PagedListInfo
        {
            PageNumber = page.Value,
            TotalCount = count,
            PageSize = pageSize
        };

        paginationMetadata.TotalPages =
            (int)Math.Ceiling(count / (double)pageSize.Value);

        var data = await source
            .Skip((page.Value - 1) * pageSize.Value)
            .Take(pageSize.Value)
            .ToListAsync(ct);

        return new PagedList<TDest, TExtra>
        {
            List = data.Adapt<List<TDest>>(),
            Pagination = paginationMetadata
        };
    }


}
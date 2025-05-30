﻿namespace CleanArchitecture.Application.Common.Models;

public class PagedResult<T>
{
    public List<T> Items { get; set; }
    public int TotalItems { get; set; }
    public int Page { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    public int PageSize { get; set; }

    public PagedResult(List<T> items, int totalItems, int page, int pageSize)
    {
        Items = items;
        TotalItems = totalItems;
        Page = page;
        PageSize = pageSize;
    }
}
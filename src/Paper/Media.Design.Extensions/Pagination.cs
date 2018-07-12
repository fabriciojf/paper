using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Toolset;

namespace Paper.Media.Design.Extensions
{
  public class Pagination
  {
    private int rows = 50;

    private int? _offset;
    private int? _page;

    public Pagination()
    {
    }

    public int Limit
    {
      get => rows;
      set
      {
        rows = (value > 0) ? value : 50;
        IsPageSizeSet = false;
      }
    }

    public int PageSize
    {
      get => rows;
      set
      {
        rows = (value > 0) ? value : 50;
        IsPageSizeSet = true;
      }
    }

    public int Offset
    {
      get
      {
        if (_offset != null)
        {
          return _offset.Value;
        }
        if (_page > 1)
        {
          return (_page.Value - 1) * rows;
        }
        return 0;
      }
      set
      {
        if (value > 0)
        {
          _offset = value;
          _page = null;
        }
        else
        {
          _offset = null;
        }
      }
    }

    public int Page
    {
      get
      {
        if (_page != null)
        {
          return _page.Value;
        }
        else if (_offset > 0)
        {
          return (_offset.Value /  rows) + 1;
        }
        else
        {
          return 1;
        }
      }
      set
      {
        if (value > 1)
        {
          _page = value;
          _offset = null;
        }
        else
        {
          _page = null;
        }
      }
    }

    public bool IsLimitSet => !IsPageSizeSet;

    public bool IsPageSizeSet { get; private set; }

    public bool IsOffsetSet => _page == null;

    public bool IsPageSet => _page != null;

    public IEnumerable<KeyValuePair<string, int>> GetValues()
    {
      if (IsLimitSet)
        yield return KeyValuePair.Create("limit", Limit);
      else
        yield return KeyValuePair.Create("pageSize", PageSize);

      if (IsOffsetSet)
        yield return KeyValuePair.Create("offset", Offset);
      else
        yield return KeyValuePair.Create("page", Page);
    }

    public override string ToString()
    {
      return ToUriComponent();
    }

    public string ToUriComponent()
    {
      var rowsName = IsPageSizeSet ? "pageSize" : "limit";
      return IsPageSet
        ? $"page={Page}&{rowsName}={Limit}"
        : $"offset={Offset}&{rowsName}={Limit}";
    }

    public static Pagination CreateOffset(int? limit = 50, int? offset = 0)
    {
      return new Pagination { Limit = limit.Value, Offset = offset.Value };
    }

    public static Pagination CreatePage(int? pageSize = 50, int? page = 1)
    {
      return new Pagination { PageSize = pageSize.Value, Page = page.Value };
    }

    public static Pagination CreateFromUri(string uri)
    {
      var pagination = new Pagination();

      var queryString = uri.Split('?').Skip(1).FirstOrDefault();
      if (queryString == null)
      {
        if (uri.Contains("="))
        {
          queryString = uri;
        }
      }

      if (queryString != null)
      {
        var args =
          from token in queryString.Split('&')
          let parts = token.Split('=')
          let name = parts.First().ToLower()
          let value = parts.Skip(1).LastOrDefault()
          where value != null
          orderby name descending
          select new { name, value };

        foreach (var arg in args)
        {
          switch (arg.name)
          {
            case "page":
              {
                int number = 0;
                if (int.TryParse(arg.value, out number))
                {
                  pagination.Page = number;
                }
                break;
              }
            case "offset":
              {
                int number = 0;
                if (int.TryParse(arg.value, out number))
                {
                  pagination.Offset = number;
                }
                break;
              }
            case "pagesize":
              {
                int number = 0;
                if (int.TryParse(arg.value, out number))
                {
                  pagination.PageSize = number;
                }
                break;
              }
            case "limit":
              {
                int number = 0;
                if (int.TryParse(arg.value, out number))
                {
                  pagination.Limit = number;
                }
                break;
              }
          }
        }
      }

      return pagination;
    }
  }
}
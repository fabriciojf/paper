using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Toolset;

namespace Paper.Media.Design.Extensions
{
  public class Pagination : IEnumerable<KeyValuePair<string, int>>
  {
    private int count = 50;
    private int index = 0;

    private bool _isOffsetSet;
    private bool _isLimitSet;

    public Pagination()
    {
    }

    public int Limit
    {
      get => count;
      set
      {
        SetLimitOrPageSize(value);
        IsLimitSet = true;
      }
    }

    public int PageSize
    {
      get => count;
      set
      {
        SetLimitOrPageSize(value);
        IsPageSizeSet = true;
      }
    }

    public int Offset
    {
      get => IsOffsetSet ? index : (index - 1) * count;
      set
      {
        index = (value > 0) ? value : 0;
        IsOffsetSet = true;
      }
    }

    public int Page
    {
      get => IsPageSet ? index : (index / count) + 1;
      set
      {
        index = (value > 1) ? value : 1;
        IsPageSet = true;
      }
    }

    public bool IsLimitSet
    {
      get => _isLimitSet;
      set => _isLimitSet = value;
    }

    public bool IsPageSizeSet
    {
      get => !_isLimitSet;
      set => _isLimitSet = !value;
    }

    public bool IsOffsetSet
    {
      get => _isOffsetSet;
      set => _isOffsetSet = value;
    }

    public bool IsPageSet
    {
      get => !_isOffsetSet;
      set => _isOffsetSet = !value;
    }

    public void SetLimitOrPageSize(int value)
    {
      count = (value > 0) ? value : 50;
    }

    public override string ToString()
    {
      var rowsName = IsPageSizeSet ? "pageSize" : "limit";
      return IsPageSet
        ? $"page={Page}&{rowsName}={Limit}"
        : $"offset={Offset}&{rowsName}={Limit}";
    }

    public void CopyFromUri(string uri)
    {
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
                  Page = number;
                }
                break;
              }
            case "offset":
              {
                int number = 0;
                if (int.TryParse(arg.value, out number))
                {
                  Offset = number;
                }
                break;
              }
            case "pagesize":
              {
                int number = 0;
                if (int.TryParse(arg.value, out number))
                {
                  PageSize = number;
                }
                break;
              }
            case "limit":
              {
                int number = 0;
                if (int.TryParse(arg.value, out number))
                {
                  Limit = number;
                }
                break;
              }
          }
        }
      }
    }

    public string CopyToUri(string uri)
    {
      return CopyToUri(new Route(uri));
    }

    public Route CopyToUri(Route uri)
    {
      var args = EnumerateArgs();
      uri = uri
        .UnsetArgs("pageSize", "limit", "page", "offset")
        .SetArg(args);
      return uri;
    }

    public IEnumerator<KeyValuePair<string, int>> GetEnumerator()
    {
      return EnumerateArgPairs().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return EnumerateArgPairs().GetEnumerator();
    }

    private IEnumerable<KeyValuePair<string, int>> EnumerateArgPairs()
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

    private IEnumerable<object> EnumerateArgs()
    {
      if (IsPageSet)
      {
        yield return "page";
        yield return Page;
      }
      else
      {
        yield return "offset";
        yield return Offset;
      }

      if (IsPageSizeSet)
      {
        yield return "pageSize";
        yield return PageSize;
      }
      else
      {
        yield return "limit";
        yield return Limit;
      }
    }

    public Pagination Clone()
    {
      return new Pagination
      {
        count = this.count,
        index = this.index,
        _isOffsetSet = this._isOffsetSet,
        _isLimitSet = this._isLimitSet
      };
    }

    public Pagination FirstPage()
    {
      var clone = this.Clone();
      if (clone.IsPageSet)
      {
        clone.Page = 1;
      }
      else
      {
        clone.Offset = 0;
      }
      return clone;
    }

    public Pagination NextPage()
    {
      var clone = this.Clone();
      if (clone.IsPageSet)
      {
        clone.Page++;
      }
      else
      {
        clone.Offset += clone.PageSize;
      }
      return clone;
    }

    public Pagination PreviousPage()
    {
      var clone = this.Clone();
      if (clone.IsPageSet)
      {
        if (clone.Page <= 1)
          return null;

        clone.Page--;
      }
      else
      {
        if (clone.Offset <= 0)
          return null;

        clone.Offset -= clone.PageSize;
        if (clone.Offset < 0)
          clone.Offset = 0;
      }
      return clone;
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
      pagination.CopyFromUri(uri);
      return pagination;
    }
  }
}
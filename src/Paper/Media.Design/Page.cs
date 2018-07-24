using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Toolset;

namespace Paper.Media.Design
{
  public class Page
  {
    private int count;
    private int index;

    private bool _isOffsetSet;
    private bool _isLimitSet;

    public Page()
    {
      this.Size = 50;
      this.Number = 1;
    }

    public int Limit
    {
      get => count;
      set
      {
        SetLimitOrSize(value);
        IsLimitSet = true;
      }
    }

    public int Size
    {
      get => count;
      set
      {
        SetLimitOrSize(value);
        isSizeSet = true;
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

    public int Number
    {
      get => IsNumberSet ? index : (index / count) + 1;
      set
      {
        index = (value > 1) ? value : 1;
        IsNumberSet = true;
      }
    }

    public bool IsLimitSet
    {
      get => _isLimitSet;
      set => _isLimitSet = value;
    }

    public bool isSizeSet
    {
      get => !_isLimitSet;
      set => _isLimitSet = !value;
    }

    public bool IsOffsetSet
    {
      get => _isOffsetSet;
      set => _isOffsetSet = value;
    }

    public bool IsNumberSet
    {
      get => !_isOffsetSet;
      set => _isOffsetSet = !value;
    }

    public void SetLimitOrSize(int value)
    {
      count = (value > 0) ? value : 50;
    }

    public override string ToString()
    {
      var rowsName = isSizeSet ? "pageSize" : "limit";
      return IsNumberSet
        ? $"page={Number}&{rowsName}={Limit}"
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
                  Number = number;
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
                  Size = number;
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
      var route = new Route(uri);
      var args = EnumerateArgs();
      route = route
        .UnsetArgs("pageSize", "limit", "page", "offset")
        .SetArg(args);
      return route;
    }

    private IEnumerable<KeyValuePair<string, int>> EnumerateArgPairs()
    {
      if (IsLimitSet)
        yield return KeyValuePair.Create("limit", Limit);
      else
        yield return KeyValuePair.Create("pageSize", Size);

      if (IsOffsetSet)
        yield return KeyValuePair.Create("offset", Offset);
      else
        yield return KeyValuePair.Create("page", Number);
    }

    private IEnumerable<object> EnumerateArgs()
    {
      if (IsNumberSet)
      {
        yield return "page";
        yield return Number;
      }
      else
      {
        yield return "offset";
        yield return Offset;
      }

      if (isSizeSet)
      {
        yield return "pageSize";
        yield return Size;
      }
      else
      {
        yield return "limit";
        yield return Limit;
      }
    }

    public Page Clone()
    {
      return new Page
      {
        count = this.count,
        index = this.index,
        _isOffsetSet = this._isOffsetSet,
        _isLimitSet = this._isLimitSet
      };
    }

    public Page FirstPage()
    {
      var clone = this.Clone();
      if (clone.IsNumberSet)
      {
        clone.Number = 1;
      }
      else
      {
        clone.Offset = 0;
      }
      return clone;
    }

    public Page NextPage()
    {
      var clone = this.Clone();
      if (clone.IsNumberSet)
      {
        clone.Number++;
      }
      else
      {
        clone.Offset += clone.Size;
      }
      return clone;
    }

    public Page PreviousPage()
    {
      var clone = this.Clone();
      if (clone.IsNumberSet)
      {
        if (clone.Number <= 1)
          return null;

        clone.Number--;
      }
      else
      {
        if (clone.Offset <= 0)
          return null;

        clone.Offset -= clone.Size;
        if (clone.Offset < 0)
          clone.Offset = 0;
      }
      return clone;
    }

    public static Page CreateOffset(int? limit = 50, int? offset = 0)
    {
      return new Page { Limit = limit.Value, Offset = offset.Value };
    }

    public static Page CreatePage(int? pageSize = 50, int? page = 1)
    {
      return new Page { Size = pageSize.Value, Number = page.Value };
    }
  }
}
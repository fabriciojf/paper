using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolset;
using Toolset.Collections;

namespace Paper.Media.Design
{
  public static class PaginationExtensions
  {
    public static Pagination AddLimit(this Pagination page, int value)
    {
      page.Limit = value;
      return page;
    }

    public static Pagination AddOffset(this Pagination page, int value)
    {
      page.Offset = value;
      return page;
    }

    public static Pagination AddPage(this Pagination page, string specification)
    {
      int limit = -1;
      int offset = -1;
      int number = -1;

      var items =
        from token in specification.Split('&')
        let parts = token.Split('=')
        let name = parts.First().Trim()
        let value = parts.Skip(1).LastOrDefault()?.Trim()
        select new { name, value };

      foreach (var item in items)
      {
        if (item.name.EqualsIgnoreCase("offset"))
        {
          offset = int.Parse(item.value);
        }
        else if (item.name.EqualsIgnoreCase("limit"))
        {
          limit = int.Parse(item.value);
        }
        else if (item.name.EqualsIgnoreCase("page"))
        {
          number = int.Parse(item.value);
        }
        else
        {
          throw new Exception(
            "Parâmetro de paginação não reconhecido: " + item.name + "."
            + " Era esperado um texto na forma \"offset=20&limit=10\" ou \"page=2&limit=10\"");
        }
      }

      if (limit > -1)
        page.Limit = limit;

      if (number == 0)
        number = 1;
      if (number > -1)
        offset = (number - 1) * page.Limit;

      if (offset > -1)
        page.Offset = offset;

      return page;
    }

    public static Pagination AddPage(this Pagination page, int offset, int limit)
    {
      page.Offset = offset;
      page.Limit = limit;
      return page;
    }

    public static Pagination AddPageNumber(this Pagination page, int pageNumber, int pageSize)
    {
      page.Offset = pageSize * (pageNumber - 1);
      page.Limit = pageSize;
      return page;
    }
  }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Paper.Media.Design.Extensions;

namespace Paper.Media.Papers
{
  public interface IPaperRows<TRow> : IPaper
  {
    Pagination RowPagination { get; }

    Sort RowSort { get; }

    IFilter RowFilter { get; }

    string GetTitle();

    IEnumerable<TRow> GetRows();

    IEnumerable<HeaderInfo> GetRowHeaders(IEnumerable<TRow> rows);

    IEnumerable<ILink> GetRowLinks(TRow row);
  }
}
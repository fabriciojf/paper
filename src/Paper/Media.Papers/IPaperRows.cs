using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Paper.Media.Design.Extensions;

namespace Paper.Media.Papers
{
  public interface IPaperRows : IPaper
  {
    Pagination RowPagination { get; }

    Sort RowSort { get; }

    IFilter RowFilter { get; }

    string GetTitle();

    DataTable GetRows();

    IEnumerable<HeaderInfo> GetRowHeaders(DataTable rows);

    IEnumerable<ILink> GetRowLinks(DataRow row);
  }
}
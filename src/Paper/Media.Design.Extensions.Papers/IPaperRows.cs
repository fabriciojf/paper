using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Paper.Media.Design.Extensions;

namespace Media.Design.Extensions.Papers
{
  public interface IPaperRows : IPaper
  {
    Page RowsPage { get; }

    Sort RowsSort { get; }

    IFilter RowFilter { get; }

    string GetTitle();

    DataTable GetRows();

    IEnumerable<HeaderInfo> GetRowHeaders(DataTable rows);

    IEnumerable<ILink> GetRowLinks(DataRow row);
  }
}
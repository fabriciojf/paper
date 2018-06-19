using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Paper.Media.Design.Queries
{
  public interface IDataQuery : IQuery
  {
    string GetTitle();

    Links GetLinks();

    Cols GetDataHeaders();

    DataTable GetData();

    Links GetDataLinks(DataRow data);
  }
}
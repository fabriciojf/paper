using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Paper.Media.Design.Queries
{
  public interface IHasQueryMetadata<TProperties>
  {
    NameCollection GetClass();

    NameCollection GetRels();

    string GetTitle();

    Links GetLinks();

    TProperties GetProperties();
  }
}

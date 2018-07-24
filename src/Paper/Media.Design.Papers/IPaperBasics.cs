using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Design.Papers
{
  public interface IPaperBasics : IPaper
  {
    string GetTitle();

    NameCollection GetClass();

    NameCollection GetRel();

    object GetProperties();

    IEnumerable<ILink> GetLinks();
  }
}
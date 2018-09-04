using System.Collections.Generic;
using System.Data;

namespace Paper.Media.Design.Papers
{
  public interface IPaperCards : IPaper
  {
    string GetTitle();

    DataTable GetCards();
    
    IEnumerable<ILink> GetCardLinks(DataRow card);
  }
}
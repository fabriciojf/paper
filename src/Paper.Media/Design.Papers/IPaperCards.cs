using System.Collections.Generic;
using System.Data;

namespace Paper.Media.Design.Papers
{
  public interface IPaperCards : IPaper
  {
    string GetTitle();

    DataTable GetCards();

    IEnumerable<HeaderInfo> GetCardHeaders(DataRow card);

    IEnumerable<ILink> GetCardLinks(DataRow card);
  }
}
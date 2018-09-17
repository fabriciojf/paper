using System.Collections.Generic;
using System.Data;

namespace Paper.Media.Design.Papers
{
  public interface IPaperCards<T> : IPaper
  {
    string GetTitle();

    IEnumerable<T> GetCards();

    IEnumerable<HeaderInfo> GetCardHeaders(T card);

    IEnumerable<ILink> GetCardLinks(T card);
  }
}
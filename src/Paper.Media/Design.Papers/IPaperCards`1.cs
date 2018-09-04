using System.Collections.Generic;

namespace Paper.Media.Design.Papers
{
  public interface IPaperCards<TCard> : IPaper
  {
    string GetTitle();

    IEnumerable<TCard> GetCards();
    
    IEnumerable<ILink> GetCardLinks(TCard card);
  }
}
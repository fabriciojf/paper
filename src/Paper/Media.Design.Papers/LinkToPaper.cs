using System;
using System.Collections.Generic;
using System.Text;
using Paper.Media.Design;
using Paper.Media.Design.Papers.Rendering;

namespace Paper.Media.Design.Papers
{
  /// <summary>
  /// Fábrica para criação de link para outro Paper.
  /// </summary>
  /// <typeparam name="T">O tipo do Paper.</typeparam>
  /// <seealso cref="Media.Design.Extensions.Papers.ILinkFactory{Media.Design.Extensions.Papers.LinkToPaper{T}.Context}" />
  public class LinkToPaper<T> : ILink
    where T : IPaperBasics
  {
    public Link RenderLink(PaperContext ctx)
    {
      return null;
    }
  }
}
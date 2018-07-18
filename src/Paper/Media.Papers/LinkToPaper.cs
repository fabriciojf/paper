using System;
using System.Collections.Generic;
using System.Text;
using Paper.Media.Design.Extensions;
using Paper.Media.Papers.Rendering;

namespace Paper.Media.Papers
{
  /// <summary>
  /// Fábrica para criação de link para outro Paper.
  /// </summary>
  /// <typeparam name="T">O tipo do Paper.</typeparam>
  /// <seealso cref="Paper.Media.Papers.ILinkFactory{Paper.Media.Papers.LinkToPaper{T}.Context}" />
  public class LinkToPaper<T> : ILink
    where T : IPaperInfo
  {
    public Link RenderLink(PaperContext ctx)
    {
      return null;
    }
  }
}
using System;
using System.Collections.Generic;
using System.Text;
using Paper.Media;
using Media.Design.Extensions.Papers.Rendering;

namespace Media.Design.Extensions.Papers
{
  /// <summary>
  /// Cria o link da própria entidade.
  /// </summary>
  /// <seealso cref="Paper.Media.Link" />
  /// <seealso cref="Media.Design.Extensions.Papers.ILink" />
  public class LinkSelf : Link, ILink
  {
    public LinkSelf(string href)
    {
      this.Href = href;
      this.Rel = RelNames.Self;
    }

    public Link RenderLink(PaperContext ctx)
    {
      return this;
    }
  }
}
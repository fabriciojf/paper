using System;
using System.Collections.Generic;
using System.Text;
using Paper.Media;
using Paper.Media.Rendering;

namespace Paper.Media.Design.Papers
{
  /// <summary>
  /// Cria o link da própria entidade.
  /// </summary>
  /// <seealso cref="Paper.Media.Link" />
  /// <seealso cref="Media.Design.Extensions.Papers.ILink" />
  public class LinkSelf : Link, ILink
  {
    public LinkSelf()
    {
      this.Rel = RelNames.Self;
    }

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
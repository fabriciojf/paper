﻿using System;
using System.Collections.Generic;
using System.Text;
using Paper.Media;
using Paper.Media.Papers.Rendering;

namespace Paper.Media.Papers
{
  /// <summary>
  /// Cria um link para uma URI.
  /// </summary>
  /// <seealso cref="Paper.Media.Link" />
  /// <seealso cref="Paper.Media.Papers.ILink" />
  public class LinkTo : Link, ILink
  {
    public LinkTo()
    {
    }

    public LinkTo(string href)
    {
      this.Href = href;
      this.Rel = RelNames.Link;
    }

    public LinkTo(string href, string title)
    {
      this.Href = href;
      this.Title = title;
      this.Rel = RelNames.Link;
    }

    public LinkTo(string href, string title, string rel)
    {
      this.Href = href;
      this.Title = title;
      this.Rel = rel;
    }

    public Link RenderLink(PaperContext ctx)
    {
      return this;
    }
  }
}
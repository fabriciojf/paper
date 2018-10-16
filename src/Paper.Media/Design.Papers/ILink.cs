using System;
using System.Collections.Generic;
using System.Text;
using Paper.Media;
using Paper.Media.Rendering_Obsolete;
using Paper.Media.Routing;

namespace Paper.Media.Design.Papers
{
  /// <summary>
  /// Interface para links vinculados a instâncias de IPaper.
  /// </summary>
  public interface ILink
  {
    [Obsolete("Será removido em breve.")]
    Link RenderLink(PaperContext ctx);

    Link RenderLink(IContext context);
  }
}
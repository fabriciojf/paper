using System;
using System.Collections.Generic;
using System.Text;
using Paper.Media;
using Paper.Media.Rendering;

namespace Paper.Media.Design.Papers
{
  /// <summary>
  /// Interface para links vinculados a instâncias de IPaper.
  /// </summary>
  public interface ILink
  {
    Link RenderLink(PaperContext ctx);
  }
}
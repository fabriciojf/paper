using System;
using System.Collections.Generic;
using System.Text;
using Paper.Media;
using Paper.Media.Papers.Rendering;

namespace Paper.Media.Papers
{
  /// <summary>
  /// Interface para links vinculados a instâncias de IPaper.
  /// </summary>
  public interface ILink
  {
    Link RenderLink(PaperContext ctx);
  }
}
using System;
using System.Collections.Generic;
using System.Text;
using Paper.Media;
using Media.Design.Extensions.Papers.Rendering;

namespace Media.Design.Extensions.Papers
{
  /// <summary>
  /// Interface para links vinculados a instâncias de IPaper.
  /// </summary>
  public interface ILink
  {
    Link RenderLink(PaperContext ctx);
  }
}
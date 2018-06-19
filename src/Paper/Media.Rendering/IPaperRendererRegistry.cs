using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Paper.Media;
using Toolset;

namespace Paper.Media.Rendering
{
  public interface IPaperRendererRegistry : IEnumerable<PaperRendererInfo>
  {
    void Add(PaperRendererInfo renderer);

    PaperRendererInfo FindPaperRenderer(string path);

    IEnumerable<PaperRendererInfo> FindPaperRenderers(Type paperType);
  }
}

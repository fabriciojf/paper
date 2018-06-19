using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Paper.Media;
using Toolset;

namespace Paper.Media.Rendering
{
  public interface IPaperRenderer
  {
    Type PaperType { get; set; }

    string PathTemplate { get; set; }

    Ret<Entity> RenderEntity(HttpContext httpContext, string path);
  }
}

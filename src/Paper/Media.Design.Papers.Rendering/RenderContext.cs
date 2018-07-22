using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Paper.Media.Rendering.Queries
{
  class RenderContext
  {
    public IServiceProvider Injector { get; set; }

    public IPaperRendererRegistry PaperRendererRegistry { get; set; }

    public HttpContext HttpContext { get; set; }

    public string PathTemplate { get; set; }

    public Entity Entity { get; set; }

    public object Query { get; set; }

    public object Data { get; set; }

    public object Rows { get; set; }

    public IDictionary<string, object> PathArgs { get; set; }

    public IDictionary<string, object> QueryArgs { get; set; }
  }
}

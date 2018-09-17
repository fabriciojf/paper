using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Routing
{
  public interface IRenderer
  {
    Entity RenderEntity(IPaper paper, IArgs args, IContext context, IObjectFactory factory);
  }
}
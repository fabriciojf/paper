﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Media.Design.Extensions.Papers.Rendering
{
  internal class PaperInfoRenderer
  {
    private readonly IServiceProvider serviceProvider;

    public PaperInfoRenderer(IServiceProvider serviceProvider)
    {
      this.serviceProvider = serviceProvider;
    }

    public void RenderEntity(IPaperInfo paper, Entity entity)
    {
    }
  }
}

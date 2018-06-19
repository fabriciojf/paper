using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolset;
using Toolset.Collections;

namespace Paper.Media.Rendering.Entities
{
  public class ExposeEntityAttribute : ExposePaperAttribute
  {
    public ExposeEntityAttribute(string path, params string[] alternatePaths)
      : base(EntityRenderer.ContractName, path.AsSingle().Concat(alternatePaths))
    {
    }
  }
}

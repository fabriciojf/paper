using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolset;

namespace Paper.Media.Rendering
{
  public abstract class ExposePaperAttribute : ExposeAttribute
  {
    public ExposePaperAttribute(string contractName, IEnumerable<string> paths)
      : base(contractName)
    {
      this.PathTemplates = paths.ToArray();
    }

    public string[] PathTemplates { get; }
  }
}

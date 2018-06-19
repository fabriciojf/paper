using System;
using System.Collections.Generic;
using System.Text;
using Toolset;

namespace Paper.Media.Rendering
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
  public class PaperRendererAttribute : ExposeAttribute
  {
    public PaperRendererAttribute(string paperContractName)
    {
      this.PaperContractName = paperContractName;
    }

    public string PaperContractName { get; }
  }
}
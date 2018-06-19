using System;
using System.Collections.Generic;
using System.Text;
using Toolset;
using Toolset.Collections;

namespace Paper.Media.Rendering
{
  public class PaperRendererInfo
  {
    private string _pathTemplate;

    public Type PaperRendererType { get; set; }

    public Type PaperType { get; set; }

    public string PathTemplate
    {
      get => _pathTemplate;
      set => _pathTemplate = string.Join("/", value.Split('/').NonEmpty());
    }
  }
}
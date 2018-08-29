using System;
using System.Collections.Generic;
using System.Text;
using Paper.Media.Design;

namespace Paper.Media.Routing
{
  public class PaperInfo
  {
    public Type Type { get; }
    public string Path { get; }

    public PaperInfo(Type type, string path)
    {
      this.Type = type;
      this.Path = path;
    }

    public static PaperInfo CreatePaperInfo<T>()
    {
      return CreatePaperInfo(typeof(T));
    }

    public static PaperInfo CreatePaperInfo(Type paperType)
    {
      var path = PaperAttribute.Extract(paperType).UriTemplate;
      var info = new PaperInfo(paperType, path);
      return info;
    }

  }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Service
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
  }
}
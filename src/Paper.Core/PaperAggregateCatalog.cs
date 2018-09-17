using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Paper.Core.Utilities;
using Paper.Media;
using Paper.Media.Design;
using Paper.Media.Rendering;
using Paper.Media.Utilities;
using Toolset;
using Toolset.Collections;

namespace Paper.Core
{
  public class PaperAggregateCatalog : IPaperCatalog
  {
    private readonly PathIndex<PaperSpec> pathIndex = new PathIndex<PaperSpec>();
    private readonly Map<Type, PaperSpec> typeIndex = new Map<Type, PaperSpec>();

    public PaperAggregateCatalog()
    {
    }

    public PaperAggregateCatalog(IEnumerable<Type> paperTypes)
    {
      paperTypes?.Distinct().ForEach(Add);
    }

    public string[] Paths => pathIndex.Paths;

    public void AddRange(IEnumerable<Type> paperTypes)
    {
      paperTypes?.Distinct().ForEach(Add);
    }

    public void Add(Type paperType)
    {
      try
      {
        var info = PaperSpec.GetSpec(paperType);
        typeIndex.Add(info.Type, info);
        pathIndex.Add(info.Route, info);
      }
      catch (Exception ex)
      {
        ex.Trace($"[PAPER]FAULT: {paperType.FullName}: {string.Join(",", ex.GetCauseMessages())}");
      }
    }

    public void AddExposedTypes()
    {
      var knownTypes =
        ExposedTypes
          .GetTypes<Entity>()
          .Concat(ExposedTypes.GetTypes<IPaper>());
      knownTypes.ForEach(Add);
    }

    public PaperSpec FindPaper<T>()
    {
      var info = typeIndex[typeof(T)];
      return info;
    }

    public PaperSpec FindPaper(Type paperType)
    {
      var info = typeIndex[paperType];
      return info;
    }

    public PaperSpec FindPaper(string path)
    {
      var info = pathIndex.FindExact(path);
      return info;
    }

    public void PrintInfo()
    {
      foreach (var path in Paths.OrderBy(x => x))
      {
        var info = FindPaper(path);
        Debug.WriteLine($"[PAPER]MAPPED {path} -> {info.Type.FullName}");
        Console.WriteLine($"[PAPER]MAPPED {path} -> {info.Type.FullName}");
      }
    }

    public IEnumerator<PaperSpec> GetEnumerator()
    {
      return typeIndex.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return typeIndex.Values.GetEnumerator();
    }
  }
}

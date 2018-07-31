using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Paper.Media.Design.Papers;
using Toolset;
using Toolset.Collections;

namespace Paper.Media.Service
{
  public class PaperRegistry : IPaperRegistry
  {
    private readonly PathIndex<PaperInfo> pathIndex = new PathIndex<PaperInfo>();
    private readonly Map<Type, PaperInfo> typeIndex = new Map<Type, PaperInfo>();

    public PaperRegistry()
    {
    }

    public PaperRegistry(IEnumerable<Type> paperTypes)
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
        var info = CreatePaperInfo(paperType);
        typeIndex.Add(info.Type, info);
        pathIndex.Add(info.Path, info);
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

    public PaperInfo FindPaper<T>()
    {
      var info = typeIndex[typeof(T)];
      return info;
    }

    public PaperInfo FindPaper(Type paperType)
    {
      var info = typeIndex[paperType];
      return info;
    }

    public PaperInfo FindPaper(string path)
    {
      var info = pathIndex.FindExact(path);
      return info;
    }

    public PaperInfo FindPaperByPrefix(string path)
    {
      var info = pathIndex.FindByPrefix(path);
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

    internal static PaperInfo CreatePaperInfo<T>()
    {
      return CreatePaperInfo(typeof(T));
    }

    internal static PaperInfo CreatePaperInfo(Type paperType)
    {
      var path = PaperAttribute.Extract(paperType).UriTemplate;
      var info = new PaperInfo(paperType, path);
      return info;
    }

    public IEnumerator<PaperInfo> GetEnumerator()
    {
      return typeIndex.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return typeIndex.Values.GetEnumerator();
    }
  }
}

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
    private readonly PathIndex<Type> typeIndex = new PathIndex<Type>();

    public PaperRegistry()
    {
    }

    public PaperRegistry(IEnumerable<Type> paperTypes)
    {
      paperTypes?.Distinct().ForEach(Add);
    }

    public string[] Paths => typeIndex.Paths;

    public void AddRange(IEnumerable<Type> paperTypes)
    {
      paperTypes?.Distinct().ForEach(Add);
    }

    public void Add(Type paperType)
    {
      try
      {
        var path = PaperAttribute.Extract(paperType).UriTemplate;
        typeIndex.Add(path, paperType);
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

    public Type FindPaperType(string path)
    {
      var type = typeIndex.FindExact(path);
      return type;
    }

    public Type FindPaperTypeByPrefix(string path)
    {
      var type = typeIndex.FindByPrefix(path);
      return type;
    }

    public void PrintInfo()
    {
      foreach (var path in Paths.OrderBy(x => x))
      {
        var type = FindPaperType(path);
        Debug.WriteLine($"[PAPER]MAPPED {path} -> {type.FullName}");
        Console.WriteLine($"[PAPER]MAPPED {path} -> {type.FullName}");
      }
    }

    public IEnumerator<Type> GetEnumerator()
    {
      throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      throw new NotImplementedException();
    }
  }
}

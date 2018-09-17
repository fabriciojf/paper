using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Paper.Media;
using Paper.Media.Design;
using Paper.Media.Rendering;
using Paper.Media.Utilities;
using Toolset;
using Toolset.Collections;

namespace Paper.Media.Rendering
{
  public class PaperLocalCatalog : IPaperCatalog
  {
    private readonly PathIndex<PaperSpec> pathIndex = new PathIndex<PaperSpec>();
    private readonly Map<Type, PaperSpec> typeIndex = new Map<Type, PaperSpec>();

    public PaperLocalCatalog()
    {
    }

    public PaperLocalCatalog(IEnumerable<Type> paperTypes)
    {
      AddRange(paperTypes);
    }

    public void Add(Type paperType)
    {
      var spec = PaperSpec.GetSpec(paperType);
      typeIndex.Add(spec.Type, spec);
      pathIndex.Add(spec.Route, spec);
    }

    public void AddRange(IEnumerable<Type> paperTypes)
    {
      var specList = paperTypes.Select(PaperSpec.GetSpec).ToArray();
      foreach (var spec in specList)
      {
        typeIndex.Add(spec.Type, spec);
        pathIndex.Add(spec.Route, spec);
      }
    }

    public void AddExposedTypes()
    {
      var knownTypes =
        ExposedTypes
          .GetTypes<Entity>()
          .Concat(ExposedTypes.GetTypes<IPaper>());
      AddRange(knownTypes);
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

    public IEnumerator<PaperSpec> GetEnumerator()
    {
      return typeIndex.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return typeIndex.Values.GetEnumerator();
    }

    public static PaperLocalCatalog CreateDefaultCatalog()
    {
      var catalog = new PaperLocalCatalog();
      catalog.AddExposedTypes();
      return catalog;
    }
  }
}

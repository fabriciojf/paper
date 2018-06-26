using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Media.Service.Utilities;
using Paper.Media.Design;
using Paper.Media.Rendering;
using Toolset;
using Toolset.Collections;

namespace Media.Service.Papers
{
  public class PaperRendererRegistry : IPaperRendererRegistry
  {
    private readonly PathIndex pathIndex;
    private readonly Map<string, PaperRendererInfo> pathTemplateIndex;
    private readonly Map<Type, List<PaperRendererInfo>> paperTypeIndex;

    public PaperRendererRegistry()
    {
      pathIndex = new PathIndex();
      pathTemplateIndex = new Map<string, PaperRendererInfo>();
      paperTypeIndex = new Map<Type, List<PaperRendererInfo>>();

      // atrasando a inicialização do índex para liberar a inicialização do sistema
      Task.Run(async () =>
      {
        await Task.Delay(500);
        InitializeIndex();
      });
    }

    public void Add(PaperRendererInfo renderer)
      => Index(renderer);

    public IEnumerator<PaperRendererInfo> GetEnumerator()
      => pathTemplateIndex.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
      => pathTemplateIndex.Values.GetEnumerator();

    private void InitializeIndex()
    {
      var renderers = EnumeratePaperRenderers();
      foreach (var renderer in renderers)
      {
        Index(renderer);
      }
    }

    private IEnumerable<PaperRendererInfo> EnumeratePaperRenderers()
    {
      var rendererTypes = ExposedTypes.GetTypes<IPaperRenderer>();
      foreach (var rendererType in rendererTypes)
      {
        var renderers = EnumeratePaperRenderers(rendererType);
        foreach (var renderer in renderers)
        {
          yield return renderer;
        }
      }
    }

    private IEnumerable<PaperRendererInfo> EnumeratePaperRenderers(Type rendererType)
    {
      var attribute = rendererType.GetCustomAttribute<PaperRendererAttribute>();
      if (attribute == null)
      {
        Trace.TraceError($"O renderizador {rendererType.FullName} não está mapeado com o atributo obrigatório {typeof(PaperRendererAttribute).FullName}.");
        yield break;
      }

      var paperTypes = ExposedTypes.GetTypes(attribute.PaperContractName);
      foreach (var paperType in paperTypes)
      {
        var renderers = EnumeratePaperRenderers(rendererType, paperType);
        foreach (var renderer in renderers)
        {
          yield return renderer;
        }
      }
    }

    private IEnumerable<PaperRendererInfo> EnumeratePaperRenderers(Type rendererType, Type paperType)
    {
      var attribute = paperType.GetCustomAttribute<ExposePaperAttribute>();
      if (attribute == null)
      {
        Trace.TraceError($"A definição de página {paperType.FullName} não está mapeado com uma das subclasses de {typeof(ExposePaperAttribute).FullName}.");
        yield break;
      }

      foreach (var pathTemplate in attribute.PathTemplates)
      {
        yield return new PaperRendererInfo
        {
          PaperRendererType = rendererType,
          PaperType = paperType,
          PathTemplate = pathTemplate
        };
      }
    }

    private void Index(PaperRendererInfo renderer)
    {
      try
      {
        if (!paperTypeIndex.ContainsKey(renderer.PaperType))
          paperTypeIndex[renderer.PaperType] = new List<PaperRendererInfo>();

        pathIndex.Add(renderer.PathTemplate);
        pathTemplateIndex[renderer.PathTemplate] = renderer;
        paperTypeIndex[renderer.PaperType].Add(renderer);

        Debug.WriteLine($"[PAPER]Mapped: {renderer.PaperType.FullName}: /{renderer.PathTemplate}");
        Console.WriteLine($"[PAPER]Mapped: {renderer.PaperType.FullName}: /{renderer.PathTemplate}");
      }
      catch (Exception ex)
      {
        ex.Trace($"[PAPER]Fault: {renderer.PaperType.FullName}: {ex.Message}");
      }
    }

    public PaperRendererInfo FindPaperRenderer(string path)
    {
      var key = pathIndex.FindExact(path);
      var renderer = (key != null) ? pathTemplateIndex[key] : null;
      return renderer;
    }

    public IEnumerable<PaperRendererInfo> FindPaperRenderers(Type paperType)
    {
      return paperTypeIndex[paperType] ?? Enumerable.Empty<PaperRendererInfo>();
    }
  }
}
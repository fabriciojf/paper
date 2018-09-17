using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Media.Utilities.Types;
using Paper.Media.Rendering;
using Toolset.Collections;

namespace Paper.Media.Rendering
{
  public class PaperContext
  {
    public const string ApiPrefix = "/Api/1";

    private EntryCollection _cache;

    public PaperContext(IInjector injector, object paper, IPaperCatalog catalog, string requestUri)
    {
      var paperSpec = PaperSpec.GetSpec(paper as Type ?? paper.GetType());
      var template = paperSpec.UriTemplate;
      var prefix = ApiPrefix ?? "";

      if (!prefix.StartsWith("/"))
        prefix = "/" + prefix;
      while (prefix.EndsWith("/"))
        prefix = prefix.Substring(0, prefix.Length - 1);

      if (!template.StartsWith("/"))
        template = "/" + template;
      while (template.EndsWith("/"))
        template = template.Substring(0, template.Length - 1);

      var composedTemplate = $"{prefix}{template}";

      var uriTemplate = new UriTemplate(template);
      uriTemplate.SetArgsFromUri(requestUri);

      var args = uriTemplate.CreateArgs();

      this.Injector = injector;
      this.Paper = paper;
      this.PaperCatalog = catalog;
      this.RequestUri = requestUri;
      this.UriTemplate = template;
      this.PathArgs = args;
    }

    public IInjector Injector { get; }

    public object Paper { get; }

    public IPaperCatalog PaperCatalog { get; }

    public string RequestUri { get; }

    public string UriTemplate { get; }

    /// <summary>
    /// Argumentos coletados da URI de requisição.
    /// </summary>
    public ArgMap PathArgs { get; }

    public EntryCollection Cache => _cache ?? (_cache = new EntryCollection());
  }
}
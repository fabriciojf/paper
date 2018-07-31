using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Paper.Media.Service;
using Toolset.Collections;

namespace Paper.Media.Design.Papers.Rendering
{
  public class PaperContext
  {
    public const string ApiPrefix = "/Api/1";

    private EntryCollection _cache;

    public PaperContext(IServiceProvider serviceProvider, object paper, IEnumerable<Type> knownPapers, string requestUri)
      : this(
        serviceProvider,
        paper,
        (knownPapers != null)
          ? new PaperRegistry(paper.GetType().AsSingle().Concat(knownPapers))
          : new PaperRegistry(paper.GetType().AsSingle()),
        requestUri
      )
    {
    }

    public PaperContext(IServiceProvider serviceProvider, object paper, IPaperRegistry registry, string requestUri)
    {
      var template = PaperAttribute.Extract(paper).UriTemplate ?? "/";
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

      this.ServiceProvider = serviceProvider;
      this.Paper = paper;
      this.PaperRegistry = registry;
      this.RequestUri = requestUri;
      this.UriTemplate = template;
      this.PathArgs = args;
    }

    public IServiceProvider ServiceProvider { get; }

    public object Paper { get; }

    public IPaperRegistry PaperRegistry { get; }

    public string RequestUri { get; }

    public string UriTemplate { get; }

    /// <summary>
    /// Argumentos coletados da URI de requisição.
    /// </summary>
    public ArgMap PathArgs { get; }

    public EntryCollection Cache => _cache ?? (_cache = new EntryCollection());
  }
}
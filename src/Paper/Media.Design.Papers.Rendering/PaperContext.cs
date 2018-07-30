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

    public PaperContext(object paper, IEnumerable<Type> knownPapers, string requestUri)
    {
      IPaperRegistry registry = knownPapers as IPaperRegistry;
      if (registry == null)
      {
        registry =
          (knownPapers != null)
            ? new PaperRegistry(paper.GetType().AsSingle().Concat(knownPapers))
            : new PaperRegistry(paper.GetType().AsSingle());
      }

      this.Paper = paper;
      this.PaperRegistry = registry;
      this.RequestUri = requestUri;
      this.UriTemplate = PaperAttribute.Extract(paper).UriTemplate;
      this.PathArgs = ArgMap.ParseFromUri(this.RequestUri, this.UriTemplate, ApiPrefix);
    }

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
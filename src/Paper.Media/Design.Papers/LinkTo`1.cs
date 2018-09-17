using System;
using System.Linq;
using Media.Utilities.Types;
using Paper.Media.Rendering;
using Toolset;
using Toolset.Collections;
using Toolset.Reflection;

namespace Paper.Media.Design.Papers
{
  /// <summary>
  /// Fábrica para criação de link para outro Paper.
  /// </summary>
  /// <typeparam name="T">O tipo do Paper.</typeparam>
  /// <seealso cref="Media.Design.Extensions.Papers.ILinkFactory{Media.Design.Extensions.Papers.LinkToPaper{T}.Context}" />
  public class LinkTo<T> : ILink
    where T : IPaper
  {
    private readonly Action<T> setup;
    private readonly Action<Link> builder;

    public LinkTo(Action<T> setup = null, Action<Link> builder = null)
    {
      this.setup = setup;
      this.builder = builder;
    }

    public Link RenderLink(PaperContext ctx)
    {
      var paper = ctx.Injector.CreateInstance<T>();

      setup?.Invoke(paper);

      var link = new Link
      {
        Href = CreateHref(ctx, paper)
      };

      if (paper._Has<string>("GetTitle"))
      {
        link.Title = paper._Call<string>("GetTitle");
      }

      builder?.Invoke(link);

      if (link.Rel?.Any() != true)
        link.Rel = RelNames.Link;

      return link;
    }

    private string CreateHref(PaperContext ctx, T paper)
    {
      var paperInfo = PaperSpec.GetSpec<T>();
      var paperTemplate = new UriTemplate(paperInfo.Route);

      paperTemplate.SetArgsFromGraph(paper);

      var uri = paperTemplate.CreateUri();
      var targetUri = new Route(uri);

      targetUri = targetUri.SetArg(
        "f", ctx.PathArgs["f"],
        "in", ctx.PathArgs["in"],
        "out", ctx.PathArgs["out"]
      );

      var filter = paper._Get<IFilter>("Filter");
      if (filter != null)
      {
        var map = new FieldMap(filter);
        var args = (
          from field in map
          where field.Value != null
          select new[] {
            field.Key.ChangeCase(TextCase.CamelCase),
            field.Value
          }
        ).SelectMany();
        targetUri = targetUri.SetArg(args);
      }

      return targetUri.ToString();
    }
  }
}
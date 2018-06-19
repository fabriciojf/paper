using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Moq;
using Paper.Media;
using Paper.Media.Design;
using Paper.Media.Design.Queries;
using Paper.Media.Design.Widgets;
using Paper.Media.Rendering;
using Paper.Media.Rendering.Queries;
using Toolset;
using Toolset.Collections;
using Toolset.Reflection;
using Xunit;

namespace Paper.Test.Media.Rendering.Queries
{
  public class QueryRendererTest
  {
    private readonly Mock<IServiceProvider> injector;
    private readonly Mock<HttpContext> httpContext;
    private readonly Mock<HttpRequest> httpRequest;
    private readonly Mock<IPaperRendererRegistry> registry;

    public QueryRendererTest()
    {
      injector = new Mock<IServiceProvider>();

      httpRequest = new Mock<HttpRequest>();
      httpRequest.Setup(x => x.Scheme).Returns("http");
      httpRequest.Setup(x => x.Host).Returns(new HostString("localhost", 90));
      httpRequest.Setup(x => x.PathBase).Returns("/Tests");
      httpRequest.Setup(x => x.Method).Returns("GET");

      httpContext = new Mock<HttpContext>();
      httpContext.Setup(x => x.Request).Returns(httpRequest.Object);

      registry = new Mock<IPaperRendererRegistry>();
      registry
        .Setup(x => x.FindPaperRenderer(It.IsAny<string>()))
        .Returns((PaperRendererInfo)null);
      registry
        .Setup(x => x.FindPaperRenderers(It.IsAny<Type>()))
        .Returns(Enumerable.Empty<PaperRendererInfo>());
    }

    #region RenderEntity

    [Theory]
    [InlineData(typeof(NoCodeQuery))]
    [InlineData(typeof(NullCodeQuery))]
    public void RenderEntity_InferTitle(Type paperType)
    {
      // Given
      var template = "/";
      var path = "/";

      var renderer = new QueryRenderer(injector.Object, registry.Object);
      renderer.PathTemplate = template;
      renderer.PaperType = paperType;

      // When
      httpRequest.Setup(x => x.Path).Returns(path);
      var entity = renderer.RenderEntity(httpContext.Object, path);

      // Then
      var expected = paperType.Name.Replace("Query", "").ChangeCase(TextCase.ProperCase);
      var obtained = entity.Data?.Title;
      Assert.Equal(expected, obtained);
    }

    [Theory]
    [InlineData(typeof(EmptyCodeQuery))]
    [InlineData(typeof(DataQuery))]
    [InlineData(typeof(RowsQuery))]
    public void RenderEntity_HasTitle(Type paperType)
    {
      // Given
      var template = "/";
      var path = "/";

      var renderer = new QueryRenderer(injector.Object, registry.Object);
      renderer.PathTemplate = template;
      renderer.PaperType = paperType;

      // When
      httpRequest.Setup(x => x.Path).Returns(path);
      var entity = renderer.RenderEntity(httpContext.Object, path);

      // Then
      var expected = "Test Query";
      var obtained = entity.Data?.Title;
      Assert.Equal(expected, obtained);
    }

    [Theory]
    [InlineData(typeof(NoCodeQuery))]
    [InlineData(typeof(NullCodeQuery))]
    [InlineData(typeof(EmptyCodeQuery))]
    [InlineData(typeof(DataQuery))]
    [InlineData(typeof(RowsQuery))]
    [InlineData(typeof(DataAndRowsQuery))]
    public void RenderEntity_HasClass(Type paperType)
    {
      // Given
      var template = "/";
      var path = "/";

      var renderer = new QueryRenderer(injector.Object, registry.Object);
      renderer.PathTemplate = template;
      renderer.PaperType = paperType;

      // When
      httpRequest.Setup(x => x.Path).Returns(path);
      var entity = renderer.RenderEntity(httpContext.Object, path);

      // Then
      var expected = paperType.FullName;
      var collection = entity.Data?.Class?.ToString();
      Assert.Contains(expected, collection);
    }

    [Theory]
    [InlineData(typeof(RowsQuery))]
    [InlineData(typeof(DataAndRowsQuery))]
    public void RenderEntity_HasCustomClass(Type paperType)
    {
      // Given
      var template = "/";
      var path = "/";

      var renderer = new QueryRenderer(injector.Object, registry.Object);
      renderer.PathTemplate = template;
      renderer.PaperType = paperType;

      // When
      httpRequest.Setup(x => x.Path).Returns(path);
      var entity = renderer.RenderEntity(httpContext.Object, path);

      // Then
      var expected = "customClass";
      var collection = entity.Data?.Class?.ToString();
      Assert.Contains(expected, collection);
    }

    [Theory]
    [InlineData(typeof(RowsQuery))]
    [InlineData(typeof(DataAndRowsQuery))]
    public void RenderEntity_HasCustomRel(Type paperType)
    {
      // Given
      var template = "/";
      var path = "/";

      var renderer = new QueryRenderer(injector.Object, registry.Object);
      renderer.PathTemplate = template;
      renderer.PaperType = paperType;

      // When
      httpRequest.Setup(x => x.Path).Returns(path);
      var entity = renderer.RenderEntity(httpContext.Object, path);

      // Then
      var expected = "customRel";
      var collection = entity.Data?.Rel?.ToString();
      Assert.Contains(expected, collection);
    }

    [Theory]
    [InlineData(typeof(NoCodeQuery))]
    [InlineData(typeof(NullCodeQuery))]
    [InlineData(typeof(EmptyCodeQuery))]
    [InlineData(typeof(DataQuery))]
    [InlineData(typeof(RowsQuery))]
    [InlineData(typeof(DataAndRowsQuery))]
    public void RenderEntity_HasSelfLink(Type paperType)
    {
      // Given
      var template = "/{id}";
      var path = "/17";

      var renderer = new QueryRenderer(injector.Object, registry.Object);
      renderer.PathTemplate = template;
      renderer.PaperType = paperType;

      // When
      httpRequest.Setup(x => x.Path).Returns(path);
      var entity = renderer.RenderEntity(httpContext.Object, path);
      var selfLink = entity.Data?.Links.FirstOrDefault(x => x.Rel.Contains("self"));

      // Then
      var expected = $"http://localhost:90/Tests/17";
      var obtained = selfLink?.Href;
      Assert.Equal(expected, obtained);
    }

    [Theory]
    [InlineData(typeof(DataAndRowsQuery))]
    public void RenderEntity_HasProperties(Type paperType)
    {
      // Given
      var template = "/";
      var path = "/";

      var renderer = new QueryRenderer(injector.Object, registry.Object);
      renderer.PathTemplate = template;
      renderer.PaperType = paperType;

      // When
      httpRequest.Setup(x => x.Path).Returns(path);
      var entity = renderer.RenderEntity(httpContext.Object, path);
      var version = entity.Data?.Properties["Version"]?.Value as PropertyCollection;

      // Then
      var expected = new object[] { "TestApp", 1, 2, 3, null };
      var obtained = new object[] {
        entity.Data?.Properties["Name"]?.Value,
        version["Major"]?.Value,
        version["Minor"]?.Value,
        version["Revision"]?.Value,
        entity.Data?.Properties["Undefined"]?.Value
      };
      Assert.Equal(expected, obtained);
    }

    [Theory]
    [InlineData(typeof(DataQuery))]
    [InlineData(typeof(DataAndRowsQuery))]
    public void RenderEntity_HasData(Type paperType)
    {
      // Given
      var template = "/{id}";
      var path = "/17";

      var renderer = new QueryRenderer(injector.Object, registry.Object);
      renderer.PathTemplate = template;
      renderer.PaperType = paperType;

      // When
      httpRequest.Setup(x => x.Path).Returns(path);
      var entity = renderer.RenderEntity(httpContext.Object, path);

      // Then
      var expected = new object[] { 17, "Query17" };
      var obtained = new object[] {
        entity.Data?.Properties["Id"]?.Value,
        entity.Data?.Properties["ItemTitle"]?.Value
      };
      Assert.Equal(expected, obtained);
    }

    [Theory]
    [InlineData(typeof(DataQuery))]
    public void RenderEntity_HasData_InferHeaders(Type paperType)
    {
      // Given
      var template = "/{id}";
      var path = "/17";

      var renderer = new QueryRenderer(injector.Object, registry.Object);
      renderer.PathTemplate = template;
      renderer.PaperType = paperType;

      // When
      httpRequest.Setup(x => x.Path).Returns(path);
      var entity = renderer.RenderEntity(httpContext.Object, path);
      var headers = entity.Data?.Properties?.GetDataHeaders();

      // Then
      var expected = new object[] { "Id", "Item Title" };
      var obtained = new object[] {
        headers["Id"].Title,
        headers["ItemTitle"].Title
      };
      Assert.Equal(expected, obtained);
    }

    [Theory]
    [InlineData(typeof(DataAndRowsQuery))]
    public void RenderEntity_HasData_HasHeaders(Type paperType)
    {
      // Given
      var template = "/{id}";
      var path = "/17";

      var renderer = new QueryRenderer(injector.Object, registry.Object);
      renderer.PathTemplate = template;
      renderer.PaperType = paperType;

      // When
      httpRequest.Setup(x => x.Path).Returns(path);
      var entity = renderer.RenderEntity(httpContext.Object, path);
      var headers = entity.Data?.Properties?.GetDataHeaders();

      // Then
      var expected = new object[] { "The Id", "The Item Title" };
      var obtained = new object[] {
        headers["Id"].Title,
        headers["ItemTitle"].Title
      };
      Assert.Equal(expected, obtained);
    }

    [Theory]
    [InlineData(typeof(DataAndRowsQuery))]
    public void RenderEntity_HasRows(Type paperType)
    {
      // Given
      var template = "/{id}";
      var path = "/17";
      var queryString = "?text=Hello, world!";

      var renderer = new QueryRenderer(injector.Object, registry.Object);
      renderer.PathTemplate = template;
      renderer.PaperType = paperType;

      // When
      httpRequest.Setup(x => x.Path).Returns(path);
      httpRequest.Setup(x => x.QueryString).Returns(new QueryString(queryString));
      var entity = renderer.RenderEntity(httpContext.Object, path);
      var firstRow = entity.Data?.Entities?.FirstOrDefault();

      // Then
      var expected = new object[] { 1, "ODD", 17, "Hello, world!" };
      var obtained = new object[] {
        firstRow?.Properties?["Id"]?.Value,
        firstRow?.Properties?["Title"]?.Value,
        firstRow?.Properties?["GroupId"]?.Value,
        firstRow?.Properties?["GroupTitle"]?.Value
      };
      Assert.Equal(expected, obtained);
    }

    [Theory]
    [InlineData(typeof(RowsQuery))]
    public void RenderEntity_HasRows_InferHeaders(Type paperType)
    {
      // Given
      var template = "/{id}";
      var path = "/17";

      var renderer = new QueryRenderer(injector.Object, registry.Object);
      renderer.PathTemplate = template;
      renderer.PaperType = paperType;

      // When
      httpRequest.Setup(x => x.Path).Returns(path);
      var entity = renderer.RenderEntity(httpContext.Object, path);
      var headers = entity.Data?.Properties?.GetRowsHeaders();

      // Then
      var expected = new object[] {
        "Id",
        "Title",
        "Group Id",
        "Group Title"
      };
      var obtained = new object[] {
        headers["Id"]?.Title,
        headers["Title"]?.Title,
        headers["GroupId"]?.Title,
        headers["GroupTitle"]?.Title
      };
      Assert.Equal(expected, obtained);
    }

    [Theory]
    [InlineData(typeof(DataAndRowsQuery))]
    public void RenderEntity_HasRows_HasHeaders(Type paperType)
    {
      // Given
      var template = "/{id}";
      var path = "/17";

      var renderer = new QueryRenderer(injector.Object, registry.Object);
      renderer.PathTemplate = template;
      renderer.PaperType = paperType;

      // When
      httpRequest.Setup(x => x.Path).Returns(path);
      var entity = renderer.RenderEntity(httpContext.Object, path);
      var headers = entity.Data?.Properties?.GetRowsHeaders();

      // Then
      var expected = new object[] {
        "Custom Id",
        "Custom Title",
        "Custom Group Id",
        "Custom Group Title"
      };
      var obtained = new object[] {
        headers?["Id"]?.Title,
        headers?["Title"]?.Title,
        headers?["GroupId"]?.Title,
        headers?["GroupTitle"]?.Title
      };
      Assert.Equal(expected, obtained);
    }

    [Theory]
    [InlineData(typeof(DataAndRowsQuery))]
    public void RenderEntity_Sorted_DefaultSyntax(Type paperType)
    {
      // Given
      var template = "/{id}";
      var path = "/17";
      var queryString = "?sort[]=title&sort[]=id:desc";

      var renderer = new QueryRenderer(injector.Object, registry.Object);
      renderer.PathTemplate = template;
      renderer.PaperType = paperType;

      // When
      httpRequest.Setup(x => x.Path).Returns(path);
      httpRequest.Setup(x => x.QueryString).Returns(new QueryString(queryString));
      var entity = renderer.RenderEntity(httpContext.Object, path);

      // Then
      var expected = new object[] { 2, 3, 1 };
      var obtained = entity?.Data?.Entities?.Select(x => x.Properties["Id"]?.Value).ToArray();
      Assert.Equal(expected, obtained);
    }

    [Theory]
    [InlineData(typeof(DataAndRowsQuery))]
    public void RenderEntity_Sorted_AlternateSyntax(Type paperType)
    {
      // Given
      var template = "/{id}";
      var path = "/17";
      var queryString = "?sort=title&sort=id:desc";

      var renderer = new QueryRenderer(injector.Object, registry.Object);
      renderer.PathTemplate = template;
      renderer.PaperType = paperType;

      // When
      httpRequest.Setup(x => x.Path).Returns(path);
      httpRequest.Setup(x => x.QueryString).Returns(new QueryString(queryString));
      var entity = renderer.RenderEntity(httpContext.Object, path);

      // Then
      var expected = new object[] { 2, 3, 1 };
      var obtained = entity?.Data?.Entities?.Select(x => x.Properties["Id"]?.Value).ToArray();
      Assert.Equal(expected, obtained);
    }

    [Theory]
    [InlineData(typeof(RowsQuery))]
    public void RenderEntity_Paginated(Type paperType)
    {
      // Given
      var template = "/{id}";
      var path = "/17";
      var queryString = "?offset=1&limit=2";

      var renderer = new QueryRenderer(injector.Object, registry.Object);
      renderer.PathTemplate = template;
      renderer.PaperType = paperType;

      // When
      httpRequest.Setup(x => x.Path).Returns(path);
      httpRequest.Setup(x => x.QueryString).Returns(new QueryString(queryString));
      var entity = renderer.RenderEntity(httpContext.Object, path);

      // Then
      var expected = new object[] { 2, 3 };
      var obtained = entity?.Data?.Entities?.Select(x => x.Properties["Id"]?.Value).ToArray();
      Assert.Equal(expected, obtained);
    }

    [Theory]
    [InlineData(typeof(DataAndRowsQuery))]
    public void RenderEntity_HasLinks(Type paperType)
    {
      // Given
      var template = "/{id}";
      var path = "/17";

      var renderer = new QueryRenderer(injector.Object, registry.Object);
      renderer.PathTemplate = template;
      renderer.PaperType = paperType;

      //... mock da classe link referenciada ...
      var linkedRenderer = new PaperRendererInfo
      {
        PaperRendererType = typeof(QueryRenderer),
        PaperType = typeof(DataAndRowsQuery),
        PathTemplate = "/{id}/Link"
      };
      registry
        .Setup(x => x.FindPaperRenderers(It.IsAny<Type>()))
        .Returns(linkedRenderer.AsSingle());

      // When
      httpRequest.Setup(x => x.Path).Returns(path);
      var entity = renderer.RenderEntity(httpContext.Object, path);

      // Then
      var expected = new string[] {
        "link.com",
        "http://localhost:90/Tests/17/Link?offset=1&limit=2&sort[]=id:desc"
      };
      var obtained =
        entity?.Data?.Links
          .Where(x => x.Class?.Contains("entityLink") == true)
          .Select(x => x.Href)
          .ToArray();
      Assert.Equal(expected, obtained);
    }

    [Theory]
    [InlineData(typeof(DataAndRowsQuery))]
    public void RenderEntity_HasDataLinks(Type paperType)
    {
      var template = "/{id}";
      var path = "/17";

      var renderer = new QueryRenderer(injector.Object, registry.Object);
      renderer.PathTemplate = template;
      renderer.PaperType = paperType;

      //... mock da classe link referenciada ...
      var linkedRenderer = new PaperRendererInfo
      {
        PaperRendererType = typeof(QueryRenderer),
        PaperType = typeof(DataAndRowsQuery),
        PathTemplate = "/{id}/DataLink"
      };
      registry
        .Setup(x => x.FindPaperRenderers(It.IsAny<Type>()))
        .Returns(linkedRenderer.AsSingle());

      // When
      httpRequest.Setup(x => x.Path).Returns(path);
      var entity = renderer.RenderEntity(httpContext.Object, path);

      // Then
      var expected = new string[] {
        "data.link.com",
        "http://localhost:90/Tests/17/DataLink?offset=1&limit=2&sort[]=id:desc"
      };
      var obtained =
        entity?.Data?.Links
          .Where(x => x.Class?.Contains("dataLink") == true)
          .Select(x => x.Href)
          .ToArray();
      Assert.Equal(expected, obtained);
    }

    [Theory]
    [InlineData(typeof(DataAndRowsQuery))]
    public void RenderEntity_HasRowsLinks(Type paperType)
    {
      var template = "/{id}";
      var path = "/17";

      var renderer = new QueryRenderer(injector.Object, registry.Object);
      renderer.PathTemplate = template;
      renderer.PaperType = paperType;

      //... mock da classe link referenciada ...
      var linkedRenderer = new PaperRendererInfo
      {
        PaperRendererType = typeof(QueryRenderer),
        PaperType = typeof(DataAndRowsQuery),
        PathTemplate = "/{id}/RowLink"
      };
      registry
        .Setup(x => x.FindPaperRenderers(It.IsAny<Type>()))
        .Returns(linkedRenderer.AsSingle());

      // When
      httpRequest.Setup(x => x.Path).Returns(path);
      var entity = renderer.RenderEntity(httpContext.Object, path);
      var secondRow = entity?.Data?.Entities?.Skip(1).FirstOrDefault();

      // Then
      var expected = new string[] {
        "rows.link.com?q=2",
        "http://localhost:90/Tests/2/RowLink?offset=1&limit=2&sort[]=id:desc"
      };
      var obtained =
        secondRow?.Links
          .Where(x => x.Class?.Contains("rowLink") == true)
          .Select(x => x.Href)
          .ToArray();
      Assert.Equal(expected, obtained);
    }

    [Theory]
    [InlineData(typeof(RowsQuery))]
    [InlineData(typeof(DataAndRowsQuery))]
    public void RenderEntity_HasFilter(Type paperType)
    {
      // Given
      var template = "/{id}";
      var path = "/17";
      var queryString = "?offset=1&limit=2&sort[]=Title:desc&text=Olá, mundo!";

      var renderer = new QueryRenderer(injector.Object, registry.Object);
      renderer.PathTemplate = template;
      renderer.PaperType = paperType;

      // When
      httpRequest.Setup(x => x.Path).Returns(path);
      httpRequest.Setup(x => x.QueryString).Returns(new QueryString(queryString));
      var entity = renderer.RenderEntity(httpContext.Object, path);
      var filter = entity?.Data?.Actions.FirstOrDefault(x => x.Name.EqualsIgnoreCase("Filter"));

      // Then
      var expected = new object[] {
        "http://localhost:90/Tests/17?sort[]=title:desc",
        "Text",
        "Some Text",
        "text",
        "Olá, mundo!"
      };
      var obtained = new[] {
        filter?.Href,
        filter?.Fields.Select(x=>x.Name).FirstOrDefault(),
        filter?.Fields.Select(x=>x.Title).FirstOrDefault(),
        filter?.Fields.Select(x=>x.DataType).FirstOrDefault(),
        filter?.Fields.Select(x=>x.Value).FirstOrDefault()
      };
      Assert.Equal(expected, obtained);
    }

    #endregion

    #region Query Classes

    // nada implementado.
    class NoCodeQuery : IQuery
    {
    }

    // implementado mas retornando nulo
    class NullCodeQuery : IQuery
    {
      public object Filter => null;
      public string GetTitle() => null;
      public object GetData() => null;
    }

    // implementado mas retornando objetos e coleções vazias
    class EmptyCodeQuery : IQuery
    {
      public CustomFilter Filter { get; set; }
      public string GetTitle() => "Test Query";
      public object GetData() => new { };
    }

    class DataQuery : IQuery
    {
      public CustomFilter Filter { get; set; }
      public int Id { get; set; }
      public string GetTitle() => "Test Query";
      public object GetData() => new { Id, ItemTitle = $"Query{Id}" };
    }

    class RowsQuery : IQuery
    {
      public CustomFilter Filter { get; set; }
      public Sort Sort { get; set; } = new Sort().AddSortableFields("Id", "Title");
      public Pagination Pagination { get; set; }
      public string GetClass() => "customClass";
      public string GetRels() => "customRel";
      public string GetTitle() => "Test Query";
      public object GetRows() => RowsProvider.ProvideRows(Filter, Sort, Pagination);
    }

    class DataAndRowsQuery : IDataQuery<object>, IRowsQuery<object, CustomFilter>, IHasQueryMetadata<Info>
    {
      public int Id { get; set; }
      public CustomFilter Filter { get; set; }
      public Sort Sort { get; set; } = new Sort().AddSortableFields("Id", "Title");
      public Pagination Pagination { get; set; }

      public NameCollection GetClass()
        => "customClass";

      public NameCollection GetRels()
        => "customRel";

      public Info GetProperties()
        => new Info
        {
          Name = "TestApp",
          Version = new Info.VersionInfo
          {
            Major = 1,
            Minor = 2,
            Revision = 3
          }
        };

      public string GetTitle()
        => "Test Query";

      public Links GetLinks()
        => new Links()
          .Add("link.com")
          .AddQuery<DataAndRowsQuery>(x => {
            x.Id = Id;
            x.Pagination = new Pagination { Offset = 1, Limit = 2 };
            x.Sort.AddSort("Id", Sort.Order.Descending);
          });

      public Cols GetDataHeaders()
        => new Cols()
          .Add("Id", "The Id")
          .Add("ItemTitle", "The Item Title");

      public object GetData()
        => new { Id, ItemTitle = $"Query{Id}" };

      public Links GetDataLinks(object row)
        => new Links()
          .Add("data.link.com")
          .AddQuery<DataAndRowsQuery>(x => {
            x.Id = Id;
            x.Pagination = new Pagination { Offset = 1, Limit = 2 };
            x.Sort.AddSort("Id", Sort.Order.Descending);
          });

      public Cols GetRowsHeaders()
        => new Cols()
          .Add("Id", "Custom Id")
          .Add("Title", "Custom Title")
          .Add("GroupId", "Custom Group Id")
          .Add("GroupTitle", "Custom Group Title");

      public IEnumerable<object> GetRows()
        => RowsProvider.ProvideRows(Filter, Sort, Pagination);

      public Links GetRowLinks(object row)
        => new Links()
          .Add($"rows.link.com?q={(row.Get("Id"))}")
          .AddQuery<DataAndRowsQuery>(x => {
            x.Id = row.Get<int>("Id");
            x.Pagination = new Pagination { Offset = 1, Limit = 2 };
            x.Sort.AddSort("Id", Sort.Order.Descending);
          });
    }

    class Info
    {
      public string Name { get; set; }
      public VersionInfo Version { get; set; }
      public object Undefined => null;

      public class VersionInfo
      {
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Revision { get; set; }
      }
    }

    class CustomFilter
    {
      public int? Id { get; set; }
      public TextWidget Text { get; } = new TextWidget(nameof(Text))
      {
        Title = "Some Text",
        MaxLength = 50
      };
    }

    static class RowsProvider
    {
      public static object[] ProvideRows(CustomFilter filter, Sort sort, Pagination pagination)
      {
        var items = new[] {
          new { Id = 1, Title = "ODD" , GroupId = 17, GroupTitle = filter?.Text.Value },
          new { Id = 2, Title = "EVEN", GroupId = 17, GroupTitle = filter?.Text.Value },
          new { Id = 3, Title = "ODD" , GroupId = 17, GroupTitle = filter?.Text.Value },
          new { Id = 4, Title = "EVEN", GroupId = 37, GroupTitle = filter?.Text.Value }
        }.AsEnumerable();

        if (filter?.Id != null)
        {
          items = items.Where(x => x.GroupId == filter.Id);
        }

        sort?.SortedFields.Reverse().ForEach(field =>
        {
          if (field.FieldName == "Id")
          {
            items = (field.Order == Sort.Order.Ascending)
              ? items.OrderBy(x => x.Id)
              : items.OrderByDescending(x => x.Id);
          }
          else
          {
            items = (field.Order == Sort.Order.Ascending)
              ? items.OrderBy(x => x.Title)
              : items.OrderByDescending(x => x.Title);
          }
        });


        if (pagination != null)
        {
          items = items.Skip(pagination.Offset).Take(pagination.Limit);
        }

        return items.ToArray();
      }
    }

    #endregion
  }
}
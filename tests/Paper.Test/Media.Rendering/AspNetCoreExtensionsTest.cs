using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Moq;
using Paper.Media.Rendering;
using Xunit;

namespace Paper.Test.Media.Rendering
{
  public class AspNetCoreExtensionsTest
  {
    private readonly Mock<HttpRequest> httpRequest;

    public AspNetCoreExtensionsTest()
    {
      httpRequest = new Mock<HttpRequest>();
    }

    [Theory]
    [InlineData("http", "localhost", 90, "/Tests", "/Sample/Path", "?q=10")]
    [InlineData("http", "localhost", 90, "/Tests", "/Sample/Path", null)]
    [InlineData("http", "localhost", 90, "/Tests", null, "?q=10")]
    [InlineData("http", "localhost", 90, null, "/Sample/Path", "?q=10")]
    [InlineData(null, null, null, "/Tests", "/Sample/Path", "?q=10")]
    [InlineData(null, null, null, "/Tests", "/Sample/Path", null)]
    [InlineData(null, null, null, "/Tests", null, "?q=10")]
    [InlineData(null, null, null, null, "/Sample/Path", "?q=10")]
    public void GetRequestUri(string scheme, string host, int? port, string pathBase, string path, string queryString)
    {
      // Given
      if (scheme != null)
        httpRequest.Setup(x => x.Scheme).Returns(scheme);
      if (host != null)
        httpRequest.Setup(x => x.Host).Returns(new HostString(host, port ?? 80));
      if (pathBase != null)
        httpRequest.Setup(x => x.PathBase).Returns(pathBase);
      if (path != null)
        httpRequest.Setup(x => x.Path).Returns(path);
      if (queryString != null)
        httpRequest.Setup(x => x.QueryString).Returns(new QueryString(queryString));

      // When
      string uri = AspNetCoreExtensions.GetRequestUri(httpRequest.Object);

      // Then
      string expected = string.Concat(
        scheme,
        (scheme != null ? "://" : ""),
        host,
        (port != null) ? $":{port}" : "",
        pathBase,
        path,
        queryString
      );
      string obtained = uri;
      Assert.Equal(expected, obtained);
    }
  }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Toolset;
using Toolset.Collections;
using Xunit;

namespace Paper.Media.Routing
{
  public class DefaultArgsTest
  {
    [Fact]
    public void ParseArgs_ByName_Test()
    {
      // Given
      var templateUri = "/Items/{id}/Skus/{sku}?ean={code}&on={active}";
      var requestUri = "/Items/10/Skus/20?on&ean=78912349";

      // When
      var args = new DefaultArgs(templateUri, requestUri);

      // Then
      var expected = new object[] { "10", "20", "78912349", "1" };
      var obtained = new[]{
        args["id"],
        args["sku"],
        args["code"],
        args["active"]
      };
      Assert.Equal(expected, obtained);
    }

    [Fact]
    public void ParseArgs_ByIndex_Test()
    {
      // Given
      var templateUri = "/Items/{id}/Skus/{sku}?ean={code}&on={active}";
      var requestUri = "/Items/10/Skus/20?on&ean=78912349";

      // When
      var args = new DefaultArgs(templateUri, requestUri);

      // Then
      var expected = new object[] { "10", "20", "78912349", "1" };
      var obtained = new[]{
        args[0],
        args[1],
        args[2],
        args[3]
      };
      Assert.Equal(expected, obtained);
    }
  }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Paper.Media.Rendering.Utilities;
using Toolset.Reflection;
using Xunit;

namespace Paper.Test.Media.Rendering.Utilities
{
  public class UriUtilTest
  {
    #region ParseQueryString

    [Fact]
    public void ParseQueryString_String()
    {
      // Given
      var queryString = "?empty=&text=hello, world!";
      // When
      var args = UriUtil.ParseQueryString(queryString);
      // Then
      var expected = new object[] { null, "hello, world!" };
      var obtained = new object[] { args["empty"], args["text"] };
      Assert.Equal(expected, obtained);
    }

    [Fact]
    public void ParseQueryString_Boolean()
    {
      // Given
      var queryString = "?up=true&down=false&forward";
      // When
      var args = UriUtil.ParseQueryString(queryString);
      // Then
      var expected = new object[] { true, false, true };
      var obtained = new object[] { args["up"], args["down"], args["forward"] };
      Assert.Equal(expected, obtained);
    }

    [Fact]
    public void ParseQueryString_Integer()
    {
      // Given
      var queryString = "?zero=0&plus=010&minus=-020";
      // When
      var args = UriUtil.ParseQueryString(queryString);
      // Then
      var expected = new object[] { 0, 10, -20 };
      var obtained = new object[] { args["zero"], args["plus"], args["minus"] };
      Assert.Equal(expected, obtained);
    }

    [Fact]
    public void ParseQueryString_Decimal()
    {
      // Given
      var queryString = "?debit=-20.02&credit=15,51";
      // When
      var args = UriUtil.ParseQueryString(queryString);
      // Then
      var expected = new object[] { -20.02M, 15.51M };
      var obtained = new object[] { args["debit"], args["credit"] };
      Assert.Equal(expected, obtained);
    }

    [Fact]
    public void ParseQueryString_DateTime()
    {
      // Given
      var queryString = "?at=2020-12-31T23:59:59&on=2020-12-31";
      // When
      var args = UriUtil.ParseQueryString(queryString);
      // Then
      var expected = new object[] {
        new DateTime(2020, 12, 31, 23, 59, 59),
        new DateTime(2020, 12, 31)
      };
      var obtained = new object[] { args["at"], args["on"] };
      Assert.Equal(expected, obtained);
    }

    [Fact]
    public void ParseQueryString_Timespan()
    {
      // Given
      var queryString = "?interval=23:59&delay=10.23:59:59.999";
      // When
      var args = UriUtil.ParseQueryString(queryString);
      // Then
      var expected = new object[] {
        new TimeSpan(23, 59, 0),
        new TimeSpan(10, 23, 59, 59, 999)
      };
      var obtained = new object[] { args["interval"], args["delay"] };
      Assert.Equal(expected, obtained);
    }

    [Fact]
    public void ParseQueryString_ArrayWithNoItems()
    {
      // Given
      var queryString = "?ids[]=";
      // When
      var args = UriUtil.ParseQueryString(queryString);
      // Then
      var array = args["ids"] as object[];
      Assert.Empty(array);
    }

    [Fact]
    public void ParseQueryString_ArrayWithOneItem()
    {
      // Given
      var queryString = "?ids[]=10";
      // When
      var args = UriUtil.ParseQueryString(queryString);
      // Then
      var expected = new object[] { 10 };
      var obtained = args["ids"] as object[];
      Assert.Equal(expected, obtained);
    }

    [Fact]
    public void ParseQueryString_Array()
    {
      // Given
      var queryString = "?ids=1&ids[]=2&ids=3";
      // When
      var args = UriUtil.ParseQueryString(queryString);
      // Then
      var expected = new object[] { 1, 2, 3 };
      var obtained = args["ids"] as object[];
      Assert.Equal(expected, obtained);
    }

    [Fact]
    public void ParseQueryString_Range()
    {
      // Given
      var queryString = "?num.min=-20&num.max=20&age.min=2002-12-31&top.max=100.001";
      // When
      var args = UriUtil.ParseQueryString(queryString);
      // Then
      var expected = new object[] {
        -20, 20,
        new DateTime(2002, 12, 31), null,
        null, 100.001M
      };
      var obtained = new object[] {
        args["num"]?.Get("Min"), args["num"]?.Get("Max"),
        args["age"]?.Get("Min"), args["age"]?.Get("Max"),
        args["top"]?.Get("Min"), args["top"]?.Get("Max")
      };
      Assert.Equal(expected, obtained);
    }

    #endregion

    #region ToQueryString

    [Fact]
    public void ToQueryString_String()
    {
      // Given
      IDictionary args = new Dictionary<string, object>
      {
        { "empty", null },
        { "text", "hello, world!" }
      };
      // When
      var queryString = UriUtil.ToQueryString(args);
      // Then
      var expected = "?text=hello, world!";
      var obtained = queryString;
      Assert.Equal(expected, obtained);
    }

    [Fact]
    public void ToQueryString_Boolean()
    {
      // Given
      IDictionary args = new Dictionary<string, object>
      {
        { "up", true },
        { "down", false }
      };
      // When
      var queryString = UriUtil.ToQueryString(args);
      // Then
      var expected = "?up=true&down=false";
      var obtained = queryString;
      Assert.Equal(expected, obtained);
    }

    [Fact]
    public void ToQueryString_Integer()
    {
      // Given
      IDictionary args = new Dictionary<string, object>
      {
        { "zero", 0 },
        { "plus", 10 },
        { "minus", -20 }
      };
      // When
      var queryString = UriUtil.ToQueryString(args);
      // Then
      var expected = "?zero=0&plus=10&minus=-20";
      var obtained = queryString;
      Assert.Equal(expected, obtained);
    }

    [Fact]
    public void ToQueryString_Decimal()
    {
      // Given
      IDictionary args = new Dictionary<string, object>
      {
        { "debit", -20.02M },
        { "credit", 15.51M }
      };
      // When
      var queryString = UriUtil.ToQueryString(args);
      // Then
      var expected = "?debit=-20.02&credit=15.51";
      var obtained = queryString;
      Assert.Equal(expected, obtained);
    }

    [Fact]
    public void ToQueryString_DateTime()
    {
      // Given
      IDictionary args = new Dictionary<string, object>
      {
        { "at", new DateTime(2002, 12, 31, 23, 59, 59) },
        { "on", new DateTime(2002, 12, 31) },
      };
      // When
      var queryString = UriUtil.ToQueryString(args);
      // Then
      var pattern = "at=2002-12-31T23:59:59-0.:00&on=2002-12-31T00:00:00-0.:00";
      Assert.Matches(pattern, queryString);
    }

    [Fact]
    public void ToQueryString_TimeSpan()
    {
      // Given
      IDictionary args = new Dictionary<string, object>
      {
        { "delay", new TimeSpan(10, 23, 59, 59) }
      };
      // When
      var queryString = UriUtil.ToQueryString(args);
      // Then
      var expected = "?delay=10.23:59:59";
      var obtained = queryString;
      Assert.Equal(expected, obtained);
    }

    [Fact]
    public void ToQueryString_ArrayWithNoItems()
    {
      // Given
      IDictionary args = new Dictionary<string, object>
      {
        { "arr", new object[0] }
      };
      // When
      var queryString = UriUtil.ToQueryString(args);
      // Then
      Assert.Empty(queryString);
    }

    [Fact]
    public void ToQueryString_ArrayWithOneItem()
    {
      // Given
      IDictionary args = new Dictionary<string, object>
      {
        { "ten", new object[] { 10 } }
      };
      // When
      var queryString = UriUtil.ToQueryString(args);
      // Then
      var expected = "?ten[]=10";
      var obtained = queryString;
      Assert.Equal(expected, obtained);
    }

    [Fact]
    public void ToQueryString_Array()
    {
      // Given
      IDictionary args = new Dictionary<string, object>
      {
        { "ids", new object[] { 1, 2, 3 } }
      };
      // When
      var queryString = UriUtil.ToQueryString(args);
      // Then
      var expected = "?ids[]=1&ids[]=2&ids[]=3";
      var obtained = queryString;
      Assert.Equal(expected, obtained);
    }

    [Fact]
    public void ToQueryString_Range()
    {
      // Given
      IDictionary args = new Dictionary<string, object>
      {
        { "num", new { Min = -20, Max = 20 } },
        { "age", new { min = new DateTime(2002, 12, 31) } },
        { "top", new { max = 100.001M } },
      };
      // When
      var queryString = UriUtil.ToQueryString(args);
      // Then
      var pattern = @"num\.min=-20&num\.max=20&age\.min=2002-12-31T00:00:00-0.:00&top\.max=100\.001";
      Assert.Matches(pattern, queryString);
    }

    [Fact]
    public void ToQueryString_FromObject()
    {
      // Given
      var args = new
      {
        empty = (string)null,
        text = "hello, world!",
        up = true,
        down = false,
        zero = 0,
        plus = 10,
        minus = -20,
        debit = -20.02M,
        credit = 15.51M,
        at = new DateTime(2002, 12, 31, 23, 59, 59),
        on = new DateTime(2002, 12, 31),
        delay = new TimeSpan(10, 23, 59, 59),
        arr = new object[0],
        ten = new object[] { 10 },
        ids = new object[] { 1, 2, 3 },
        num = new { Min = -20, Max = 20 },
        age = new { min = new DateTime(2002, 12, 31) },
        top = new { max = 100.001M }
      };
      // When
      var queryString = UriUtil.ToQueryString(args);
      // Then
      var pattern = 
          @"text=hello, world!"
        + @"&up=true"
        + @"&down=false"
        + @"&zero=0"
        + @"&plus=10"
        + @"&minus=-20"
        + @"&debit=-20\.02"
        + @"&credit=15\.51"
        + @"&at=2002-12-31T23:59:59-0.:00"
        + @"&on=2002-12-31T00:00:00-0.:00"
        + @"&delay=10\.23:59:59"
        + @"&ten\[\]=10"
        + @"&ids\[\]=1&ids\[\]=2&ids\[\]=3"
        + @"&num\.min=-20&num\.max=20"
        + @"&age\.min=2002-12-31T00:00:00-0.:00"
        + @"&top\.max=100\.001";
      Assert.Matches(pattern, queryString);
    }

    #endregion
  }
}
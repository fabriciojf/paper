using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Toolset;
using Toolset.Collections;

namespace Paper.Media.Papers
{
  public class Sort : IEnumerable<SortField>
  {
    private readonly List<SortField> fields = new List<SortField>();

    public IEnumerable<string> FieldNames => fields.Select(x => x.Name);

    public bool Contains(string fieldName)
    {
      return fields.Any(x => x.Name.EqualsIgnoreCase(fieldName));
    }

    public SortField this[string fieldName]
    {
      get => fields.Where(x => x.Name.EqualsIgnoreCase(fieldName)).FirstOrDefault();
      set
      {
        fields.RemoveAll(x => x.Name.EqualsIgnoreCase(fieldName));
        if (value != null)
        {
          value.Name = fieldName;
          fields.Add(value);
        }
      }
    }

    public void CopyFromUri(string uri, string argName)
    {
      var queryString = uri.Split('?').Skip(1).FirstOrDefault();
      if (queryString == null)
      {
        if (uri.Contains("="))
        {
          queryString = uri;
        }
      }

      if (queryString != null)
      {
        var fields = (
          from token in queryString.Split('&')
          let parts = token.Split('=')
          where parts.Length == 2
          let key = parts.First()
          where key.EqualsAnyIgnoreCase(argName, $"{argName}[]")
          let field = parts.Last()
          where !string.IsNullOrWhiteSpace(field)
          let specs = field.Split(':')
          let fieldName = specs.First()
          let fieldOrder = specs.Skip(1).LastOrDefault()
          select new
          {
            fieldName,
            fieldOrder =
              fieldOrder.EqualsAnyIgnoreCase("desc", "descending")
                ? SortOrder.Descending : SortOrder.Ascending
          }
        );

        foreach (var field in fields)
        {
          var instance = this[field.fieldName];
          if (instance != null)
          {
            instance.Order = field.fieldOrder;
          }
        }
      }
    }

    public string CopyToUri(string uri, string argName)
    {
      return CopyToUri(new Route(uri), argName);
    }

    public Route CopyToUri(Route uri, string argName)
    {
      foreach (var field in fields)
      {
        if (field.Order == SortOrder.Ascending)
        {
          uri.SetArg($"{argName}[]", $"{field.Name}");
        }
        else if (field.Order == SortOrder.Descending)
        {
          uri.SetArg($"{argName}[]", $"{field.Name}:desc");
        }
      }
      return uri;
    }

    public static Sort CreateFromUri(string uri, string argName)
    {
      var sort = new Sort();
      sort.CopyFromUri(uri, argName);
      return sort;
    }

    public override string ToString()
    {
      return string.Join(",", this);
    }

    public IEnumerator<SortField> GetEnumerator()
    {
      return fields.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return fields.GetEnumerator();
    }
  }
}
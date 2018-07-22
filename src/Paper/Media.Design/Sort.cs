using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Toolset;
using Toolset.Collections;

namespace Paper.Media.Design
{
  public class Sort
  {
    private readonly List<string> _fields = new List<string>();
    private readonly List<SortedField> _sortedFields = new List<SortedField>();

    public Sort()
    {
    }

    public Sort(params string[] fields)
    {
      _fields.AddRange(fields);
    }

    public Sort(IEnumerable<string> fields)
    {
      _fields.AddRange(fields);
    }

    #region Operacoes com Fields

    public IEnumerable<string> Names => _fields;

    public bool Contains(string fieldName)
    {
      return _fields.Contains(fieldName);
    }

    public void Add(string field)
    {
      if (!_fields.Contains(field))
      {
        _fields.Add(field);
      }
    }

    public void AddRange(IEnumerable<string> fields)
    {
      _fields.AddRange(fields.Except(_fields));
    }

    public void Remove(string fieldName)
    {
      _fields.Remove(fieldName);
    }

    public void Clear()
    {
      _fields.Clear();
      _sortedFields.Clear();
    }

    #endregion

    #region Operacoes com SortedFields

    public IEnumerable<string> SortedFieldNames => _sortedFields.Select(x => x.Name);

    public IEnumerable<SortedField> SortedFields => _sortedFields;

    public SortedField? GetSortedField(string fieldName)
    {
      var field =
        _sortedFields
          .Where(x => x.Name.EqualsIgnoreCase(fieldName))
          .Select(x => (SortedField?)x)
          .FirstOrDefault();
      return field;
    }

    public bool ContainsSortedField(string fieldName)
    {
      return _sortedFields.Any(x => x.Name.EqualsIgnoreCase(fieldName));
    }

    public void AddSortedField(string field, SortOrder order)
    {
      AddSortedField(new SortedField(field, order));
    }

    public void AddSortedField(SortedField field)
    {
      var isValid = field.Name.EqualsAnyIgnoreCase(_fields);
      if (!isValid)
        throw new Exception("O campo não está disponível para ser ordenado: " + field.Name);

      _sortedFields.Add(field);
    }

    public void RemoveSortedField(string fieldName)
    {
      _sortedFields.RemoveAll(x => x.Name.EqualsIgnoreCase(fieldName));
    }

    public void ClearSortedFields()
    {
      _sortedFields.Clear();
    }

    #endregion

    public void CopyFromUri(string uri, string argName)
    {
      ClearSortedFields();

      var queryString = uri.Split('?').Skip(1).FirstOrDefault();
      if (queryString == null)
      {
        if (uri.Contains("="))
        {
          queryString = uri;
        }
        else
        {
          return;
        }
      }

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
        AddSortedField(field.fieldName, field.fieldOrder);
      }
    }

    public string CopyToUri(string uri, string argName)
    {
      var route = new Route(uri);
      foreach (var field in SortedFields)
      {
        if (field.Order == SortOrder.Descending)
        {
          route = route.SetArg($"{argName}[]", $"{field.Name}:desc");
        }
        else
        {
          route = route.SetArg($"{argName}[]", $"{field.Name}");
        }
      }
      return route;
    }
  }
}
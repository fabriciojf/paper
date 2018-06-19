using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolset;

namespace Paper.Media.Design
{
  public class Sort
  {
    public enum Order
    {
      Ascending,
      Descending
    }

    public class SortField
    {
      public SortField(string fieldName, Order order)
      {
        this.FieldName = fieldName;
        this.Order = order;
      }

      public string FieldName { get; }

      public Order Order { get; }

      public override string ToString()
      {
        if (Order == Order.Ascending) return $"{FieldName} ASC";
        if (Order == Order.Descending) return $"{FieldName} DESC";
        return FieldName;
      }
    }

    private readonly List<string> _sortableFields = new List<string>();
    private readonly List<SortField> _sortedFields = new List<SortField>();

    public Sort()
    {
      EnableDefaultSorting();
    }

    public IEnumerable<string> SortableFields => _sortableFields;
    public IEnumerable<SortField> SortedFields => _sortedFields;

    public bool IsSortableByDefault { get; private set; }

    public Sort EnableDefaultSorting()
    {
      IsSortableByDefault = true;
      _sortableFields.Clear();
      return this;
    }

    public Sort DisableDefaultSorting()
    {
      IsSortableByDefault = false;
      _sortableFields.Clear();
      return this;
    }

    public Sort AddSortableFields(params string[] fieldNames)
    {
      IsSortableByDefault = false;
      var fields = fieldNames.Except(_sortableFields).ToArray();
      _sortableFields.AddRange(fields);
      return this;
    }

    public Sort AddSort(string fieldName, Order order)
    {
      if (IsSortableByDefault)
      {
        _sortedFields.Add(new SortField(fieldName, order));
        return this;
      }

      fieldName = _sortableFields.FirstOrDefault(x => x.EqualsIgnoreCase(fieldName));
      if (fieldName != null)
      {
        _sortedFields.Add(new SortField(fieldName, order));
      }

      return this;
    }

    public override string ToString()
    {
      return string.Join(",", _sortedFields);
    }
  }
}

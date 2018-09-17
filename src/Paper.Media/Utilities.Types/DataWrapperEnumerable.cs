
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Paper.Media.Design;
using Toolset;
using Toolset.Reflection;

namespace Media.Utilities.Types
{
  /// <summary>
  /// Utilitário de inspeção de propriedades de um objeto.
  /// </summary>
  internal class DataWrapperEnumerable : IEnumerable<DataWrapper>
  {
    private readonly IEnumerable items;
    private readonly DataWrapper reference;
    private readonly Func<DataWrapper> wrapperFactory;
    
    private DataWrapperEnumerable(
        object dataSource
      , int count
      , IEnumerable items
      , DataWrapper reference
      , Func<DataWrapper> wrapperFactory)
    {
      this.DataSource = dataSource;
      this.Count = count;
      this.items = items;
      this.reference = reference;
      this.wrapperFactory = wrapperFactory;
    }

    /// <summary>
    /// Objeto original dos dados encapsulados pela instância.
    /// </summary>
    public object DataSource { get; private set; }

    public int Count { get; set; }

    /// <summary>
    /// Instancia o navegador de coleção de dado.
    /// Tipos suportados:
    /// -   DataTable
    /// -   IEnumerable of Object
    /// </summary>
    /// <param name="data">The data.</param>
    /// <returns>O navegador de itens</returns>
    public static DataWrapperEnumerable Create(object data)
    {
      var dataTable = data as DataTable;
      if (dataTable != null)
      {
        var items = dataTable.Rows;

        var reference = new DataWrapper.DataRowInspector();
        reference.DataSource = dataTable;

        return new DataWrapperEnumerable(
          data,
          dataTable.Rows.Count,
          items,
          reference,
          () => new DataWrapper.DataRowInspector()
        );
      }
      else
      {
        var items = (IEnumerable)data;

        var reference = new DataWrapper.GraphInspector();
        reference.DataSource = items.Cast<object>().FirstOrDefault();

        var count =
          (items as IList)?.Count
          ?? (items as ICollection)?.Count
          ?? (items.Cast<object>().Count());

        return new DataWrapperEnumerable(
          data,
          count,
          items,
          reference,
          () => new DataWrapper.GraphInspector()
        );
      }
    }

    /// <summary>
    /// Enumera os nomes de propriedades do objeto.
    /// </summary>
    /// <returns>Os nomes de propriedades do objeto.</returns>
    public IEnumerable<string> EnumerateKeys()
    {
      return reference.EnumerateKeys();
    }

    /// <summary>
    /// Obtém informação de cabelalho sobre a propriedade.
    /// </summary>
    /// <param name="key">O nome da propriedade.</param>
    /// <returns>Inforações de cabeçalho da propriedade.</returns>
    public HeaderInfo GetHeader(string key)
    {
      return reference.GetHeader(key);
    }

    public IEnumerator<DataWrapper> GetEnumerator()
    {
      return EnumerateItems().Take(Count).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    private IEnumerable<DataWrapper> EnumerateItems()
    {
      var wrapper = wrapperFactory.Invoke();
      foreach (var item in items)
      {
        wrapper.DataSource = item;
        yield return wrapper;
      }
    }
  }
}
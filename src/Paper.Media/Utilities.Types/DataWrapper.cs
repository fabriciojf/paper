
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Paper.Media;
using Paper.Media.Design;
using Paper.Media.Design.Mappings;
using Toolset;
using Toolset.Reflection;

namespace Media.Utilities.Types
{
  /// <summary>
  /// Utilitário de inspeção de propriedades de um objeto.
  /// </summary>
  internal abstract class DataWrapper
  {
    /// <summary>
    /// Objeto original dos dados encapsulados pela instância.
    /// </summary>
    public abstract object DataSource { get; set; }

    /// <summary>
    /// Enumera os nomes de propriedades do objeto.
    /// </summary>
    /// <returns>Os nomes de propriedades do objeto.</returns>
    public abstract IEnumerable<string> EnumerateKeys();

    /// <summary>
    /// Obtém informação de cabelalho sobre a propriedade.
    /// </summary>
    /// <param name="key">O nome da propriedade.</param>
    /// <returns>Inforações de cabeçalho da propriedade.</returns>
    public abstract HeaderInfo GetHeader(string key);

    /// <summary>
    /// Obtém o valor da propriedade.
    /// </summary>
    /// <param name="key">O nome da propriedade.</param>
    /// <returns>O valor da propriedade.</returns>
    public abstract object GetValue(string key);

    private DataWrapper()
    {
    }

    /// <summary>
    /// Cria uma instância de DataWrapper capaz de inspecionar o dado especificado.
    /// 
    /// Tipos suportados:
    /// -   DataRow
    /// -   IDictionary
    /// -   Objeto
    /// 
    /// Caso um DataTable seja indicado o primeiro DataRow será usado como referência.
    /// </summary>
    /// <param name="data">O dado a ser inspecionado.</param>
    public static DataWrapper Create(object data)
    {
      if (data is DataTable dataTable)
      {
        return new DataRowInspector() { DataSource = dataTable };
      }

      if (data is DataRow dataRow)
      {
        return new DataRowInspector() { DataSource = dataRow };
      }

      if (data is IDictionary dictionary)
      {
        return new DictionaryInspector() { DataSource = dictionary };
      }

      return new GraphInspector() { DataSource = data };
    }

    /// <summary>
    /// Inspeção para DataRow.
    /// </summary>
    /// <seealso cref="Media.Design.Extensions.Papers.Rendering.DataWrapper" />
    internal class DataRowInspector : DataWrapper
    {
      // NOTA: Pode ser nulo.
      private DataTable table;

      // NOTA: Pode ser nulo.
      private DataRow row;

      private object _currentData;

      public override object DataSource
      {
        get => _currentData;
        set
        {
          _currentData = value;
          if (value is DataTable)
          {
            this.table = (DataTable)value;
            this.row =
              (this.table != null && this.table.Rows.Count > 0)
                ? this.table.Rows[0] : null;
          }
          else
          {
            this.row = (DataRow)value;
            this.table = this.row?.Table;
          }
        }
      }

      public override IEnumerable<string> EnumerateKeys()
      {
        if (table != null)
        {
          foreach (DataColumn column in table.Columns)
          {
            yield return Conventions.MakeName(column);
          }
        }
      }

      public override HeaderInfo GetHeader(string key)
      {
        if (table == null)
          return null;

        var column = table.Columns[key];
        var property = column._GetPropertyInfo(key);
        var header = new HeaderInfo
        {
          Name = Conventions.MakeName(column),
          Title = Conventions.MakeTitle(column),
          DataType = Conventions.MakeDataType(column)
        };
        header.Hidden = header.Name.StartsWith("_");

        var fieldHidden = property.GetCustomAttributes(true).OfType<FieldHiddenAttribute>().FirstOrDefault();
        if (fieldHidden != null)
        {
          header.Hidden = fieldHidden.Hidden;
        }

        return header;
      }

      public override object GetValue(string key)
      {
        return row?[key];
      }
    }

    /// <summary>
    /// Inspeção para dicionário.
    /// </summary>
    /// <seealso cref="Media.Design.Extensions.Papers.Rendering.DataWrapper" />
    internal class DictionaryInspector : DataWrapper
    {
      // NOTA: Pode ser nulo.
      private IDictionary dictionary;

      public override object DataSource
      {
        get => dictionary;
        set => dictionary = (IDictionary)value;
      }

      public override IEnumerable<string> EnumerateKeys()
      {
        if (dictionary != null)
        {
          foreach (string key in dictionary.Keys)
          {
            yield return key;
          }
        }
      }

      public override HeaderInfo GetHeader(string key)
      {
        if (dictionary == null)
          return null;

        var header = new HeaderInfo
        {
          Name = Conventions.MakeName(key),
          Title = Conventions.MakeTitle(key),
          DataType = DataTypeNames.GetDataTypeName(dictionary[key])
        };
        header.Hidden = header.Name.StartsWith("_");
        return header;
      }

      public override object GetValue(string key)
      {
        return dictionary?[key];
      }
    }

    /// <summary>
    /// Inspeção de propriedades de objeto.
    /// </summary>
    /// <seealso cref="Media.Design.Extensions.Papers.Rendering.DataWrapper" />
    internal class GraphInspector : DataWrapper
    {
      // NOTA: Pode ser nulo.
      private object data;

      public override object DataSource
      {
        get => data;
        set => data = value;
      }

      public override IEnumerable<string> EnumerateKeys()
      {
        return
          (data != null)
            ? data._GetPropertyNames()
            : Enumerable.Empty<string>();
      }

      public override HeaderInfo GetHeader(string key)
      {
        if (data == null)
          return null;

        var property = data._GetPropertyInfo(key);
        var header = new HeaderInfo
        {
          Name = Conventions.MakeName(property),
          Title = Conventions.MakeTitle(property),
          DataType = DataTypeNames.GetDataTypeName(property.PropertyType)
        };
        header.Hidden = header.Name.StartsWith("_");

        var fieldHidden = property.GetCustomAttributes(true).OfType<FieldHiddenAttribute>().FirstOrDefault();
        if (fieldHidden != null)
        {
          header.Hidden = fieldHidden.Hidden;
        }

        return header;
      }

      public override object GetValue(string key)
      {
        return data?._Get(key);
      }
    }
  }
}
using System;
using System.Linq;
using System.Collections.Generic;
using Paper.Media;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Toolset;
using Toolset.Serialization;
using Toolset.Serialization.Json;
using Toolset.Reflection;
using System.Globalization;
using System.Runtime.Serialization;
using Paper.Media.Design;
using Toolset.Data;
using Paper.Media.Utilities;

namespace Paper.Media.Serialization
{
  /// <summary>
  /// Seriaizador de hipermídia para o formato Siren+JSON.
  /// </summary>
  public class SirenSerializer : ISerializer
  {
    #region Variações de Serialize()

    /// <summary>
    /// Serializa a entidade para a saída indicada.
    /// </summary>
    /// <param name="entity">Entidade a ser serializada.</param>
    /// <param name="output">Stream de saída.</param>
    public void Serialize(Entity entity, TextWriter output)
    {
      Write(output, entity);
    }

    /// <summary>
    /// Serializa a entidade para a saída indicada.
    /// </summary>
    /// <param name="entity">Entidade a ser serializada.</param>
    /// <param name="output">Stream de saída.</param>
    public void Serialize(Entity entity, Stream output)
    {
      TextWriter writer = new StreamWriter(output);
      Serialize(entity, writer);
    }

    /// <summary>
    /// Serializa a entidade para string.
    /// </summary>
    /// <param name="entity">Entidade a ser serializada.</param>
    public string Serialize(Entity entity)
    {
      using (var writer = new StringWriter())
      {
        Serialize(entity, writer);
        return writer.ToString();
      }
    }

    #endregion

    #region Variações de Deserialize()

    /// <summary>
    /// Deserializa o texto para uma instância de Entity.
    /// </summary>
    /// <param name="input">A entrada para leitura do texto a ser deserializado.</param>
    /// <returns>A entidade obtida da serialização.</returns>
    public Entity Deserialize(TextReader input)
    {
      var model = ParseJson(input);
      var entity = new Entity();
      CopyNodeProperties(model, entity);
      return entity;
    }

    /// <summary>
    /// Deserializa o texto para uma instância de Entity.
    /// </summary>
    /// <param name="input">A entrada para leitura do texto a ser deserializado.</param>
    /// <returns>A entidade obtida da serialização.</returns>
    public Entity Deserialize(Stream input)
    {
      return Deserialize(new StreamReader(input));
    }

    /// <summary>
    /// Deserializa o texto para uma instância de Entity.
    /// </summary>
    /// <param name="text">O texto a ser deserializado.</param>
    /// <returns>A entidade obtida da serialização.</returns>
    public Entity Deserialize(string text)
    {
      using (var reader = new StringReader(text))
      {
        return Deserialize(reader);
      }
    }

    #endregion

    #region Parsing

    /// <summary>
    /// Copia as propriedades do modelo deserializado para o objeto indicado.
    /// As propriedades são correspondidas por nome insensível a caso.
    /// </summary>
    /// <param name="sourceNode">O modelo deserializado.</param>
    /// <param name="targetGraph">O objeto destino das propriedades.</param>
    private void CopyNodeProperties(NodeModel sourceNode, object targetGraph)
    {
      foreach (var property in sourceNode.ChildProperties())
      {
        var info = targetGraph._GetPropertyInfo(property.Name);
        if (info == null)
          continue;

        var propertyValue = CreateCompatibleValue(info.PropertyType, property.Value);

        if (propertyValue != null)
        {
          var isCaseVariantString =
            info
              .GetCustomAttributes(true)
              .OfType<CaseVariantStringAttribute>()
              .Any();

          if (isCaseVariantString)
          {
            propertyValue = propertyValue.ToString().ChangeCase(TextCase.PascalCase);
          }
        }

        targetGraph._Set(property.Name, propertyValue);
      }
    }

    /// <summary>
    /// Cria um valor compatível para o tipo indicado com base no conteúdo
    /// do modelo deserializado.
    /// </summary>
    /// <param name="type">O tipo do dado esperado.</param>
    /// <param name="node">O modelo deserializado.</param>
    /// <returns>O valor obtido da compatibilização.</returns>
    private object CreateCompatibleValue(Type type, NodeModel node)
    {
      if (node == null)
      {
        return null;
      }

      if (type == typeof(PropertyCollection))
      {
        return (PropertyCollection)CreateCompatibleValue(typeof(object), node);
      }

      if (type == typeof(NameCollection))
      {
        var target = new NameCollection();
        foreach (var item in node.ChildValues())
        {
          target.Add(item.Value.ToString());
        }
        return target;
      }

      if (type == typeof(FieldCollection))
      {
        var target = new FieldCollection();
        foreach (var @object in node.ChildObjects())
        {
          var field = new Field();
          CopyNodeProperties(@object, field);
          target.Add(field);
        }
        return target;
      }

      if (type == typeof(LinkCollection))
      {
        var target = new LinkCollection();
        foreach (var item in node.Children())
        {
          var link = new Link();
          CopyNodeProperties(item, link);
          target.Add(link);
        }
        return target;
      }

      if (type == typeof(EntityActionCollection))
      {
        var target = new EntityActionCollection();
        foreach (var item in node.Children())
        {
          var action = new EntityAction();
          CopyNodeProperties(item, action);
          target.Add(action);
        }
        return target;
      }

      if (type == typeof(EntityCollection))
      {
        var target = new EntityCollection();
        foreach (var item in node.Children())
        {
          var entity = new Entity();
          CopyNodeProperties(item, entity);
          target.Add(entity);
        }
        return target;
      }

      if (type == typeof(CaseVariantString))
      {
        var text = (node as ValueModel)?.Value.ToString();
        return text.ChangeCase(TextCase.PascalCase);
      }

      return CreateCompatibleValue(node);
    }
    
    /// <summary>
    /// Cria um valor compatível com base no conteúdo do modelo deserializado.
    /// </summary>
    /// <param name="node">O modelo deserializado.</param>
    /// <returns>O valor obtido da compatibilização.</returns>
    private object CreateCompatibleValue(NodeModel node)
    {
      {
        if (node is ValueModel)
        {
          return ((ValueModel)node).Value;
        }

        if (node is ObjectModel)
        {
          var propertyCollection = new PropertyCollection();
          foreach (var child in node.ChildProperties())
          {
            var value = child.Value;

            object convertedValue;

            if (value is ValueModel)
            {
              convertedValue = ((ValueModel)value).Value;
            }
            else
            {
              convertedValue = CreateCompatibleValue(typeof(object), value);
            }

            var property = new Property();
            property.Name = child.Name.ChangeCase(TextCase.PascalCase);
            property.Value = convertedValue;

            propertyCollection.Add(property);
          }
          return propertyCollection;
        }

        if (node is CollectionModel)
        {
          var collection = new List<object>();
          foreach (var value in node.Children())
          {
            object convertedValue;

            if (value is ValueModel)
            {
              convertedValue = ((ValueModel)value).Value;
            }
            else
            {
              convertedValue = CreateCompatibleValue(typeof(object), value);
            }

            collection.Add(convertedValue);
          }
          return collection;
        }
      }

      throw new Exception("Conteúdo não suportado: " + node.GetType().FullName);
    }

    /// <summary>
    /// Obtém um modelo deserializado do JSON indicado.
    /// O modelo é uma representação navegável do conteúdo do JSON deserializado.
    /// </summary>
    /// <param name="input">A entrada para leitura do JSON.</param>
    /// <returns>O modelo deserializado.</returns>
    private NodeModel ParseJson(TextReader input)
    {
      var settings = new SerializationSettings { IsFragment = true };
      using (var reader = new JsonReader(input, settings))
      using (var writer = new DocumentWriter(settings))
      {
        reader.CopyTo(writer);
        var model = writer.TargetDocument.Root;
        return model;
      }
    }

    #endregion

    #region Funções de serialização

    private void WriteProperty(TextWriter writer, string property, object value, ref int count)
    {
      if (value == null)
        return;

      if (count > 0)
        writer.Write(",");

      var name = MakeCompatibleName(property);

      writer.Write("\"");
      writer.Write(name);
      writer.Write("\":");

      Write(writer, value);

      count += 1;
    }

    private void Write(TextWriter writer, object value)
    {
      if (value is CaseVariantString)
      {
        // apenas ajusta o valor de acordo com a caixa e prossegue...
        value = MakeCompatibleName(value.ToString());
      }

      if (value is IVar)
      {
        // var any = (Any)value;
        // // apenas extrai o valor real de Any e prossegue...
        // if (any.IsText) value = any.Text;
        // if (any.
        value = ((IVar)value).Value;
      }

      if (IsNull(value))
      {
        writer.Write("null");
        return;
      }

      if (value is DateTime)
      {
        writer.Write("\"");
        writer.Write(((DateTime)value).ToString("yyyy-MM-ddTHH:mm:sszzz"));
        writer.Write("\"");
        return;
      }

      if (StringUtils.IsStringCompatible(value))
      {
        writer.Write("\"");
        writer.Write(Toolset.Json.Escape(value.ToString()));
        writer.Write("\"");
        return;
      }

      if (value is bool)
      {
        writer.Write((bool)value ? 1 : 0);
        return;
      }

      if (value is IFormattable)
      {
        var formattable = (IFormattable)value;
        var text = formattable.ToString(null, CultureInfo.InvariantCulture);
        writer.Write(text);
        return;
      }

      if (value is NameCollection)
      {
        var items = (NameCollection)value;

        writer.Write("[");

        var comma = false;
        foreach (var item in items)
        {
          if (comma) writer.Write(',');
          comma = true;

          writer.Write("\"");
          writer.Write(Toolset.Json.Escape(item));
          writer.Write("\"");
        }

        writer.Write("]");
        return;
      }

      if (value is PropertyCollection)
      {
        var items = (PropertyCollection)value;
        
        writer.Write("{");

        int count = 0;
        foreach (var item in items)
        {
          WriteProperty(writer, item.Name, item.Value, ref count);
        }

        writer.Write("}");
        return;
      }

      if (value is IEnumerable)
      {
        var items = (IEnumerable)value;

        writer.Write("[");

        bool comma = false;
        foreach (var item in items)
        {
          if (comma) writer.Write(",");
          comma = true;

          Write(writer, item);
        }

        writer.Write("]");
        return;
      }

      writer.Write("{");

      var properties = (
        from property in value.GetType().GetProperties()
        where !property.GetIndexParameters().Any()
        select property
      ).ToArray();

      int propertyCount = 0;
      foreach (var property in properties)
      {
        var propertyValue = property.GetValue(value);
        if (IsEmpty(propertyValue))
          continue;

        // Se a string for CaseVariantStringAttribute temos que converte-la para camelCase
        if (property.GetCustomAttributes().OfType<CaseVariantStringAttribute>().Any())
        {
          propertyValue = (CaseVariantString)propertyValue.ToString();
        }
        // Se estamos renderizando um link a sua coleção "Rel" precisa ser convertida para camelCase
        else if (value is Link)
        {
          if (property.Name == "Rel")
          {
            propertyValue = 
              ((NameCollection)propertyValue)
                .Select(x => (CaseVariantString)x)
                .ToArray();
          }
        }

        var propertyName = MakeCompatibleName(property);

        WriteProperty(writer, propertyName, propertyValue, ref propertyCount);
      }

      writer.Write("}");
    }

    private bool IsEmpty(object value)
    {
      if (value == null) return true;
      if (value is IEnumerable)
      {
        var any = ((IEnumerable)value).Cast<object>().Any();
        return !any;
      }
      return false;
    }

    private bool IsNull(object value)
    {
      return value == null || value == DBNull.Value;
    }

    private bool IsNumeric(object value)
    {
      if (value is short
       || value is int
       || value is long
       || value is float
       || value is double
       || value is decimal
      )
      {
        return true;
      }

      if (value is string)
      {
        var text = (string)value;
        return text != "" && text.Length <= 9 && text.All(c => char.IsDigit(c) || char.IsNumber(c));
      }

      return false;
    }

    private string MakeCompatibleName(PropertyInfo property)
    {
      var attr = property._GetAttr<DataMemberAttribute>();
      var name = attr?.Name ?? property.Name;
      return MakeCompatibleName(name);
    }

    private string MakeCompatibleName(string originalName)
    {
      var tokens = originalName.Split('.');
      var parts =
        from token in tokens
        select Json.Escape(token.ChangeCase(TextCase.CamelCase));
      var name = string.Join(".", parts);
      return name;
    }

    #endregion
  }
}
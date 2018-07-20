using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Toolset;

namespace Toolset.Xml
{
  /// <summary>
  /// Utilitário de aplicação de XPath contra objetos do domínio System.Xml.Linq.
  /// </summary>
  public class XPathUtil
  {
    private readonly XmlNamespaceManager xmlns = new XmlNamespaceManager(new NameTable());

    /// <summary>
    /// Registra um namespace associado a um prefixo para ser usado nas expressões
    /// XPath.
    /// 
    /// Uma vez registrado a expressão XPath pode referenciar o namespace pelo seu
    /// prefixo. Exemplo: "//nfe:enviNFe"
    /// </summary>
    /// <param name="prefix">O prefixo de referência do namespace.</param>
    /// <param name="xmlNamespace">O namespace registrado.</param>
    public void AddNamespace(string prefix, string xmlNamespace)
    {
      xmlns.AddNamespace(prefix, xmlNamespace);
    }

    /// <summary>
    /// Aplica uma expressão XPath e retorna o resultado convertido para o tipo
    /// especificado.
    /// 
    /// O resultado contém os valores encontrados pela aplicação do XPath que são
    /// compatíveis com o tipo desejado. Os valores não compatíveis são descartados.
    /// 
    /// Se um array for indicado todos os valores encontrados compatíveis com o
    /// tipo do array são retornados.
    /// 
    /// Se o tipo desejado não for um array apenas o primeiro resultado encontrado
    /// compatível com o tipo desejado é retornado.
    /// 
    /// O tipo anulável pode ser indicado, como "int?".
    /// Neste caso se nenhum valor for encontrado nulo é retornado.
    /// </summary>
    /// <example>
    /// 
    /// var util = new XPathUtil();
    /// 
    /// // Registra o namespace
    /// util.AddNamespace("nfe", "http://www.portalfiscal.inf.br/nfe");
    /// 
    /// // Retorna o atributo ID como uma string
    /// var result = util.XPath(xml, "//nfe:NFe//@Id");
    /// 
    /// // Retorna o número da nota como um inteiro
    /// var result = util.XPath<int>(xml, "//nfe:NFe//nNF");
    /// 
    /// // Retorna os números de todos os itens encontrados
    /// var result = util.XPath<int[]>(xml, "//nfe:NFe//nfe:det/@nItem");
    /// 
    /// // Retorna as tags correspondetes às notas encontradas
    /// var result = util.XPath<XElement[]>(xml, "//nfe:NFe//nfe:det/nfe:prod");
    /// 
    /// // Retorna a data de recebimento da nota se houver, senão, retorna nulo
    /// var result = util.XPath<DateTime?>(xml, "//nfe:infProt//dhRecbto");
    /// 
    /// </example>
    /// <typeparam name="T">
    /// O tipo de retorno esperado.
    /// Há suporte para:
    /// -   Tipos básicos do C#.
    /// -   Tipo anulável para os tipos básicos do c#, como "int?", etc.
    /// -   XElement e XAttribute
    /// -   Array dos tipos acima, como int[], XElement[], etc.
    /// </typeparam>
    /// <param name="xml">O XML a ser pesquisado.</param>
    /// <param name="xpath">O XPath a ser aplicado.</param>
    /// <returns>O resultado convertido para o tipo indicado.</returns>
    public T XPath<T>(XContainer xml, string xpath)
    {
      var result = xml.XPathEvaluate(xpath, xmlns);

      IEnumerable<object> values;
      if (result is string)
        values = new object[] { result };
      else if (result is IEnumerable)
        values = ((IEnumerable)result).OfType<object>();
      else
        values = new object[] { result };

      var targetType = typeof(T);
      if (!targetType.IsArray)
        values = values.Take(1);

      var isNullableType =
        (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>));

      Type valueType = typeof(T);
      if (isNullableType)
        valueType = Nullable.GetUnderlyingType(targetType);
      else if (targetType.IsArray)
        valueType = targetType.GetElementType();
      else
        valueType = targetType;

      var convertedValues = values.Select(x => CastTo(x, valueType));

      if (typeof(T).IsArray)
      {
        var list = (
          from item in convertedValues
          where item != null
             && item.GetType() == valueType
          select item
        ).ToArray();

        var array = Array.CreateInstance(valueType, list.Length);
        list.CopyTo(array, 0);
        return (T)(object)array;
      }
      else
      {
        var singleValue = convertedValues.FirstOrDefault();
        return (singleValue != null) ? (T)singleValue : default(T);
      }
    }

    /// <summary>
    /// Tenta converter o valor para o tipo indicado.
    /// </summary>
    /// <param name="value">O valor a ser convertido.</param>
    /// <param name="type">O tipo desejado.</param>
    /// <returns>O valor convertido.</returns>
    private object CastTo(object value, Type type)
    {
      try
      {

        if (value == null)
          return null;

        if (value.GetType() == type)
          return value;

        if (type == typeof(XElement))
          return value as XElement;

        if (type == typeof(XAttribute))
          return value as XAttribute;

        string text = null;

        if (value is XElement)
          text = ((XElement)value).Value;
        else if (value is XDocument)
          text = ((XDocument)value).Root.Value;
        else if (value is XAttribute)
          text = ((XAttribute)value).Value;

        return ConvertStringTo(text, type);

      }
      catch
      {
        //Debug.WriteLine(string.Format(
        //  "[XPATH]Falhou a tentativa de realizar uma conversão para '{0}': {1}"
        //  , type.Name, value
        //));
        //Debug.WriteLine(ex.GetStackTrace());

        // nada a fazer. o valor não é conversivel.
        return null;
      }
    }

    /// <summary>
    /// Aplica uma transformação de tipo para converter o texto
    /// indicado 
    /// </summary>
    /// <param name="text">O texto a ser convertido.</param>
    /// <param name="type">O tipo desejado.</param>
    /// <returns>O valor convertido.</returns>
    private object ConvertStringTo(string text, Type type)
    {
      try
      {

        if (text == null)
          return null;

        if (text.GetType() == type)
          return text;

        var isNullableType =
          (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));

        var rawType =
          isNullableType
            ? Nullable.GetUnderlyingType(type)
            : type;

        object convertedValue = null;

        // se o valor é do tipo esperado nao precisamos de conversao
        if (rawType.IsInstanceOfType(text))
        {
          convertedValue = text;
        }
        else if (rawType.IsSubclassOf(typeof(Enum)))
        {
          var enumValue = Change.To<int>(text);
          convertedValue = Enum.ToObject(rawType, enumValue);
        }
        else
        {
          convertedValue = Change.To(text, rawType);
        }

        if (isNullableType)
        {
          convertedValue = Activator.CreateInstance(type, convertedValue);
        }

        return convertedValue;

      }
      catch
      {
        //Debug.WriteLine(string.Format(
        //  "[XPATH]Falhou a tentativa de realizar uma conversão para '{0}': {1}"
        //  , type.Name, text
        //));
        //Debug.WriteLine(ex.GetStackTrace());

        // nada a fazer. o valor não é conversivel.
        return null;
      }
    }
  }
}

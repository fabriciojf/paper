using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Toolset.Xml;

namespace Toolset.Text.Template
{
  internal static class XmlExtensions
  {
    private readonly static XPathUtil xPath;

    static XmlExtensions()
    {
      xPath = new XPathUtil();
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
    /// // Registra o namespace
    /// xml.AddNamespace("nfe", "http://www.portalfiscal.inf.br/nfe");
    /// 
    /// // Retorna o atributo ID como uma string
    /// var result = xml.XPath("//nfe:NFe//@Id");
    /// 
    /// // Retorna o número da nota como um inteiro
    /// var result = xml.XPath<int>("//nfe:NFe//nNF");
    /// 
    /// // Retorna o número de todos os itens encontrados
    /// var result = xml.XPath<int[]>("//nfe:NFe//nfe:det/@nItem");
    /// 
    /// // Retorna as tags correspondetes às notas encontradas
    /// var result = xml.XPath<XElement[]>("//nfe:NFe//nfe:det/nfe:prod");
    /// 
    /// // Retorna a data de recebimento da nota se houver, senão, retorna nulo
    /// var result = xml.XPath<DateTime?>("//nfe:infProt//dhRecbto");
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
    public static T XPath<T>(this XContainer xml, string xpath)
    {
      return xPath.XPath<T>(xml, xpath);
    }

    /// <summary>
    /// Aplica uma expressão XPath e retorna a primeira string encontrada.
    /// </summary>
    /// <example>
    /// 
    /// // Retorna o atributo ID como uma string
    /// var result = xml.XPath("//nfe:NFe//@Id");
    /// 
    /// </example>
    /// <param name="xml">O XML a ser pesquisado.</param>
    /// <param name="xpath">O XPath a ser aplicado.</param>
    /// <returns>A string encontrada..</returns>
    public static string XPath(this XContainer xml, string xpath)
    {
      return xPath.XPath<string>(xml, xpath);
    }

    /// <summary>
    /// Aplica uma expressão XPath e retorna todos os resultados convertidos para o tipo
    /// especificado.
    /// 
    /// O resultado contém os valores encontrados pela aplicação do XPath que são
    /// compatíveis com o tipo desejado. Os valores não compatíveis são descartados.
    /// </summary>
    /// <example>
    /// 
    /// // Retorna os números de todos os itens encontrados
    /// var result = xml.XPathAll<int>("//nfe:NFe//nfe:det/@nItem");
    /// 
    /// // Retorna as tags correspondetes às notas encontradas
    /// var result = xml.XPathAll<XElement>("//nfe:NFe//nfe:det/nfe:prod");
    /// 
    /// </example>
    /// <typeparam name="T">
    /// O tipo de retorno esperado.
    /// Há suporte para:
    /// -   Tipos básicos do C#.
    /// -   XElement e XAttribute
    /// </typeparam>
    /// <param name="xml">O XML a ser pesquisado.</param>
    /// <param name="xpath">O XPath a ser aplicado.</param>
    /// <returns>Um array de resultados convertidos para o tipo indicado.</returns>
    public static T[] XPathAll<T>(this XContainer xml, string xpath)
    {
      return xPath.XPath<T[]>(xml, xpath);
    }

    /// <summary>
    /// Aplica uma expressão XPath e retorna todas as strings encontradas.
    /// </summary>
    /// <example>
    /// 
    /// // Retorna as chaves das notas
    /// var result = xml.XPathAll("//nfe:NFe//@Id");
    /// 
    /// </example>
    /// <param name="xml">O XML a ser pesquisado.</param>
    /// <param name="xpath">O XPath a ser aplicado.</param>
    /// <returns>As strings encontradas.</returns>
    public static string[] XPathAll(this XContainer xml, string xpath)
    {
      return xPath.XPath<string[]>(xml, xpath);
    }
  }
}
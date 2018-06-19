using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Toolset.Text.Template
{
  /// <summary>
  /// Realiza a conversão de um texto no padrão TagPath para o padrão XPath.
  /// </summary>
  internal static class TagPath
  {
    /// <summary>
    /// Realiza a conversão de um texto no padrão TagPath para o padrão XPath.
    /// 
    /// Padrão TagPath
    /// --------------
    /// 
    /// O padrão TagPath é um formato próprio do Director.Sped, com o caminho da tag
    /// no documento definida em uma estrutura simples de nomes separados por pontos
    /// e curingas para representar nomes indenfidos.
    /// Estes são os formatos possíveis:
    ///   -   tag
    ///       Representa uma "tag" em qualquer parte do documento
    ///       Corresponde ao XPath: /tag
    ///   -   tagPai.tag
    ///       Representa uma "tag" imediatamente filha da "tagPai"
    ///       Corresponde ao XPath: /tagPai/tag
    ///   -   tagAvo.*.tag
    ///       Representa uma "tag" que pertence à tag "tagAvo" mas cujo pai é irrelevante
    ///       Corresponde ao XPath: /tagAvo/*/tag
    ///   -   tagRaiz..tag
    ///       Representa uma "tag" que pertence à tag "tagRaiz" independentemente de
    ///       quantas tags existam entre eles.
    ///       Corresponde ao XPath: /tagAvo//tag
    ///   -   tag.N
    ///       Representa uma tag numa posição específica.
    ///       N neste caso representa um índica
    ///       Corresponde ao XPath: /tag[N], exemplo: /tag[2]
    /// 
    /// Os padrões acima podem ser mixados para indicar o parentesco necessário, exemplo:
    ///   -   *.raiz..pai.tag.5
    ///       Corresponde ao XPath: //raiz//pai/tag[5]
    /// </summary>
    /// <param name="tagPath">O caminho da tag no formato TagPath.</param>
    /// <returns>O XPath correspondente.</returns>
    public static string ConvertTagPathToXPath(string tagPath)
    {
      if (tagPath == null)
        return null;

      tagPath = tagPath.Replace("..", ".**.");
      var tokens = tagPath.Split('.').Select(token =>
      {
        if (token == "") return "";
        if (token == "*") return "/*";
        if (token == "**") return "//*";
        if (char.IsDigit(token.FirstOrDefault())) return "[" + token + "]";
        if (token.FirstOrDefault() == '@') return "/" + token;
        return "/*[local-name()='" + token + "']";
      });
      
      return string.Join("", tokens);
    }
  }
}

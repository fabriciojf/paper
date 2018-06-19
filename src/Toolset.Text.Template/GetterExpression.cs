using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Collections;

namespace Toolset.Text.Template
{
  /// <summary>
  /// Expressão de acesso a propriedade de objeto ou navegação em XML por TagPath.
  /// </summary>
  class GetterExpression : Expression
  {
    private string path;

    public GetterExpression(string path)
    {
      this.path = path;
    }

    internal override Pipe Evaluate(Pipe input, object target, object context)
    {
      var searchPath = path;
      var searchTarget = target;

      if (path == "")
      {
        return new Pipe(input);
      }

      if (path.IsNumber())
      {
        object element = null;
        var index = int.Parse(path);

        if (index == 0)
        {
          element = input;
        }
        else if (input.Array != null)
        {
          element = input.Array.ElementAtOrDefault(index - 1);
        }

        return new Pipe(element);
      }

      if ((path == "ctx") || path.StartsWith("ctx."))
      {
        var length = (path == "ctx") ? 3 : 4;
        searchPath = path.Substring(length);
        searchTarget = context;
      }

      if (searchTarget is XContainer)
      {
        var result = GetXmlTag(searchPath, (XContainer)searchTarget);
        return new Pipe(result);
      }
      else
      {
        var result = GetProperty(searchPath, searchTarget);
        return new Pipe(result);
      }
    }

    /// <summary>
    /// Realiza uma pesquisa no XML pela tag indicada pelo TagPath.
    /// </summary>
    /// <param name="tagPath">A expressão TagPath.</param>
    /// <param name="xml">O XML pesquisado.</param>
    /// <returns>O resultado da pesquisa.</returns>
    private static object GetXmlTag(string tagPath, XContainer xml)
    {
      var xPath = TagPath.ConvertTagPathToXPath(tagPath);
      var result = xml.XPath(xPath);
      return result;
    }

    /// <summary>
    /// Resolve propriedade de objetos recursivamente.
    /// </summary>
    /// <param name="property">
    /// O caminho da propriedade separado por pontos.
    /// Exemplo:
    ///   ctx.user.name
    /// </param>
    /// <param name="graph">O objeto que será analisado.</param>
    /// <returns>O valor da propriedade ou nulo.</returns>
    private static object GetProperty(string property, object graph)
    {
      try
      {
        var tokens =
          from p in property.Split('.')
          where !string.IsNullOrEmpty(p)
          select p;

        foreach (var token in tokens)
        {
          if (graph == null)
            break;

          if (graph is NameValueCollection)
          {
            var adapter = new NameValueCollectionAdapter((NameValueCollection)graph);
            graph = adapter.GetValue(token);
            continue;
          }

          if (graph is IDictionary)
          {
            var adapter = new DictionaryAdapter((IDictionary)graph);
            graph = adapter.GetValue(token);
            continue;
          }

          if (graph is IKeyValueCollection)
          {
            graph = ((IKeyValueCollection)graph).GetValue(token);
            continue;
          }

          var method = (
            from p in graph.GetType().GetProperties()
            where p.Name.Equals(token, StringComparison.InvariantCultureIgnoreCase)
            select p
          ).FirstOrDefault();

          graph = (method != null) ? method.GetValue(graph, null) : null;
        }

        return graph;

      }
      catch (Exception ex)
      {
        ex.Report("Não foi possível resolver a propriedade: " + property);
        return null;
      }
    }

  }
}

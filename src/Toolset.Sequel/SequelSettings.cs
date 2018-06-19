using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using Toolset;

namespace Toolset.Sequel
{
  /// <summary>
  /// Coleção dos parâmetros gerais do Sequel.
  /// </summary>
  public static class SequelSettings
  {
    static SequelSettings()
    {
      QueryTemplateEnabled = true;
      StandardTemplateEnabled = true;
      ExtendedTemplateEnabled = true;
    }

    /// <summary>
    /// Nome da aplicação.
    /// O Sequel repassa este nome de aplicativo para a conexão
    /// com base de dados para facilitar a sua identificação.
    /// </summary>
    public static string DefaultApplicationName { get; set; }

    /// <summary>
    /// Ativa a impressão das SQLs executadas para depuração.
    /// </summary>
    public static bool TraceQueries { get; set; }

    /// <summary>
    /// Ativa ou desativa a aplicação automática de templates.
    /// Os padrões de template Standard e Extended são aplicados
    /// quando respectivamente ativados pelas propriedades:
    /// StandardTemplateEnabled e ExtendedTemplateEnabled.
    /// </summary>
    public static bool QueryTemplateEnabled { get; set; }

    /// <summary>
    /// Ativa ou desativa o uso de StandardTemplate na composição de SQL.
    /// Leia mais em SqlTemplate.ApplyStandardTemplate().
    /// </summary>
    public static bool StandardTemplateEnabled { get; set; }

    /// <summary>
    /// Ativa ou desativa o uso de StandardTemplate na composição de SQL.
    /// Leia mais em SqlTemplate.ApplyExtendedTemplate().
    /// </summary>
    public static bool ExtendedTemplateEnabled { get; set; }

    /// <summary>
    /// Coleção de configurações chaveadas.
    /// </summary>
    /// <typeparam name="T">Tipo da configuração.</typeparam>
    public class SettingsBag<T>
    {
      private readonly Dictionary<string, T> bag;

      internal SettingsBag(int capacity)
      {
        bag = new Dictionary<string, T>(capacity);
      }

      /// <summary>
      /// Valor padrão da propriedade.
      /// Se aplica quando não há um valor nomeado definido.
      /// </summary>
      public T DefaultValue
      {
        get { return bag.ContainsKey(string.Empty) ? bag[string.Empty] : default(T); }
        set { bag[string.Empty] = value; }
      }

      /// <summary>
      /// Valor nomeado de uma propriedade.
      /// Quando não definido, o valor padrão é retornado.
      /// </summary>
      /// <param name="key">O nome da propriedade.</param>
      /// <returns>O valor da propriedade ou o valor padrão, quando não definida.</returns>
      public T this[string key]
      {
        get { return bag.ContainsKey(key) ? bag[key] : DefaultValue; }
        set { bag[key] = value; }
      }
    }

  }
}

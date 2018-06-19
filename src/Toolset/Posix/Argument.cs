using System.Reflection;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Toolset.Posix
{
  /// <summary>
  /// Argumento de linha de comando.
  /// Um argumento pode ser:
  /// -   Opção
  ///     Na forma curta:
  ///         tool -o
  ///     Ou na forma longa:
  ///         tool --option
  ///     E pode conter um ou mais valores:
  ///         tool --option VAL1 VAL2 ...
  /// -   Operando
  ///     Argumentos não-opção, variáveis e sem representação nominal.
  ///     Como "MySolution" em:
  ///         tool build MySolution.sln
  /// </summary>
  public class Argument
  {
    private string _category;

    /// <summary>
    /// Ordem de exibição do argumento na ajuda.
    /// </summary>
    public int Order { get; set; }
    /// <summary>
    /// A propriedade é uma opção POSIX.
    /// Uma opção inicia com '-' e pode ter valor.
    /// Um traço para o nome curto formado por uma letra.
    /// Dois traços para o nome longo formado por duas ou mais letras.
    /// Como os parâmetros '-d' e '--recursive' em:
    ///   list -d "c:\" --recursive
    /// </summary>
    public bool IsOption { get; set; }
    /// <summary>
    /// A propriedade é um operando POSIX.
    /// Operando é qualquer argumento não-opção.
    /// Como os nomes de arquivo e pasta em:
    ///   copy "arquivo.x" "pasta/tal"
    /// </summary>
    public bool IsOperand { get; set; }
    /// <summary>
    /// Determina se a opção espera pelo menos um argumento.
    /// </summary>
    public bool HasArg { get; set; }
    /// <summary>
    /// Determina se o argumento suporta mais de um valor.
    /// Quando verdadeiro implica em HasArg ser tambem verdadeiro.
    /// </summary>
    public bool HasManyArgs { get { return HasArg && Value is OptArgArray; } }
    /// <summary>
    /// Nome curto de uma opção.
    /// O nome curto é formado por apenas uma letra precedida por '-'.
    /// </summary>
    public char? Flag { get; set; }
    /// <summary>
    /// Nome do argumento.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Categoria opcional para classificação de opções.
    /// </summary>
    public string Category
    {
      get => _category;
      set => _category = string.IsNullOrWhiteSpace(value) ? null : value;
    }
    /// <summary>
    /// Nome do valor de uma opção.
    /// Vários nomes podem ser indicados separados por espaços.
    /// </summary>
    public string ArgName { get; set; }
    /// <summary>
    /// Determina se variáveis de ambiente devem ser expandidas anteas
    /// da avaliação do valor.
    /// </summary>
    public bool ExpandEnvironmentVariables { get; set; }
    /// <summary>
    /// Texto de documentação do argumento.
    /// </summary>
    public string Help { get; set; }
    /// <summary>
    /// Seção de ajuda adicional exibida para o comando.
    /// </summary>
    public string HelpSection { get; set; }
    /// <summary>
    /// Instância do objeto que declara as opções.
    /// Este será o objeto modificado com as opções encontradas na linha de comando.
    /// </summary>
    public object Host { get; set; }
    /// <summary>
    /// Instância do método correspondente ao argumento.
    /// Em conjunto com Host este método permite obter e definir o 
    /// valor corrente da opção.
    /// </summary>
    public PropertyInfo Accessor { get; set; }
    /// <summary>
    /// Propriedade utilitária para obter e definir o valor da
    /// propriedade no objeto de opções.
    /// </summary>
    public Opt Value
    {
      get { return (Opt)Accessor.GetValue(Host, null); }
      set { Accessor.SetValue(Host, value, null); }
    }
    /// <summary>
    /// Retorna uma opção para o seu valor padrão.
    /// </summary>
    public void Reset()
    {
      try
      {
        Value = Activator.CreateInstance(Accessor.PropertyType, true) as Opt;
      }
      catch (Exception ex)
      {
        throw new Exception($"Falha construindo uma instância de: {Accessor.PropertyType}", ex);
      }
    }
  }
}

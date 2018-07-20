using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolset
{
  [Flags]
  public enum TextCase
  {
    Default = 0x000,
    KeepOriginal = 0x001,

    //
    // capitalização
    //
    UpperCase = 0x002,
    LowerCase = 0x004,
    ProperCase = 0x008,

    //
    // separação
    //
    Hyphenated = 0x010,
    Underscore = 0x020,
    Dotted = 0x040,
    Spaced = 0x080,
    Joined = 0x100,

    //
    // casos especiais
    //
    CamelCase = 0x200 | ProperCase | Joined,

    //
    // composições
    //
    AllCaps = UpperCase | Underscore,
    PascalCase = ProperCase | Joined,

    //
    // opções
    //

    /// <summary>
    /// Desativa a preservação de delimitadores antes e depois.
    /// Por exemplo: "__id" se torna "id".    
    /// </summary>
    NoPrefix = 0x1000,

    /// <summary>
    /// Preserva os caracteres especiais.
    /// Caracteres preservados: ? ! # $ % & @ * + / \ ; < > = ^ ~ ( ) [ ] { }
    /// </summary>
    PreserveSpecialCharacters = 0x2000
  }
}

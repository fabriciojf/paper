using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using Toolset.Reflection;

namespace Paper.Media.Design.Widgets.Mapping
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public class WidgetAttribute : Attribute
  {
    private string _type;

    public WidgetAttribute()
    {
    }

    protected WidgetAttribute(string dataType, string htmlInputType = null)
    {
      DataType = dataType;
      _type = htmlInputType;
    }

    /// <summary>
    /// Nome do campo.
    /// </summary>
    public virtual string Name { get; set; }

    /// <summary>
    /// Tipo do componente de edição do campo.
    /// 
    /// É aceito qualquer um dos tipos convencionados para o HTML5:
    /// - hidden
    /// - text
    /// - search
    /// - tel
    /// - url
    /// - email
    /// - password
    /// - datetime
    /// - date
    /// - month
    /// - week
    /// - time
    /// - datetime-local
    /// - number
    /// - range
    /// - color
    /// - checkbox
    /// - radio
    /// - file
    /// 
    /// A lista completa está definida na classe FieldTypeNames.
    /// </summary>
    public virtual string Type => (Hidden == true) ? KnownFieldTypes.Hidden : _type;

    /// <summary>
    /// Tipo do valor do campo.
    /// 
    /// Tipos aceitos:
    /// - text (string)
    /// - bit (bool, boolean)
    /// - number (int, long)
    /// - decimal (double, float)
    /// - date
    /// - time
    /// - datetime
    /// 
    /// O texto em parêntesis representa um nome alternativo, ou apelido, para o tipo.
    /// 
    /// A lista completa está definida na classe FieldDataTypeNames.
    /// </summary>
    public virtual string DataType { get; }

    /// <summary>
    /// Tipo de objeto do campo.
    /// 
    /// Mais de um tipo pode ser indicado para
    /// campos com mais de um comportamento.
    /// 
    /// A ordem dos tipos importa. O aplicativo cliente pode
    /// optar por refletir apenas o primeiro tipo definido, ou o
    /// formato de serialização pode não suportar mais de um tipo.
    /// Sempre defina o tipo primário antes dos tipos alternativos.
    /// </summary>
    public virtual NameCollection Class { get; set; }

    /// <summary>
    /// Título do campo. Opcional.
    /// </summary>
    public virtual string Title { get; set; }

    /// <summary>
    /// Texto adicional com uma breve instrução de uso do widget.
    /// </summary>
    public string Placeholder { get; set; }

    /// <summary>
    /// Ativa ou desativa a obrigatoriedade de preenchimento do campo.
    /// </summary>
    public virtual bool Required { get; set; }

    /// <summary>
    /// Torna o campo editável ou somente leitura.
    /// </summary>
    public virtual bool ReadOnly { get; set; }

    /// <summary>
    /// Ativa ou desativa a ocultação do campo.
    /// </summary>
    public virtual bool Hidden { get; set; }
  }
}
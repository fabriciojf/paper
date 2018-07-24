using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Paper.Media.Design.Papers.Rendering;
using Paper.Media.Utils;
using Toolset;
using Toolset.Collections;
using Toolset.Reflection;

namespace Paper.Media.Design
{
  public static class EntityActionFieldsExtensions
  {
    #region AddField<T>

    public static EntityAction AddField<T>(this EntityAction action, Expression<Func<T, object>> keySelector, Action<Field> builder = null)
    {
      string fieldName;
      string fieldTitle;
      string fieldDataTtype;

      ExtractInfo(keySelector, out fieldName, out fieldTitle, out fieldDataTtype);

      DoAddField(
          action
        , fieldName
        , fieldTitle
        , fieldDataTtype
        , builder
        , false
      );

      return action;
    }

    public static EntityAction AddField<T>(this EntityAction action, Expression<Func<T, object>> keySelector, string title)
    {
      string fieldName;
      string fieldTitle;
      string fieldDataTtype;

      ExtractInfo(keySelector, out fieldName, out fieldTitle, out fieldDataTtype);

      DoAddField(
          action
        , fieldName
        , title ?? fieldTitle
        , fieldDataTtype
        , null
        , false
      );

      return action;
    }

    public static EntityAction AddFieldMulti<T>(this EntityAction action, Expression<Func<T, object>> keySelector, Action<Field> builder = null)
    {
      string fieldName;
      string fieldTitle;
      string fieldDataTtype;

      ExtractInfo(keySelector, out fieldName, out fieldTitle, out fieldDataTtype);

      DoAddField(
          action
        , fieldName
        , fieldTitle
        , fieldDataTtype
        , builder
        , true
      );

      return action;
    }

    public static EntityAction AddFieldMulti<T>(this EntityAction action, Expression<Func<T, object>> keySelector, string title)
    {
      string fieldName;
      string fieldTitle;
      string fieldDataTtype;

      ExtractInfo(keySelector, out fieldName, out fieldTitle, out fieldDataTtype);

      DoAddField(
          action
        , fieldName
        , title ?? fieldTitle
        , fieldDataTtype
        , null
        , true
      );

      return action;
    }

    #endregion

    #region AddFieldsFrom<T>

    public static EntityAction AddFieldsFrom<T>(this EntityAction action)
    {
      foreach (var property in typeof(T).GetProperties())
      {
        var fieldName = Conventions.MakeName(property);
        var fieldTitle = Conventions.MakeTitle(property);
        var fieldDataType = Conventions.MakeDataType(property);

        DoAddField(
            action
          , fieldName
          , fieldTitle
          , fieldDataType
          , null
        , false
        );
      }
      return action;
    }

    public static EntityAction AddFieldsMultiFrom<T>(this EntityAction action)
    {
      foreach (var property in typeof(T).GetProperties())
      {
        var fieldName = Conventions.MakeName(property);
        var fieldTitle = Conventions.MakeTitle(property);
        var fieldDataType = Conventions.MakeDataType(property);

        DoAddField(
            action
          , fieldName
          , fieldTitle
          , fieldDataType
          , null
        , true
        );
      }
      return action;
    }

    #endregion

    #region AddField

    public static EntityAction AddField(
        this EntityAction action
      , string name
      , string dataType
      , string title = null
      )
    {
      DoAddField(
          action
        , name
        , title
        , dataType
        , null
        , false
      );
      return action;
    }

    public static EntityAction AddField(
        this EntityAction action
      , string name
      , DataType dataType
      , string title = null
      )
    {
      DoAddField(
          action
        , name
        , title
        , dataType.GetName()
        , null
        , false
      );
      return action;
    }

    public static EntityAction AddField(
        this EntityAction action
      , string name
      , Action<Field> builder
      )
    {
      DoAddField(
          action
        , name
        , null
        , null
        , builder
        , false
      );
      return action;
    }

    public static EntityAction AddFieldMulti(
        this EntityAction action
      , string name
      , string dataType
      , string title = null
      )
    {
      DoAddField(
          action
        , name
        , title
        , dataType
        , null
        , true
      );
      return action;
    }

    public static EntityAction AddFieldMulti(
        this EntityAction action
      , string name
      , DataType dataType
      , string title = null
      )
    {
      DoAddField(
          action
        , name
        , title
        , dataType.GetName()
        , null
        , true
      );
      return action;
    }

    public static EntityAction AddFieldMulti(
        this EntityAction action
      , string name
      , Action<Field> builder
      )
    {
      DoAddField(
          action
        , name
        , null
        , null
        , builder
        , true
      );
      return action;
    }

    #endregion

    #region

    public static Field GetField(this EntityAction action, string fieldName)
    {
      if (string.IsNullOrEmpty(fieldName))
        return null;

      var field = action.Fields?.FirstOrDefault(
        x => x.Name.EqualsIgnoreCase(fieldName)
      );
      return field;
    }

    public static Field GetField(this FieldCollection fields, string fieldName)
    {
      if (string.IsNullOrEmpty(fieldName))
        return null;

      var field = fields.FirstOrDefault(
        x => x.Name.EqualsIgnoreCase(fieldName)
      );
      return field;
    }

    #endregion

    #region Algoritmos

    private static void DoAddField(
        EntityAction action
      , string name
      , string title
      , string dataType
      , Action<Field> builder
      , bool allowMulti
      )
    {
      if (action.Fields == null)
      {
        action.Fields = new FieldCollection();
      }

      var field = GetOrAddField(action.Fields, name);

      field.AddTitle(title ?? name.ChangeCase(TextCase.ProperCase));
      field.AddDataType(dataType ?? DataTypeNames.Text);

      if (builder != null)
      {
        builder.Invoke(field);
      }
      else
      {
        // Aplicando padroes extendidos
        if (allowMulti)
        {
          switch (field.DataType)
          {
            case DataTypeNames.Bit:
              break;

            case DataTypeNames.Date:
            case DataTypeNames.Datetime:
            case DataTypeNames.Time:
            case DataTypeNames.Decimal:
            case DataTypeNames.Number:
              field.AddAllowMany();
              field.AddAllowRange();
              break;

            case DataTypeNames.Text:
              field.AddAllowMany();
              field.AddAllowWildcards();
              break;
          }
        }
      }
    }

    /// <summary>
    /// Obtém o campo com o nome indicado ou cria um novo.
    /// O campo criado é automaticamente inserido na lista de campos conhecidos.
    /// </summary>
    /// <param name="fields">A lista de campos conhecidos.</param>
    /// <param name="fieldName">O nome do campo.</param>
    /// <returns>O campo obtido ou criado.</returns>
    private static Field GetOrAddField(FieldCollection fields, string fieldName)
    {
      if (string.IsNullOrEmpty(fieldName))
      {
        fieldName = MakeFieldName(fields);
      }

      var field = fields.FirstOrDefault(x => x.Name.EqualsIgnoreCase(fieldName));
      if (field == null)
      {
        field = new Field
        {
          Name = fieldName,
          Title = fieldName.Replace("Field", "Campo").ChangeCase(TextCase.ProperCase)
        };
        fields.Add(field);
      }

      return field;
    }

    /// <summary>
    /// Cria um nome único para um campo.
    /// </summary>
    /// <param name="fields">A coleção dos campos conhecidos.</param>
    /// <returns>O nome único para o campo</returns>
    private static string MakeFieldName(FieldCollection fields)
    {
      string fieldName = null;

      var index = fields.Count + 1;
      do
      {
        fieldName = $"Field{index}";
      } while (fields.Any(x => x.Name.EqualsIgnoreCase(fieldName)));

      return fieldName;
    }

    private static void ExtractInfo<T>(
        Expression<Func<T, object>> keySelector
      , out string fieldName
      , out string fieldTitle
      , out string fieldDataType)
    {
      var expression = Expressions.FindMemberExpression(keySelector);
      if (expression == null)
        throw new Exception("A expressão não é válida. Apenas um seletor de propriedade de objeto é suportado.");

      var memberType =
        (expression.Member as PropertyInfo)?.PropertyType
        ?? (expression.Member as FieldInfo)?.FieldType
        ?? typeof(string);

      fieldName = Conventions.MakeName(expression.Member.Name);
      fieldTitle = Conventions.MakeTitle(expression.Member.Name);
      fieldDataType = Conventions.MakeDataType(memberType);
    }

    #endregion
  }
}
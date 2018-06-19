using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Paper.Media;
using System.Collections.Specialized;
using Toolset;
using Paper.Media.Design;
using Toolset.Collections;
using System.Collections;
using Paper.Media.Design.Queries;
using Microsoft.Extensions.DependencyInjection;
using Paper.Media.Rendering.Utilities;
using System.Net;
using System.Reflection;
using Toolset.Reflection;
using System.ComponentModel;
using Paper.Media.Design.Widgets.Mapping;
using Toolset.Serialization;
using Toolset.Serialization.Json;

namespace Paper.Media.Rendering.Entities
{
  [PaperRenderer(ContractName)]
  public class FeatureRenderer : IPaperRenderer
  {
    public const string ContractName = "Feature";

    public Type PaperType { get; set; }

    public string PathTemplate { get; set; }

    private IServiceProvider injector;

    public FeatureRenderer(IServiceProvider injector)
    {
      this.injector = injector;
    }

    public Ret<Entity> RenderEntity(HttpContext httpContext, string path)
    {
      if (httpContext.Request.Method == "GET")
      {
        return RenderForm(httpContext);
      }
      else
      {
        return ExecuteAndRenderResult(httpContext);
      }
    }

    #region Utilitários de execução da ação

    public Ret<Entity> ExecuteAndRenderResult(HttpContext httpContext)
    {
      var action = (string)httpContext.Request.Query["action"];
      if (string.IsNullOrWhiteSpace(action))
      {
        var self = httpContext.Request.GetRequestUri();
        var entity = HttpEntity.Create(self, HttpStatusCode.BadRequest,
          $"Nenhuma ação foi indicada. Faltou definir o parâmetro \"action\"?"
        );
        return entity;
      }
      
      var method = PaperType.GetMethodIgnoreCase(action);
      if (method == null)
      {
        var self = httpContext.Request.GetRequestUri();
        var entity = HttpEntity.Create(self, HttpStatusCode.NotImplemented,
          $"A ação solicitada não está disponível: \"{action}\""
        );
        return entity;
      }

      return ExecuteMethod(httpContext, method);
    }

    private Ret<Entity> ExecuteMethod(HttpContext httpContext, MethodInfo method)
    {
      try
      {
        var target = injector.CreateInstance(PaperType);
        try
        {
          var ret = CreateParameters(httpContext, method);
          if (ret.IsFault())
            return Ret.Throw(ret);

          object[] parameters = ret.Data;
          method.Invoke(target, parameters);
        }
        finally
        {
          (target as IDisposable)?.Dispose();
        }

        return Ret.Ok();
      }
      catch (Exception ex)
      {
        return Ret.Fail(ex);
      }
    }

    private Ret<object[]> CreateParameters(HttpContext httpContext, MethodInfo method)
    {
      var args = ParseArgs(httpContext);

      var parameters = method.GetParameters().ToList();
      var parameterValues = new object[parameters.Count];

      for (int i = 0; i < parameters.Count; i++)
      {
        var parameter = parameters[i];
        if (!IsSimpleValue(parameter.ParameterType))
        {
          parameterValues[i] = injector.CreateInstance(parameter.ParameterType);
        }
      }

      foreach (var arg in args)
      {
        var tokens = arg.Key.Split('.');
        var argName = tokens.First();
        var argProperty = tokens.Skip(1).FirstOrDefault();

        var parameter = parameters.FirstOrDefault(x => x.Name.EqualsIgnoreCase(argName));
        if (parameter == null)
          return Ret.Fail(HttpStatusCode.BadRequest, $"Parâmetro não esperado: {arg.Value}");

        var parameterIndex = parameters.IndexOf(parameter);

        if (argProperty == null)
        {
          var value = Cast.To(arg.Value, parameter.ParameterType);
          parameterValues[parameterIndex] = value;
        }
        else
        {
          var target = parameterValues[parameterIndex];
          target.Set(argProperty, arg.Value);
        }
      }

      return parameterValues;
    }

    private HashMap ParseArgs(HttpContext httpContext)
    {
      DocumentModel document = null;
      using (var stream = httpContext.Request.Body)
      {
        var settings = new SerializationSettings
        {
          IsFragment = true,
          IsLenient = true,
          TextCase = TextCase.KeepOriginal
        };
        var documentWriter = new DocumentWriter(settings);
        var jsonReader = new JsonReader(stream, settings);
        jsonReader.CopyTo(documentWriter);
        document = documentWriter.TargetDocument;
      }

      var map = new HashMap();
      foreach (var property in document.Root.ChildProperties())
      {
        map[property.Name] = property.Value.SerializationValue;
      }
      return map;
    }

    #endregion

    #region Utilitários de renderização do form

    private Ret<Entity> RenderForm(HttpContext httpContext)
    {
      var entity = new Entity();
      var feature = injector.CreateInstance(PaperType);
      try
      {
        RenderForm(httpContext, feature, entity);
      }
      finally
      {
        (feature as IDisposable)?.Dispose();
      }
      return entity;
    }

    private Entity RenderForm(HttpContext httpContext, object feature, Entity entity)
    {
      var type = feature.GetType();

      var req = httpContext.Request;

      var actions = RenderActions(httpContext, feature).ToArray();
      var entitites =
        from action in actions
        select new Entity
        {
          Class = KnownClasses.Row,
          Rel = KnownRelations.Row,
          Title = action.Title,
          Properties = PropertyCollection.Create(new
          {
            action.Title,
            Description = $"Ação {action.Title} disponível para {entity.Title}"
          }),
          Actions = new EntityActionCollection(action)
        };

      var self = httpContext.Request.GetRequestUri();

      entity.Class = $"{KnownClasses.Rows}, feature";
      entity.Title = GetName(type).ChangeCase(TextCase.ProperCase);
      entity.Links = new LinkCollection().AddSelf(self);
      entity.Properties = PropertyCollection.Create(new
      {
        entity.Title,
        Description = $"Ações disponíveis para {entity.Title}."
      });
      entity.Entities = new EntityCollection(entitites);
      entity.Actions = new EntityActionCollection(actions);

      return entity;
    }

    #region Utilitários de renderização compartilhados

    public static IEnumerable<EntityAction> RenderActions(HttpContext httpContext, object feature)
    {
      var type = feature.GetType();
      var methods =
        from method in type.GetMethods()
        where method.GetCustomAttributes(true).OfType<ExposeAttribute>().Any()
           && !method.GetCustomAttributes(true).OfType<IgnoreAttribute>().Any()
        select method;

      foreach (var method in methods)
      {
        var action = RenderAction(httpContext, feature, method);
        if (action != null)
          yield return action;
      }
    }

    public static EntityAction RenderAction(HttpContext httpContext, object feature, string methodName)
    {
      var type = feature.GetType();
      var methods =
        from method in type.GetMethods()
        where method.Name.EqualsIgnoreCase(methodName)
           && !method.GetCustomAttributes(true).OfType<IgnoreAttribute>().Any()
        select method;

      var methodFound = methods.FirstOrDefault();
      if (methodFound == null)
        return null;

      return RenderAction(httpContext, feature, methodFound);
    }

    public static EntityAction RenderAction(HttpContext httpContext, object feature, MethodInfo method)
    {
      Route self = httpContext.Request.GetRequestUri();

      var action = new EntityAction();
      action.Name = GetName(method);
      action.Title = action.Name.ChangeCase(TextCase.ProperCase);
      action.Method = "POST";
      action.Href = self.SetArg("action", action.Name.ChangeCase(TextCase.CamelCase));
      action.Fields = new FieldCollection();

      var parameters = method.GetParameters();
      foreach (var parameter in parameters)
      {
        var fields = RenderFields(httpContext, feature, method, parameter);
        action.Fields.AddRange(fields);
      }

      return action;
    }

    #endregion

    #endregion

    #region Utilitários

    private static IEnumerable<Field> RenderFields(HttpContext httpContext, object feature, MethodInfo method, ParameterInfo parameter)
    {
      var type = parameter.ParameterType;
      if (IsSimpleValue(type))
      {
        var field = new Field();
        field.Name = GetName(parameter);
        field.Title = field.Name.ChangeCase(TextCase.ProperCase);
        field.DataType = KnownFieldDataTypes.GetDataTypeName(type);
        field.Type = KnownFieldTypes.GetTypeName(field.DataType);
        yield return field;
      }
      else
      {
        var prefix = GetName(parameter);
        var category = prefix.ChangeCase(TextCase.ProperCase);

        var properties = type.GetProperties();
        foreach (var property in properties)
        {
          var suffix = GetName(property);

          var field = new Field();
          field.Category = category;
          field.Name = $"{prefix}.{suffix}";
          field.Title = suffix.ChangeCase(TextCase.ProperCase);
          field.DataType = KnownFieldDataTypes.GetDataTypeName(property.PropertyType);
          field.Type = KnownFieldTypes.GetTypeName(field.DataType);

          var widget =
            type.GetCustomAttributes(true).OfType<WidgetAttribute>().FirstOrDefault();
          if (widget != null)
          {
            field.CopyFrom(widget);
            field.Properties = new FieldProperties();
            field.Properties.CopyFrom(widget);

            // TODO: Implementar HAS OPTIONS de alguma forma
            //
            //// Verificando se o widget da suporte à interface IHasOptions<TOption>
            //var options = field.Get<Option[]>("Options");
            //if (options != null)
            //{
            //  var values = field.Get<object[]>("Value");
            //  if (values == null)
            //  {
            //    var value = field.Get("Value");
            //    values = value.AsSingle().NonNull().ToArray();
            //  }

            //  var fieldValues = new FieldValueCollection();
            //  foreach (var option in options)
            //  {
            //    var fieldValue = new FieldValue();
            //    fieldValue.CopyFrom(options);
            //    fieldValue.Selected = values.Contains(option.Value);
            //  }
            //  field.Value = fieldValues;
            //}
          }

          yield return field;
        }
      }
    }

    private static bool IsSimpleValue(Type type)
    {
      return type.IsValueType || type == typeof(string);
    }

    private static string GetName(Type type)
    {
      var displayNameAttribute =
        type
          .GetCustomAttributes(true)
          .OfType<DisplayNameAttribute>()
          .FirstOrDefault();
      var name = displayNameAttribute?.DisplayName ?? type.Name;
      return name.ChangeCase(TextCase.PascalCase);
    }

    private static string GetName(MethodInfo method)
    {
      var displayNameAttribute =
        method
          .GetCustomAttributes(true)
          .OfType<DisplayNameAttribute>()
          .FirstOrDefault();
      var name = displayNameAttribute?.DisplayName ?? method.Name;
      return name.ChangeCase(TextCase.PascalCase);
    }

    private static string GetName(ParameterInfo arg)
    {
      var displayNameAttribute =
        arg
          .GetCustomAttributes(true)
          .OfType<DisplayNameAttribute>()
          .FirstOrDefault();
      var name = displayNameAttribute?.DisplayName ?? arg.Name;
      return name.ChangeCase(TextCase.PascalCase);
    }

    private static string GetName(PropertyInfo property)
    {
      var displayNameAttribute =
        property
          .GetCustomAttributes(true)
          .OfType<DisplayNameAttribute>()
          .FirstOrDefault();
      var name = displayNameAttribute?.DisplayName ?? property.Name;
      return name.ChangeCase(TextCase.PascalCase);
    }

    #endregion
  }
}

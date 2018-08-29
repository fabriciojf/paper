using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Toolset.Data;

namespace Toolset.Reflection
{
  public static class ObjectExtensions
  {
    private readonly static BindingFlags Flags =
      BindingFlags.Public | BindingFlags.Instance;

    private readonly static BindingFlags CaseInsensitiveFlags = 
      BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase;

    public static PropertyInfo _GetPropertyInfo(this object typeOrObject, string propertyName)
    {
      var type = (typeOrObject is Type) ? (Type)typeOrObject : typeOrObject.GetType();
      var property =
        type
          .GetProperties(Flags)
          .Where(x => x.Name.EqualsAnyIgnoreCase(propertyName))
          .FirstOrDefault();
      return property;
    }

    public static Type _GetPropertyType(this object typeOrObject, string propertyName)
    {
      return _GetPropertyInfo(typeOrObject, propertyName)?.PropertyType;
    }

    public static MethodInfo _GetMethodInfo(this object typeOrObject, string methodName, Type[] argTypes = null)
    {
      var type = (typeOrObject is Type) ? (Type)typeOrObject : typeOrObject.GetType();
      return LocateMethod(type, methodName, CaseInsensitiveFlags, argTypes, leniente: false);
    }

    private static MethodInfo LocateMethod(Type type, string methodName, BindingFlags flags, Type[] argTypes, bool leniente)
    {
      if (argTypes == null)
      {
        return type.GetMethod(methodName, CaseInsensitiveFlags);
      }
      else
      {
        // localizando o metodo que melhor suporta os parametros
        MethodInfo method = null;
        do
        {
          method = type.GetMethod(methodName, CaseInsensitiveFlags, null, argTypes, null);
          if (method != null || argTypes.Length == 0 || !leniente)
            break;

          argTypes = argTypes.Take(argTypes.Length - 1).ToArray();
        } while (true);

        return method;
      }
    }

    public static T _GetAttr<T>(this object typeOrObject)
      where T : Attribute
    {
      if (typeOrObject == null)
        return null;

      var member = typeOrObject as MemberInfo ?? typeOrObject.GetType();
      var attr = member.GetCustomAttributes(true).OfType<T>().FirstOrDefault();
      return attr;
    }

    public static IEnumerable<T> _GetAttrs<T>(this object typeOrObject)
      where T : Attribute
    {
      if (typeOrObject == null)
        return null;

      var member = typeOrObject as MemberInfo ?? typeOrObject.GetType();
      var attrs = member.GetCustomAttributes(true).OfType<T>();
      return attrs;
    }

    public static IEnumerable<string> _GetPropertyNames(this object typeOrObject)
    {
      var type = (typeOrObject is Type) ? (Type)typeOrObject : typeOrObject.GetType();
      return type.GetProperties().Select(x => x.Name);
    }

    public static IEnumerable<string> _GetMethodNames(this object typeOrObject)
    {
      var type = (typeOrObject is Type) ? (Type)typeOrObject : typeOrObject.GetType();
      return type.GetMethods().Select(x => x.Name);
    }

    public static bool _Has(this object target, string propertyOrMethodName)
    {
      return _HasProperty(target, propertyOrMethodName)
          || _HasMethod(target, propertyOrMethodName);
    }

    public static bool _Has(this object target, string propertyOrMethodName, Type propertyOrMethodType)
    {
      return _HasProperty(target, propertyOrMethodName, propertyOrMethodType)
          || _HasMethod(target, propertyOrMethodName, propertyOrMethodType);
    }

    public static bool _Has<T>(this object target, string propertyOrMethodName)
    {
      return _HasProperty<T>(target, propertyOrMethodName)
          || _HasMethod<T>(target, propertyOrMethodName);
    }

    public static bool _CanWrite(this object target, string propertyName)
    {
      var property = _GetPropertyInfo(target, propertyName);
      return property?.CanWrite == true;
    }

    public static bool _CanWrite(this object target, string propertyName, Type propertyType)
    {
      var property = _GetPropertyInfo(target, propertyName);
      return property?.CanWrite == true
          && propertyType.IsAssignableFrom(property.PropertyType);
    }

    public static bool _CanWrite<T>(this object target, string propertyName)
    {
      var property = _GetPropertyInfo(target, propertyName);
      return property?.CanWrite == true
          && typeof(T).IsAssignableFrom(property.PropertyType);
    }

    public static object _Get(this object target, string propertyName)
    {
      var property = _GetPropertyInfo(target, propertyName);
      return property?.GetValue(target);
    }

    public static T _Get<T>(this object target, string propertyName)
    {
      var property = _GetPropertyInfo(target, propertyName);
      var value = property?.GetValue(target);
      if (value == null)
        return default(T);

      if (typeof(T).IsAssignableFrom(property.PropertyType))
        return (T)value;

      try
      {
        var convertedValue = Change.To<T>(value);
        return convertedValue;
      }
      catch (FormatException ex)
      {
        throw new FormatException(
          $"Era esperado um valor para a propriedade {property.Name} compatível com \"{property.PropertyType.FullName}\" mas foi obtido: \"{value}\""
          , ex);
      }
    }

    public static void _Set(this object target, string propertyName, object value)
    {
      var property = _GetPropertyInfo(target, propertyName);
      if (property == null)
        throw new NullReferenceException($"A propriedade não existe: {target.GetType().FullName}.{propertyName}");
      if (!property.CanWrite)
        throw new NullReferenceException($"A propriedade é somente leitura: {target.GetType().FullName}.{propertyName}");

      if (value == null)
      {
        property.SetValue(target, null);
        return;
      }

      // Tratamento especial para o tipo Any do Toolset.
      if (typeof(IVar).IsAssignableFrom(property.PropertyType))
      {
        if (!(value is IVar))
        {
          value = Activator.CreateInstance(property.PropertyType, value);
        }
      }

      if (property.PropertyType.IsAssignableFrom(value.GetType()))
      {
        property.SetValue(target, value);
        return;
      }

      try
      {
        var convertedValue = Change.To(value, property.PropertyType);
        property.SetValue(target, convertedValue);
      }
      catch (FormatException ex)
      {
        throw new FormatException(
          $"Era esperado um valor para a propriedade {property.Name} compatível com \"{property.PropertyType.FullName}\" mas foi obtido: \"{value}\""
          , ex);
      }
    }

    public static object _SetNew(this object target, string propertyName, params object[] args)
    {
      var type = _GetPropertyInfo(target, propertyName)?.PropertyType;
      if (type == null)
        throw new NullReferenceException($"A propriedade não existe: {target.GetType().FullName}.{propertyName}");

      var value = Activator.CreateInstance(type, args);
      _Set(target, propertyName, value);

      return value;
    }

    public static T _SetNew<T>(this object target, string propertyName, params object[] args)
    {
      return (T)_SetNew(target, propertyName, args);
    }

    public static bool _TrySet(this object target, string propertyName, object value)
    {
      if (!_Has(target, propertyName))
        return false;

      try
      {
        _Set(target, propertyName, value);
        return true;
      }
      catch
      {
        return false;
      }
    }

    public static object _Call(this object target, string methodName, params object[] args)
    {
      var type = target.GetType();
      var argTypes = args.Select(x => x?.GetType() ?? typeof(object)).ToArray();
      var method = LocateMethod(type, methodName, CaseInsensitiveFlags, argTypes, leniente: true);
      if (method == null)
        return null;

      args = args.Take(method.GetParameters().Length).ToArray();
      var result = method.Invoke(target, args);
      return result;
    }

    public static TResult _Call<TResult>(this object target, string methodName, params object[] args)
    {
      var value = _Call(target, methodName, args);
      try
      {
        var convertedValue = Change.To<TResult>(value);
        return convertedValue;
      }
      catch (FormatException ex)
      {
        throw new FormatException(
          $"O resultado do método {methodName} não é compatível com o tipo esperado { typeof(TResult).FullName}: \"{value}\""
          , ex);
      }
    }

    public static bool _HasProperty(this object target, string propertyName)
    {
      var property = _GetPropertyInfo(target, propertyName);
      return property != null;
    }

    public static bool _HasProperty(this object target, string propertyName, Type propertyType)
    {
      var property = _GetPropertyInfo(target, propertyName);
      return property != null
          && propertyType.IsAssignableFrom(property.PropertyType);
    }

    public static bool _HasProperty<T>(this object target, string propertyName)
    {
      var property = _GetPropertyInfo(target, propertyName);
      return property != null
          && typeof(T).IsAssignableFrom(property.PropertyType);
    }

    public static bool _HasMethod(this object target, string methodName, params Type[] argTypes)
    {
      if (argTypes.Length == 0)
        argTypes = null;

      var type = target.GetType();
      var method = LocateMethod(type, methodName, CaseInsensitiveFlags, argTypes, leniente: true);
      return method != null;
    }

    public static bool _HasMethod<TReturn>(this object target, string methodName, params Type[] argTypes)
    {
      if (argTypes.Length == 0)
        argTypes = null;

      var type = target.GetType();
      var method = LocateMethod(type, methodName, CaseInsensitiveFlags, argTypes, leniente: true);
      return method != null && typeof(TReturn).IsAssignableFrom(method.ReturnType);
    }

    public static T _CopyFrom<T>(this T target, object source, CopyOptions options = CopyOptions.None)
    {
      if (target == null || source == null)
        return target;

      var ignoreNull = options.HasFlag(CopyOptions.IgnoreNull);

      foreach (var sourceProperty in source.GetType().GetProperties())
      {
        var targetProperty = _GetPropertyInfo(target, sourceProperty.Name);
        if (targetProperty != null)
        {
          object sourceValue = sourceProperty.GetValue(source);
          object targetValue = null;

          if (ignoreNull && sourceValue == null)
            continue;

          if (sourceValue != null)
          {
            targetValue = Change.To(sourceValue, targetProperty.PropertyType);
          }

          targetProperty.SetValue(target, targetValue);
        }
      }

      return target;
    }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Toolset.Reflection
{
  public static class ObjectExtensions
  {
    private readonly static BindingFlags CaseInsensitiveFlags = 
      BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance;

    private readonly static BindingFlags CaseSensitiveFlags =
      BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance;

    public static PropertyInfo GetProperty(this object typeOrObject, string propertyName)
    {
      var type = (typeOrObject is Type) ? (Type)typeOrObject : typeOrObject.GetType();
      return type.GetProperty(propertyName, CaseSensitiveFlags);
    }

    public static PropertyInfo GetPropertyIgnoreCase(this object typeOrObject, string propertyName)
    {
      var type = (typeOrObject is Type) ? (Type)typeOrObject : typeOrObject.GetType();
      return type.GetProperty(propertyName, CaseInsensitiveFlags);
    }

    public static MethodInfo GetMethod(this object typeOrObject, string methodName, Type[] argTypes = null)
    {
      var type = (typeOrObject is Type) ? (Type)typeOrObject : typeOrObject.GetType();
      return LocateMethod(type, methodName, CaseSensitiveFlags, argTypes, leniente: false);
    }

    public static MethodInfo GetMethodIgnoreCase(this object typeOrObject, string methodName, Type[] argTypes = null)
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

    public static bool Has(this object target, string propertyName)
    {
      var property = GetPropertyIgnoreCase(target, propertyName);
      return property != null;
    }

    public static bool Has(this object target, string propertyName, Type propertyType)
    {
      var property = GetPropertyIgnoreCase(target, propertyName);
      return property != null
          && propertyType.IsAssignableFrom(property.PropertyType);
    }

    public static bool Has<T>(this object target, string propertyName)
    {
      var property = GetPropertyIgnoreCase(target, propertyName);
      return property != null
          && typeof(T).IsAssignableFrom(property.PropertyType);
    }

    public static bool IsWritable(this object target, string propertyName)
    {
      var property = GetPropertyIgnoreCase(target, propertyName);
      return property?.CanWrite == true;
    }

    public static bool IsWritable(this object target, string propertyName, Type propertyType)
    {
      var property = GetPropertyIgnoreCase(target, propertyName);
      return property?.CanWrite == true
          && propertyType.IsAssignableFrom(property.PropertyType);
    }

    public static bool IsWritable<T>(this object target, string propertyName)
    {
      var property = GetPropertyIgnoreCase(target, propertyName);
      return property?.CanWrite == true
          && typeof(T).IsAssignableFrom(property.PropertyType);
    }

    public static object Get(this object target, string propertyName)
    {
      var property = GetPropertyIgnoreCase(target, propertyName);
      return property?.GetValue(target);
    }

    public static T Get<T>(this object target, string propertyName)
    {
      var property = GetPropertyIgnoreCase(target, propertyName);
      var value = property?.GetValue(target);
      if (value == null)
        return default(T);

      if (typeof(T).IsAssignableFrom(property.PropertyType))
        return (T)value;

      try
      {
        var convertedValue = Cast.To<T>(value);
        return convertedValue;
      }
      catch (FormatException ex)
      {
        throw new FormatException(
          $"Era esperado um valor para a propriedade {property.Name} compatível com \"{property.PropertyType.FullName}\" mas foi obtido: \"{value}\""
          , ex);
      }
    }

    public static void Set(this object target, string propertyName, object value)
    {
      var property = GetPropertyIgnoreCase(target, propertyName);
      if (property == null)
        throw new NullReferenceException($"A propriedade não existe: {target.GetType().FullName}.{propertyName}");
      if (!property.CanWrite)
        throw new NullReferenceException($"A propriedade é somente leitura: {target.GetType().FullName}.{propertyName}");

      if (value == null)
      {
        property.SetValue(target, null);
        return;
      }

      if (property.PropertyType.IsAssignableFrom(value.GetType()))
      {
        property.SetValue(target, value);
        return;
      }

      try
      {
        var convertedValue = Cast.To(value, property.PropertyType);
        property.SetValue(target, convertedValue);
      }
      catch (FormatException ex)
      {
        throw new FormatException(
          $"Era esperado um valor para a propriedade {property.Name} compatível com \"{property.PropertyType.FullName}\" mas foi obtido: \"{value}\""
          , ex);
      }
    }

    public static object SetNew(this object target, string propertyName, params object[] args)
    {
      var type = GetPropertyIgnoreCase(target, propertyName)?.PropertyType;
      if (type == null)
        throw new NullReferenceException($"A propriedade não existe: {target.GetType().FullName}.{propertyName}");

      var value = Activator.CreateInstance(type, args);
      Set(target, propertyName, value);

      return value;
    }

    public static T SetNew<T>(this object target, string propertyName, params object[] args)
    {
      return (T)SetNew(target, propertyName, args);
    }

    public static bool HasMethod(this object target, string methodName, params Type[] argTypes)
    {
      if (argTypes.Length == 0)
        argTypes = null;

      var type = target.GetType();
      var method = LocateMethod(type, methodName, CaseInsensitiveFlags, argTypes, leniente: true);
      return method != null;
    }

    public static bool HasMethod<TReturn>(this object target, string methodName, params Type[] argTypes)
    {
      if (argTypes.Length == 0)
        argTypes = null;

      var type = target.GetType();
      var method = LocateMethod(type, methodName, CaseInsensitiveFlags, argTypes, leniente: true);
      return method != null && typeof(TReturn).IsAssignableFrom(method.ReturnType);
    }

    public static object Call(this object target, string methodName, params object[] args)
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

    public static TResult Call<TResult>(this object target, string methodName, params object[] args)
    {
      var value = Call(target, methodName, args);
      try
      {
        var convertedValue = Cast.To<TResult>(value);
        return convertedValue;
      }
      catch (FormatException ex)
      {
        throw new FormatException(
          $"O resultado do método {methodName} não é compatível com o tipo esperado { typeof(TResult).FullName}: \"{value}\""
          , ex);
      }
    }

    public static T CopyFrom<T>(this T target, object source, CopyOptions options = CopyOptions.None)
    {
      if (target == null || source == null)
        return target;

      var ignoreNull = options.HasFlag(CopyOptions.IgnoreNull);

      foreach (var sourceProperty in source.GetType().GetProperties())
      {
        var targetProperty = GetPropertyIgnoreCase(target, sourceProperty.Name);
        if (targetProperty != null)
        {
          object sourceValue = sourceProperty.GetValue(source);
          object targetValue = null;

          if (ignoreNull && sourceValue == null)
            continue;

          if (sourceValue != null)
          {
            targetValue = Cast.To(sourceValue, targetProperty.PropertyType);
          }

          targetProperty.SetValue(target, targetValue);
        }
      }

      return target;
    }
  }
}

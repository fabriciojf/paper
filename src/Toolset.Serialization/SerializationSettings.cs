using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Toolset.Serialization;
using System.Reflection;
using System.Globalization;

namespace Toolset.Serialization
{
  public class SerializationSettings : ICloneable
  {
    private readonly Dictionary<Type, object> instances;

    public SerializationSettings()
    {
      this.instances = new Dictionary<Type, object>();
      this.TextCase = TextCase.Default;
    }

    public SerializationSettings(SerializationSettings settings)
    {
      this.instances = new Dictionary<Type, object>();
      this.TextCase = TextCase.Default;
      
      this.PropertyResolver = settings?.PropertyResolver;
    }

    private IPropertyResolver PropertyResolver
    {
      get { return propertyResolver ?? (propertyResolver = new DictionaryPropertyResolver()); }
      set { propertyResolver = value; }
    }
    private IPropertyResolver propertyResolver;

    public bool HasProperty(string property)
    {
      return PropertyResolver.HasProperty(property);
    }

    public T Get<T>(string property)
    {
      return Get<T>(property, default(T));
    }

    public T Get<T>(string property, T defaultValue)
    {
      var value = PropertyResolver.Get(property) ?? defaultValue;
      return (value is T) ? (T)value : defaultValue;
    }

    public void Set<T>(string property, T value)
    {
      PropertyResolver.Set(property, value);
    }

    public void Reset<T>(string property)
    {
      PropertyResolver.Delete(property);
    }

    public virtual TextCase TextCase
    {
      get { return Get<TextCase>("TextCase"); }
      set { Set("TextCase", value); }
    }

    /// <summary>
    /// Por padrão, este deveria ser o formato do arquivo, quando JSON:
    ///    {
    ///      "nomeDoTipo": {
    ///        "prop1": valor1,
    ///        "prop2": valor2,
    ///        ...
    ///      }
    ///    }
    /// Mas, podemose também receber um documento simples, sem o nome do tipo e
    /// com apenas as suas propriedades:
    ///    {
    ///      "prop1": valor1,
    ///      "prop2": valor2,
    ///      ...
    ///    }
    /// Para o Toolset.Serialization isto é um Fragmento de documento.
    /// </summary>
    public virtual bool IsFragment
    {
      get { return Get<bool>("IsFragment"); }
      set { Set("IsFragment", value); }
    }

    /// <summary>
    /// Por padrão todas as propriedades recebidas devem ter um corresponde no objeto.
    /// Porém, é comum recebermos mais propriedades que o esperado, para algum tipo de
    /// processamento intermediário.
    /// É preciso ativar o comportamento leniente para o Toolset.Serialization ignorar
    /// tais propriedades.
    /// </summary>
    public virtual bool IsLenient
    {
      get { return Get<bool>("IsLenient"); }
      set { Set("IsLenient", value); }
    }

    public virtual CultureInfo CultureInfo
    {
      get { return Get<CultureInfo>("CultureInfo", CultureInfo.InvariantCulture); }
      set { Set("CultureInfo", value); }
    }

    public virtual string DateTimeFormat
    {
      get { return Get<string>("DateTimeFormat", "yyyy-MM-ddTHH:mm:ssK"); }
      set { Set("DateTimeFormat", value); }
    }

    public T As<T>()
      where T : SerializationSettings, new()
    {
      if (this is T)
        return (T)this;

      var type = typeof(T);
      if (instances.ContainsKey(type))
        return (T)instances[type];

      var instance = new T { PropertyResolver = PropertyResolver };
      instances[type] = instance;
      return instance;
    }

    object ICloneable.Clone()
    {
      return this.Clone();
    }

    public SerializationSettings Clone()
    {
      var clonedProperties = new Dictionary<string, object>();
      foreach (var property in PropertyResolver.GetProperties())
      {
        clonedProperties[property] = PropertyResolver.Get(property);
      }
      var clonedPropertyResolver = new DictionaryPropertyResolver(clonedProperties);
      var clone = new SerializationSettings { PropertyResolver = clonedPropertyResolver };
      return clone;
    }

    public SerializationSettings CreateScope()
    {
      var scopedPropertyResolver = new ScopedPropertyResolver(PropertyResolver);
      var scope = new SerializationSettings { PropertyResolver = scopedPropertyResolver };
      return scope;
    }

    #region Classes de gerenciamento de dicionários de propriedades ...

    protected interface IPropertyResolver
    {
      IEnumerable<string> GetProperties();
      bool HasProperty(string property);
      object Get(string property);
      void Set(string property, object value);
      void Delete(string property);
    }

    private class DictionaryPropertyResolver : IPropertyResolver
    {
      private readonly Dictionary<string, object> dictionary;

      public DictionaryPropertyResolver()
      {
        this.dictionary = new Dictionary<string, object>();
      }

      public DictionaryPropertyResolver(Dictionary<string, object> dictionary)
      {
        this.dictionary = dictionary;
      }

      public IEnumerable<string> GetProperties()
      {
        return dictionary.Keys;
      }

      public bool HasProperty(string property)
      {
        return dictionary.ContainsKey(property);
      }

      public object Get(string property)
      {
        return dictionary.ContainsKey(property) ? dictionary[property] : null;
      }

      public void Set(string property, object value)
      {
        dictionary[property] = value;
      }

      public void Delete(string property)
      {
        if (dictionary.ContainsKey(property))
        {
          dictionary.Remove(property);
        }
      }
    }

    private class ScopedPropertyResolver : IPropertyResolver
    {
      private readonly IPropertyResolver resolver;
      private readonly IPropertyResolver fallback;

      public ScopedPropertyResolver(IPropertyResolver fallback)
      {
        this.resolver = new DictionaryPropertyResolver(new Dictionary<string, object>());
        this.fallback = fallback;
      }

      public IEnumerable<string> GetProperties()
      {
        return resolver.GetProperties().Union(fallback.GetProperties()).Distinct();
      }

      public bool HasProperty(string property)
      {
        return resolver.HasProperty(property) || fallback.HasProperty(property);
      }

      public object Get(string property)
      {
        return resolver.HasProperty(property) ? resolver.Get(property) : fallback.Get(property);
      }

      public void Set(string property, object value)
      {
        resolver.Set(property, value);
      }

      public void Delete(string property)
      {
        resolver.Delete(property);
      }
    }

    #endregion
  }
}
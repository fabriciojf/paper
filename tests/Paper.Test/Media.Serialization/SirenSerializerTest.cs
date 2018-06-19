using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Paper.Media;
using Paper.Media.Serialization;
using Xunit;

namespace Paper.Test.Media.Serialization
{
  public class SirenSerializerTest
  {
    #region Serialize

    [Theory]
    [InlineData(typeof(SirenSerializer))]
    [InlineData(typeof(MediaSerializer))]
    public void Serialize_EmptyEntity_Test(Type serializerType)
    {
      // Given
      var entity = new Entity();

      // When
      var serializer = (ISerializer)Activator.CreateInstance(serializerType);
      var siren = serializer.Serialize(entity);

      // Then
      var expected = "{}";
      var obtained = siren;
      Assert.Equal(expected, obtained);
    }

    [Theory]
    [InlineData(typeof(SirenSerializer))]
    [InlineData(typeof(MediaSerializer))]
    public void Serialize_EmptyCollections_Test(Type serializerType)
    {
      // Given
      var entity = new Entity();
      entity.Class = new NameCollection();
      entity.Rel = new NameCollection();
      entity.Properties = new PropertyCollection();
      entity.Links = new LinkCollection();
      entity.Entities = new EntityCollection();
      entity.Actions = new EntityActionCollection();

      // When
      var serializer = (ISerializer)Activator.CreateInstance(serializerType);
      var siren = serializer.Serialize(entity);

      // Then
      var expected = "{}";
      var obtained = siren;
      Assert.Equal(expected, obtained);
    }

    [Theory]
    [InlineData(typeof(SirenSerializer))]
    [InlineData(typeof(MediaSerializer))]
    public void Serialize_EntityWithProperties_Test(Type serializerType)
    {
      // Given
      var entity = new Entity();
      entity.Class = "single,Other.Class";
      entity.Title = "Title";
      entity.Links = new LinkCollection();
      entity.Links.AddSelf("http://host.com");
      entity.Properties = new PropertyCollection();
      entity.Properties.Add("prop1", "value1");
      entity.Properties.Add("prop1", "value1");
      entity.Properties.Add("prop2", "value2");

      // When
      var serializer = (ISerializer)Activator.CreateInstance(serializerType);
      var siren = serializer.Serialize(entity);

      // Then
      var expected =
        "{" +
          "\"class\":[\"single\",\"Other.Class\"]," +
          "\"title\":\"Title\"," +
          "\"properties\":{" +
            "\"prop1\":\"value1\"," +
            "\"prop2\":\"value2\"" +
          "}," +
          "\"links\":[" +
            "{" +
              "\"rel\":[\"self\"]," +
              "\"href\":\"http://host.com\"" +
            "}" +
          "]" +
        "}";
      var obtained = siren;
      Assert.Equal(expected, obtained);
    }

    [Theory]
    [InlineData(typeof(SirenSerializer))]
    [InlineData(typeof(MediaSerializer))]
    public void Serialize_EntityWithData_Test(Type serializerType)
    {
      // Given
      var graph = new
      {
        Id = 10,
        Name = "Tenth"
      };

      var entity = new Entity();
      entity.Properties = new PropertyCollection();
      entity.Properties.AddFromGraph(graph);
      entity.Properties.AddDataHeadersFromGraph(graph);

      // When
      var serializer = (ISerializer)Activator.CreateInstance(serializerType);
      var siren = serializer.Serialize(entity);

      // Then
      var expected =
        "{" +
          "\"properties\":{" +
            "\"id\":10," +
            "\"name\":\"Tenth\"," +
            "\"_dataHeaders\":[" +
              "{" +
                "\"name\":\"id\"," +
                "\"title\":\"Id\"," +
                "\"type\":\"int\"" +
              "}," +
              "{" +
                "\"name\":\"name\"," +
                "\"title\":\"Name\"," +
                "\"type\":\"string\"" +
              "}" +
            "]" +
          "}" +
        "}";
      var obtained = siren;
      Assert.Equal(expected, obtained);
    }

    [Theory]
    [InlineData(typeof(SirenSerializer))]
    [InlineData(typeof(MediaSerializer))]
    public void Serialize_EntityWithRows_Test(Type serializerType)
    {
      // Given
      var rows = new[] {
        new {
          Id = 10,
          Name = "Tenth"
        },
        new {
          Id = 20,
          Name = "Twentieth"
        }
      };

      var entity = new Entity();
      entity.Properties = new PropertyCollection();
      entity.Properties.AddRowsHeadersFromGraph(rows.First());
      entity.Entities = new EntityCollection();
      for (int i = 0; i < rows.Length; i++)
      {
        var graph = rows[i];
        entity.Entities.Add(new Entity());
        entity.Entities[i].Class = "row,Other.Class";
        entity.Entities[i].Rel = "row,Other.Rel";
        entity.Entities[i].Title = "Row";
        entity.Entities[i].Links = new LinkCollection();
        entity.Entities[i].Links.AddSelf($"http://host.com/{i}");
        entity.Entities[i].Properties = new PropertyCollection();
        entity.Entities[i].Properties.AddFromGraph(graph);
      }

      // When
      var serializer = (ISerializer)Activator.CreateInstance(serializerType);
      var siren = serializer.Serialize(entity);

      // Then
      var expected =
        "{" +
          "\"properties\":{" +
            "\"_rowsHeaders\":[" +
              "{" +
                "\"name\":\"id\"," +
                "\"title\":\"Id\"," +
                "\"type\":\"int\"" +
              "}," +
              "{" +
                "\"name\":\"name\"," +
                "\"title\":\"Name\"," +
                "\"type\":\"string\"" +
              "}" +
            "]" +
          "}," +
          "\"entities\":[" +
            "{" +
              "\"class\":[\"row\",\"Other.Class\"]," +
              "\"title\":\"Row\"," +
              "\"rel\":[\"row\",\"Other.Rel\"]," +
              "\"properties\":{" +
                "\"id\":10," +
                "\"name\":\"Tenth\"" +
              "}," +
              "\"links\":[" +
                "{" +
                  "\"rel\":[\"self\"]," +
                  "\"href\":\"http://host.com/0\"" +
                "}" +
              "]" +
            "}," +
            "{" +
              "\"class\":[\"row\",\"Other.Class\"]," +
              "\"title\":\"Row\"," +
              "\"rel\":[\"row\",\"Other.Rel\"]," +
              "\"properties\":{" +
                "\"id\":20," +
                "\"name\":\"Twentieth\"" +
              "}," +
              "\"links\":[" +
                "{" +
                  "\"rel\":[\"self\"]," +
                  "\"href\":\"http://host.com/1\"" +
                "}" +
              "]" +
            "}" +
          "]" +
        "}";
      var obtained = siren;
      Assert.Equal(expected, obtained);
    }

    [Theory]
    [InlineData(typeof(SirenSerializer))]
    [InlineData(typeof(MediaSerializer))]
    public void Serialize_EntityWithActions_Test(Type serializerType)
    {
      // Given
      var entity = new Entity();
      entity.Actions = new EntityActionCollection();
      entity.Actions.Add(new EntityAction());
      entity.Actions[0].Name = "Action";
      entity.Actions[0].Title = "Some Action";
      entity.Actions[0].Method = "PUT";
      entity.Actions[0].Href = "http://host.com/";
      entity.Actions[0].Type = "text/json";
      entity.Actions[0].Fields = new FieldCollection();
      entity.Actions[0].Fields.Add(new Field());
      entity.Actions[0].Fields[0].Name = "Field1";
      entity.Actions[0].Fields[0].Class = "field,Other.Class";
      entity.Actions[0].Fields[0].Title = "Field 1";
      entity.Actions[0].Fields[0].Category = "Fields";
      entity.Actions[0].Fields[0].Type = "text";
      entity.Actions[0].Fields[0].DataType = "string";
      entity.Actions[0].Fields[0].ReadOnly = true;
      entity.Actions[0].Fields[0].Required = true;
      entity.Actions[0].Fields[0].Value = "Content";
      entity.Actions[0].Fields[0].Properties = new FieldProperties();
      entity.Actions[0].Fields[0].Properties.AllowMany = true;
      entity.Actions[0].Fields[0].Properties.AllowRange = true;
      entity.Actions[0].Fields[0].Properties.AllowWildcards = true;
      entity.Actions[0].Fields[0].Properties.MaxLength = 17;
      entity.Actions[0].Fields[0].Properties.MinLength = 7;
      entity.Actions[0].Fields[0].Properties.Multiline = true;
      entity.Actions[0].Fields[0].Properties.Pattern = ".*";

      // When
      var serializer = (ISerializer)Activator.CreateInstance(serializerType);
      var siren = serializer.Serialize(entity);

      // Then
      var expected =
        "{" +
          "\"actions\":[" +
            "{" +
              "\"name\":\"Action\"," +
              "\"title\":\"Some Action\"," +
              "\"method\":\"PUT\"," +
              "\"href\":\"http://host.com/\"," +
              "\"type\":\"text/json\"," +
              "\"fields\":[" +
                "{" +
                  "\"class\":[\"field\",\"Other.Class\"]," +
                  "\"name\":\"field1\"," +
                  "\"type\":\"text\"," +
                  "\"dataType\":\"string\"," +
                  "\"title\":\"Field 1\"," +
                  "\"category\":\"Fields\"," +
                  "\"value\":\"Content\"," +
                  "\"required\":1," +
                  "\"readOnly\":1," +
                  "\"properties\":{" +
                    "\"minLength\":7," +
                    "\"maxLength\":17," +
                    "\"pattern\":\".*\"," +
                    "\"multiline\":1," +
                    "\"allowMany\":1," +
                    "\"allowRange\":1," +
                    "\"allowWildcards\":1" +
                  "}" +
                "}" +
              "]" +
            "}" +
          "]" +
        "}";
      var obtained = siren;
      Assert.Equal(expected, obtained);
    }

    #endregion

    #region Deserialize

    [Theory]
    [InlineData(typeof(SirenSerializer))]
    [InlineData(typeof(MediaSerializer))]
    public void Deserialize_EmptyEntity_Test(Type serializerType)
    {
      // Given
      var siren = "{}";

      // When
      var serializer = (ISerializer)Activator.CreateInstance(serializerType);
      var entity = serializer.Deserialize(siren);

      // Then
      Assert.NotNull(entity);
    }

    [Theory]
    [InlineData(typeof(SirenSerializer))]
    [InlineData(typeof(MediaSerializer))]
    public void Deserialize_HasInfo_Test(Type serializerType)
    {
      // Given
      var siren =
        "{" +
          "\"class\":[\"class\",\"Other.Class\"]," +
          "\"rel\":[\"rel\",\"Other.Rel\"]," +
          "\"title\":\"Title\"" +
        "}";

      // When
      var serializer = (ISerializer)Activator.CreateInstance(serializerType);
      var entity = serializer.Deserialize(siren);

      // Then
      var expected = new[] {
        "class", "Other.Class",
        "rel", "Other.Rel",
        "Title"
      };
      var obtained = new[]{
        entity?.Class?.ElementAtOrDefault(0), entity?.Class?.ElementAtOrDefault(1),
        entity?.Rel?.ElementAtOrDefault(0), entity?.Rel?.ElementAtOrDefault(1),
        entity?.Title
      };
      Assert.Equal(expected, obtained);
    }

    [Theory]
    [InlineData(typeof(SirenSerializer))]
    [InlineData(typeof(MediaSerializer))]
    public void Deserialize_HasProperties_Test(Type serializerType)
    {
      // Given
      var siren =
        "{" +
          "\"properties\":{" +
            "\"prop1\":\"value1\"," +
            "\"prop2\":\"value2\"," +
            "\"prop3\":[\"item1\",\"item2\"]" +
          "}," +
        "}";

      // When
      var serializer = (ISerializer)Activator.CreateInstance(serializerType);
      var entity = serializer.Deserialize(siren);
      var properties = entity?.Properties;
      var array = properties["prop3"]?.Value as IList<object>;

      // Then
      var expected = new[] { "value1", "value2", "item1", "item2" };
      var obtained = new[]{
        properties["prop1"]?.Value,
        properties["prop2"]?.Value,
        array?[0],
        array?[1]
      };
      Assert.Equal(expected, obtained);
    }

    [Theory]
    [InlineData(typeof(SirenSerializer))]
    [InlineData(typeof(MediaSerializer))]
    public void Deserialize_HasLinks_Test(Type serializerType)
    {
      // Given
      var siren =
        "{" +
          "\"links\":[" +
            "{" +
              "\"class\":[\"class\",\"Other.Class\"]," +
              "\"rel\":[\"rel\",\"Other.Rel\"]," +
              "\"title\":\"Link\"," +
              "\"href\":\"http://host.com\"," +
              "\"type\":\"text/json\"" +
            "}" +
          "]" +
        "}";

      // When
      var serializer = (ISerializer)Activator.CreateInstance(serializerType);
      var entity = serializer.Deserialize(siren);
      var selfLink = entity?.Links.FirstOrDefault();

      // Then
      var expected = new[] {
        "class", "Other.Class",
        "rel", "Other.Rel",
        "Link",
        "http://host.com",
        "text/json"
      };
      var obtained = new[]{
        selfLink?.Class[0],
        selfLink?.Class[1],
        selfLink?.Rel[0],
        selfLink?.Rel[1],
        selfLink?.Title,
        selfLink?.Href,
        selfLink?.Type
      };
      Assert.Equal(expected, obtained);
    }

    [Theory]
    [InlineData(typeof(SirenSerializer))]
    [InlineData(typeof(MediaSerializer))]
    public void Deserialize_HasActions_Test(Type serializerType)
    {
      // Given
      var siren =
        "{" +
          "\"actions\":[" +
            "{" +
              "\"name\":\"Action\"," +
              "\"title\":\"Some Action\"," +
              "\"method\":\"PUT\"," +
              "\"href\":\"http://host.com/\"," +
              "\"type\":\"text/json\"," +
              "\"fields\":[" +
                "{" +
                  "\"class\":[\"class\",\"Other.Class\"]," +
                  "\"name\":\"field1\"," +
                  "\"type\":\"text\"," +
                  "\"dataType\":\"string\"," +
                  "\"title\":\"Field 1\"," +
                  "\"category\":\"Fields\"," +
                  "\"value\":\"Content\"," +
                  "\"required\":1," +
                  "\"readOnly\":1," +
                  "\"properties\":{" +
                    "\"minLength\":7," +
                    "\"maxLength\":17," +
                    "\"pattern\":\".*\"," +
                    "\"multiline\":1," +
                    "\"allowMany\":1," +
                    "\"allowRange\":1," +
                    "\"allowWildcards\":1" +
                  "}" +
                "}" +
              "]" +
            "}" +
          "]" +
        "}";

      // When
      var serializer = (ISerializer)Activator.CreateInstance(serializerType);
      var entity = serializer.Deserialize(siren);
      var action = entity?.Actions[0];

      // Then
      var expected = new object[] {
        "Action",
        "Some Action",
        "PUT",
        "http://host.com/",
        "text/json",
        "Field1",
        "class",
        "Other.Class",
        "Field 1",
        "Fields",
        "text",
        "string",
        true,
        true,
        "Content",
        true,
        true,
        true,
        17,
        7,
        true,
        ".*",
      };
      var obtained = new object[] {
        action?.Name,
        action?.Title,
        action?.Method,
        action?.Href,
        action?.Type,
        action?.Fields?[0].Name,
        action?.Fields?[0].Class?[0],
        action?.Fields?[0].Class?[1],
        action?.Fields?[0].Title,
        action?.Fields?[0].Category,
        action?.Fields?[0].Type,
        action?.Fields?[0].DataType,
        action?.Fields?[0].ReadOnly.Value,
        action?.Fields?[0].Required.Value,
        action?.Fields?[0].Value,
        action?.Fields?[0]?.Properties?.AllowMany.Value,
        action?.Fields?[0]?.Properties?.AllowRange.Value,
        action?.Fields?[0]?.Properties?.AllowWildcards.Value,
        action?.Fields?[0]?.Properties?.MaxLength.Value,
        action?.Fields?[0]?.Properties?.MinLength.Value,
        action?.Fields?[0]?.Properties?.Multiline.Value,
        action?.Fields?[0]?.Properties?.Pattern
      };
      Assert.Equal(expected, obtained);
    }

    [Theory]
    [InlineData(typeof(SirenSerializer))]
    [InlineData(typeof(MediaSerializer))]
    public void Deserialize_HasEntities_Test(Type serializerType)
    {
      // Given
      var siren =
        "{" +
          "\"entities\":[" +
            "{" +
              "\"class\":[\"class\",\"Other.Class\"]," +
              "\"title\":\"Row\"," +
              "\"rel\":[\"rel\",\"Other.Rel\"]," +
              "\"properties\":{" +
                "\"id\":10," +
                "\"name\":\"Tenth\"" +
              "}," +
              "\"links\":[" +
                "{" +
                  "\"rel\":[\"self\"]," +
                  "\"href\":\"http://host.com/\"" +
                "}" +
              "]" +
            "}," +
          "]" +
        "}";

      // When
      var serializer = (ISerializer)Activator.CreateInstance(serializerType);
      var entity = serializer.Deserialize(siren);
      var first = entity?.Entities.FirstOrDefault();

      // Then
      var expected = new object[] {
        "class",
        "Other.Class",
        "rel",
        "Other.Rel",
        "Row",
        "self",
        "http://host.com/",
        "Id",
        10,
        "Name",
        "Tenth"
      };
      var obtained = new object[] {
        first?.Class[0],
        first?.Class[1],
        first?.Rel[0],
        first?.Rel[1],
        first?.Title,
        first?.Links?[0]?.Rel[0],
        first?.Links?[0].Href,
        first?.Properties?[0].Name,
        first?.Properties?[0].Value,
        first?.Properties?[1].Name,
        first?.Properties?[1].Value,
      };
      Assert.Equal(expected, obtained);
    }

    #endregion
    static void DUMP(string text)
    {
      File.WriteAllText(@"c:\temp\log.txt", text);
    }
  }
}
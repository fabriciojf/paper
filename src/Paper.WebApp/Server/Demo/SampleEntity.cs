using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Paper.Media;
using Paper.Media.Design;
using Paper.Media.Rendering.Entities;

namespace Paper.WebApp.Server.Demo
{
  [ExposeEntity("/Sample")]
  [DataContract(Namespace = Namespaces.Default, Name = "Entity")]
  public class SampleEntity : Entity
  {
    [DataMember(EmitDefaultValue = false, Order = 10)]
    public override NameCollection Class { get; set; }
      = "customEntity, data";

    [DataMember(EmitDefaultValue = false, Order = 20)]
    public override NameCollection Rel { get; set; }
      = "custom";

    [DataMember(EmitDefaultValue = false, Order = 30)]
    public override string Title { get; set; }
      = "My Custom Entity";

    [DataMember(EmitDefaultValue = false, Order = 40)]
    public override PropertyCollection Properties { get; set; }
      = PropertyCollection.Create(new
      {
        Name = "MyCustomEntity",
        Title = "This is awesome!",
        Date = DateTime.Now,
        _DataCount = 3,
        _DataHeaders = PropertyCollection.Create(new
        {
          Name = "Nome",
          Title = "Título",
          Date = "Data"
        })
      });

    [DataMember(EmitDefaultValue = false, Order = 50)]
    public override LinkCollection Links { get; set; }
      = new LinkCollection
      {
        new Link{ Rel = "link", Title = "Google.com", Href = "google.com" },
        new Link{ Rel = "link", Title = "Menu", Href = "{Api}/Menu" }
      };
  }
}

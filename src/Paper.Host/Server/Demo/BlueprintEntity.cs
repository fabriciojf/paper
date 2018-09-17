using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Paper.Media;
using Paper.Media.Design;
using Paper.Media.Design.Papers;
using Toolset;

namespace Paper.Host.Server.Demo
{
  [Expose, Paper("/Blueprint")]
  public class BlueprintEntity : Entity
  {
    private readonly static string Guid = System.Guid.NewGuid().ToString();

    public override NameCollection Class { get; set; }
      = "blueprint";

    public override string Title { get; set; }
      = "Mercadologic";

    public override PropertyCollection Properties { get; set; }
      = new PropertyCollection
      {
        new
        {
          HasNavBox = false,
          Info = new
          {
            Guid,
            Name = "Mlogic",
            Title = "Mercadologic",
            Description = "Sistema de retaguarda do Mercadologic PDV.",
            Version = "1.0.0"
          }
        }
      };

    public override LinkCollection Links { get; set; }
      = new LinkCollection
      {
        new Link{ Rel = "index", Title = "Menu", Href = "/Menu" },
        new Link{ Rel = "index", Title = "SampleEntity", Href = "/Paper/Host/Server/Demo/Sample" },
      };
  }
}

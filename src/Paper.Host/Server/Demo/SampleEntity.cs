using System;
using Paper.Media;
using Paper.Media.Design.Papers;

namespace Paper.Host.Server.Demo
{
  [Paper]
  public class SampleEntity : Entity
  {
    public override NameCollection Class { get; set; }
      = "customEntity, data";

    public override NameCollection Rel { get; set; }
      = "custom";

    public override string Title { get; set; }
      = "My Custom Entity";

    public override PropertyCollection Properties { get; set; }
      = new PropertyCollection
      {
        new
        {
          Name = "MyCustomEntity",
          Title = "This is awesome!",
          Date = DateTime.Now,
          __DataHeaders = new[] { "Nome", "Title", "Date" }
        }
      };

    public override LinkCollection Links { get; set; }
      = new LinkCollection
      {
        new Link{ Rel = "link", Title = "Blueprint", Href = "/Blueprint" },
        new Link{ Rel = "link", Title = "Google.com", Href = "google.com" },
        new Link{ Rel = "link", Title = "Menu", Href = "/Menu" },
        new Link{ Rel = "favicon", Href = "^/favicon.ico" }
      };
  }
}

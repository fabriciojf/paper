using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Paper.Media;
using Paper.Media.Design;
using Paper.Media.Design.Mappings;
using Paper.Media.Design.Papers;
using Paper.Media.Design.Widgets;
using Toolset;

namespace Paper.Host.Server.Demo
{
  [DataContract(Namespace = "")]
  public class User
  {
    [DataMember]
    public int? Id { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public DateTime? Since { get; set; }
  }

  public static class UserDb
  {
    private static Random rnd = new Random(0);

    private static DateTime MakeDate()
    {
      return new DateTime(2018, rnd.Next(1, 12), rnd.Next(1, 28));
    }

    public static User[] All = new[]
    {
      new User{ Id = 1, Name = "Fulano"   , Since = MakeDate() },
      new User{ Id = 2, Name = "Beltrano" , Since = MakeDate() },
      new User{ Id = 3, Name = "Sicrano"  , Since = MakeDate() },
      new User{ Id = 4, Name = "Alano"    , Since = MakeDate() },
      new User{ Id = 5, Name = "Mengano"  , Since = MakeDate() },
      new User{ Id = 6, Name = "Zutano"   , Since = MakeDate() },
      new User{ Id = 7, Name = "Citano"   , Since = MakeDate() },
      new User{ Id = 8, Name = "Perengano", Since = MakeDate() }
    };
  }

  class MyFilter : IFilter
  {
    [FieldTitle("#ID")]
    public Val<int> Id { get; set; }

    [FieldMultiline]
    [FieldMaxLength(30)]
    public string[] Name { get; set; }
  }

  [Paper]
  public class UsersPaper : IPaperBasics, IPaperRows<User>
  {
    public Page RowsPage { get; } = new Page();

    public Sort RowsSort { get; } = new Sort<User>();

    //public IFilter RowFilter { get; } = new Filter()
    //  //.AddFieldsMultiFrom<User>()
    //  .AddFieldMulti<User>(x => x.Id)
    //  .AddFieldMulti("Name", DataType.Text)
    //  ;

    public IFilter RowFilter { get; } = new MyFilter();

    public string GetTitle()
      => "Users Page";

    public NameCollection GetClass()
      => null;

    public NameCollection GetRel()
      => null;

    public IEnumerable<ILink> GetLinks()
      => null;

    public object GetProperties()
      => null;

    public IEnumerable<User> GetRows()
    {
      return UserDb.All
        .FilterBy(this)
        .SortBy(RowsSort)
        .PaginateBy(RowsPage);
    }

    public IEnumerable<HeaderInfo> GetRowHeaders(IEnumerable<User> rows)
      => null;

    public IEnumerable<ILink> GetRowLinks(User row)
      => null;
  }
}

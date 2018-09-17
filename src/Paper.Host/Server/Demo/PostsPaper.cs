using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Paper.Media;
using Paper.Media.Design;
using Paper.Media.Design.Mappings;
using Paper.Media.Design.Papers;
using Toolset;
using Toolset.Data;

namespace Paper.Host.Server.Demo
{
  [DataContract(Namespace = "")]
  public class Post
  {
    [DataMember]
    public int? Id { get; set; }

    [DataMember]
    public int? UserId { get; set; }

    [DataMember]
    public string Title { get; set; }
  }

  public class PostFilter : IFilter
  {
    public Var<int> UserId { get; set; }

    public VarString Title { get; set; }
  }

  public static class PostDb
  {
    private static Random rnd = new Random(0);

    private static DateTime MakeDate()
    {
      return new DateTime(2018, rnd.Next(1, 12), rnd.Next(1, 28));
    }

    public static Post[] All = new[]
    {
      new Post{ Id = 1, Title = "1st post", UserId = UserDb.Get(1).Id },
      new Post{ Id = 2, Title = "2nd post", UserId = UserDb.Get(4).Id },
      new Post{ Id = 3, Title = "3rd post", UserId = UserDb.Get(4).Id },
      new Post{ Id = 4, Title = "4th post", UserId = UserDb.Get(4).Id },
      new Post{ Id = 5, Title = "5th post", UserId = UserDb.Get(6).Id },
      new Post{ Id = 6, Title = "6th post", UserId = UserDb.Get(7).Id },
      new Post{ Id = 7, Title = "7th post", UserId = UserDb.Get(8).Id },
      new Post{ Id = 8, Title = "8th post", UserId = UserDb.Get(8).Id }
    };

    public static Post Get(int id)
    {
      return All.FirstOrDefault(x => x.Id == id);
    }
  }

  [Expose, Paper("/Posts")]
  public class PostsPaper : IPaperBasics, IPaperRows<PostFilter, Post>
  {
    public Page Page { get; } = new Page();

    public Sort Sort { get; } = new Sort<Post>();

    public PostFilter Filter { get; } = new PostFilter();

    public string GetTitle()
      => "Posts Page";

    public NameCollection GetClass()
      => null;

    public NameCollection GetRel()
      => null;

    public IEnumerable<ILink> GetLinks()
      => null;

    public object GetProperties()
      => null;

    public IEnumerable<Post> GetRows()
    {
      return PostDb.All
        .FilterBy(Filter)
        .SortBy(Sort)
        .PaginateBy(Page);
    }

    public IEnumerable<HeaderInfo> GetRowHeaders(IEnumerable<Post> rows)
      => null;

    public IEnumerable<ILink> GetRowLinks(Post row)
      => new ILink[] {
        new LinkTo<UserPaper>(
          paper => paper.Id = row.UserId.Value,
          link => link.AddTitle("Autor")
        ),
        new LinkTo<UsersPaper>(
          paper => paper.Filter.Id = row.UserId.Value,
          link => link.AddTitle("Posts do Autor")
        ),
        new LinkTo<UsersPaper>()
      };
  }
}

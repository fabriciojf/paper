using System.Collections.Generic;
using System.Linq;
using Paper.Media;
using Paper.Media.Design;
using Paper.Media.Design.Papers;

namespace Paper.Host.Server.Demo
{
  [Paper("/Users/{id}")]
  public class UserPaper : IPaperBasics, IPaperData<User>
  {
    public int Id { get; set; }

    public string GetTitle()
      => $"User {Id}";

    public NameCollection GetClass()
      => null;

    public NameCollection GetRel()
      => null;

    public IEnumerable<ILink> GetLinks()
      => new ILink[]{
        new LinkTo<UsersPaper>(),
        new LinkTo<PostsPaper>()
      };

    public object GetProperties()
      => null;

    public User GetData()
      => UserDb.All.FirstOrDefault(x => x.Id == this.Id);

    public IEnumerable<HeaderInfo> GetDataHeaders(User data)
      => null;

    public IEnumerable<ILink> GetDataLinks(User data)
      => new[]{
        new LinkTo<PostsPaper>(
          paper => paper.Filter.UserId = data.Id,
          link => link.AddTitle("Posts do usuário")
        )
      };
  }
}

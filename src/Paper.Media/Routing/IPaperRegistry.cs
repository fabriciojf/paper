using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paper.Media.Routing
{
  public interface IPaperRegistry : IEnumerable<PaperInfo>
  {
    void Add(Type paperType);

    void AddRange(IEnumerable<Type> paperTypes);

    PaperInfo FindPaper<T>();

    PaperInfo FindPaper(Type paperType);

    PaperInfo FindPaper(string path);

    PaperInfo FindPaperByPrefix(string path);
  }
}

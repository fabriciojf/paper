using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paper.Media.Rendering
{
  public interface IPaperCatalog : IEnumerable<PaperSpec>
  {
    void Add(Type paperType);

    void AddRange(IEnumerable<Type> paperTypes);

    PaperSpec FindPaper<T>();

    PaperSpec FindPaper(Type paperType);

    PaperSpec FindPaper(string path);
  }
}

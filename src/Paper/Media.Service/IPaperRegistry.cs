using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Service
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

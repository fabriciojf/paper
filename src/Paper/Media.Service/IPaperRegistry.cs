using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Service
{
  public interface IPaperRegistry : IEnumerable<Type>
  {
    void Add(Type paperType);

    void AddRange(IEnumerable<Type> paperTypes);

    Type FindPaperType(string path);

    Type FindPaperTypeByPrefix(string path);
  }
}

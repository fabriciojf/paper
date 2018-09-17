using System;
using System.Collections.Generic;
using System.Text;

namespace Paper.Media.Design.Papers
{
  public interface IPaperBlueprint : IPaper
  {
    string GetTitle();

    Blueprint GetBlueprint();

    ILink GetIndex();
  }
}
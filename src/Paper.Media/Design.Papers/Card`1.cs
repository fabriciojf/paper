namespace Paper.Media.Design.Papers
{
  public class Card<T> : Card
    where T : Card
  {
    public new T Data
    {
      get => base.Data is T data ? data : default(T);
      set => base.Data = value;
    }
  }
}
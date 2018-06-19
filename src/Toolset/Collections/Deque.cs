using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolset.Collections
{
  public class Deque : Deque<object>
  {
    public Deque()
    {
    }

    public Deque(int capacity)
      : base(capacity)
    {
    }

    public Deque(IEnumerable<object> items)
      : base(items)
    {
    }
  }

  public class Deque<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IList, ICollection
  {
    private List<T> list;

    public Deque()
    {
      list = new List<T>();
    }

    public Deque(int capacity)
    {
      list = new List<T>(capacity);
    }

    public Deque(IEnumerable<T> items)
    {
      list = new List<T>(items);
    }

    public bool IsEmpty => list.Count == 0;

    public T RemoveAt(int index)
    {
      var item = list[index];
      list.RemoveAt(index);
      return item;
    }

    public void Add(T item) => list.Add(item);

    #region Compatibilidade com Queue

    public T First => list.First();

    public T Last => list.Last();

    public void Enqueue(T item) => list.Add(item);

    public T Dequeue() => RemoveAt(0);

    #endregion

    #region Compatibilidade com Stack

    public T Peek => list.Last();

    public void Push(T item) => list.Add(item);

    public T Pop() => RemoveAt(list.Count - 1);

    #endregion 

    #region Implementação de IList<T>

    public T this[int index]
    {
      get => list[index];
      set => list[index] = value;
    }

    public int Count => list.Count;

    public void Clear() => list.Clear();

    public bool Contains(T item) => list.Contains(item);

    public void CopyTo(T[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);

    public IEnumerator<T> GetEnumerator() => list.GetEnumerator();

    public int IndexOf(T item) => list.IndexOf(item);

    public void Insert(int index, T item) => list.Insert(index, item);

    public bool Remove(T item) => list.Remove(item);

    #endregion

    #region Implementação das demais interfaces

    void IList<T>.RemoveAt(int index) => list.RemoveAt(index);

    void ICollection<T>.Add(T item) => list.Add(item);

    bool ICollection<T>.IsReadOnly => ((ICollection<T>)list).IsReadOnly;

    object IList.this[int index]
    {
      get => this[index];
      set => this[index] = (T)value;
    }

    bool IList.IsReadOnly => ((IList)list).IsReadOnly;

    bool IList.IsFixedSize => ((IList)list).IsFixedSize;

    object ICollection.SyncRoot => ((ICollection)list).SyncRoot;

    bool ICollection.IsSynchronized => ((ICollection)list).IsSynchronized;

    int IList.Add(object value) => ((IList)list).Add(value);

    bool IList.Contains(object value) => ((IList)list).Contains(value);

    void ICollection.CopyTo(Array array, int index) => ((ICollection)list).CopyTo(array, index);

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)list).GetEnumerator();

    int IList.IndexOf(object value) => ((IList)list).IndexOf(value);

    void IList.Insert(int index, object value) => ((IList)list).Insert(index, value);

    void IList.Remove(object value) => ((IList)list).Remove(value);

    void IList.RemoveAt(int index) => ((IList)list).RemoveAt(index);

    #endregion
  }
}

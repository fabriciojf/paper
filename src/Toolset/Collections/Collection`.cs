using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolset.Collections;

namespace Toolset.Collections
{
  /// <summary>
  /// Implementação de uma coleção com possibilidade de herança e sobreposição e métodos.
  /// </summary>
  /// <typeparam name="T">O tipo da coleção.</typeparam>
  /// <seealso cref="System.Collections.Generic.IList{T}" />
  /// <seealso cref="System.Collections.Generic.ICollection{T}" />
  /// <seealso cref="System.Collections.Generic.IEnumerable{T}" />
  /// <seealso cref="System.Collections.IEnumerable" />
  /// <seealso cref="System.Collections.IList" />
  /// <seealso cref="System.Collections.ICollection" />
  /// <seealso cref="System.Collections.Generic.IReadOnlyList{T}" />
  /// <seealso cref="System.Collections.Generic.IReadOnlyCollection{T}" />
  public class Collection<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IList, ICollection, IReadOnlyList<T>, IReadOnlyCollection<T>
  {
    private readonly List<T> list;
    private readonly ItemStore store;

    public Collection()
    {
      this.list = new List<T>();
      this.store = new ItemStore(this.list);
    }

    public Collection(int capacity)
    {
      this.list = new List<T>(capacity);
      this.store = new ItemStore(this.list);
    }

    public Collection(IEnumerable<T> items)
    {
      this.list = new List<T>(items.Count());
      this.store = new ItemStore(this.list);
      OnCommitAdd(store, items);
    }

    #region Métodos de INTERCEPTAÇÃO de inserção

    /// <summary>
    /// Método de inserção de itens na coleção.
    /// É garantido que qualquer tentativa de adicionar elementos na coleção
    /// será feita por este método.
    /// Uma subclasse pode reescrever este método para modificar o comportamento
    /// de inserção de itens na coleção.
    /// </summary>
    /// <param name="store">A instância de lista na qual os itens devem ser inseridos.</param>
    /// <param name="items">Os itens que estão sendo adicionados à coleção.</param>
    /// <param name="index">
    /// Um índice opcional a partir do qual é esperado que os itens sejam inseridos.
    /// Quando índice for -1 é esperado que os itens sejam adicionados no fim da coleção.
    /// </param>
    protected virtual void OnCommitAdd(ItemStore store, IEnumerable<T> items, int index = -1)
    {
      if (index == -1 || index >= store.Count)
      {
        store.AddMany(items);
      }
      else
      {
        foreach (var item in items)
        {
          store.AddAt(index++, item);
        }
      }
    }

    #endregion

    #region Métodos de INSERÇÃO e REMOÇÂO

    public virtual T this[int index]
    {
      get => list[index];
      set
      {
        if (IsReadOnly)
          throw new UnmodifiableException("A coleção não pode ser modificada.");

        list.RemoveAt(index);
        OnCommitAdd(store, value.AsSingle(), index);
      }
    }

    public virtual void Add(T item)
    {
      if (IsReadOnly)
        throw new UnmodifiableException("A coleção não pode ser modificada.");

      OnCommitAdd(store, item.AsSingle());
    }

    public virtual void AddAt(int index, T item)
    {
      if (IsReadOnly)
        throw new UnmodifiableException("A coleção não pode ser modificada.");

      OnCommitAdd(store, item.AsSingle(), index);
    }

    public virtual void AddMany(IEnumerable<T> items)
    {
      if (IsReadOnly)
        throw new UnmodifiableException("A coleção não pode ser modificada.");

      OnCommitAdd(store, items);
    }

    public virtual void Clear()
    {
      if (IsReadOnly)
        throw new UnmodifiableException("A coleção não pode ser modificada.");

      list.Clear();
    }

    public virtual bool Remove(T item)
    {
      if (IsReadOnly)
        throw new UnmodifiableException("A coleção não pode ser modificada.");

      return list.Remove(item);
    }

    public virtual void RemoveAt(int index)
    {
      if (IsReadOnly)
        throw new UnmodifiableException("A coleção não pode ser modificada.");

      list.RemoveAt(index);
    }

    public virtual void RemoveRange(int index, int count)
    {
      if (IsReadOnly)
        throw new UnmodifiableException("A coleção não pode ser modificada.");

      var item = list[index];
      list.RemoveRange(index, count);
    }

    public virtual void RemoveMany(IEnumerable<T> items)
    {
      if (IsReadOnly)
        throw new UnmodifiableException("A coleção não pode ser modificada.");

      foreach (var item in items)
      {
        list.Remove(item);
      }
    }

    public virtual void RemoveWhen(Predicate<T> match)
    {
      if (IsReadOnly)
        throw new UnmodifiableException("A coleção não pode ser modificada.");

      list.RemoveAll(match);
    }

    #endregion

    #region Outros métodos

    public virtual bool IsReadOnly { get; protected set; }

    public virtual int Count => list.Count;

    public virtual bool Contains(T item)
    {
      return list.Contains(item);
    }

    public virtual void CopyTo(T[] array, int arrayIndex)
    {
      list.CopyTo(array, arrayIndex);
    }

    public virtual IEnumerator<T> GetEnumerator()
    {
      return list.GetEnumerator();
    }

    public virtual int IndexOf(T item)
    {
      return list.IndexOf(item);
    }

    public virtual void ForEach(Action<T> action)
    {
      list.ForEach(action);
    }

    #endregion

    #region Redirecionamentos de interface

    void IList<T>.Insert(int index, T item) => AddAt(index, item);

    object IList.this[int index]
    {
      get => this[index];
      set => this[index] = (T)value;
    }

    int ICollection.Count => Count;

    int IList.Add(object value)
    {
      Add((T)value);
      return IndexOf((T)value);
    }

    void IList.Clear() => Clear();

    bool IList.Contains(object value) => Contains((T)value);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    int IList.IndexOf(object value) => IndexOf((T)value);

    void IList.Insert(int index, object value) => AddAt(index, (T)value);

    void IList.Remove(object value) => Remove((T)value);

    void IList.RemoveAt(int index) => RemoveAt(index);

    #endregion

    #region Implementação específica de interfaces

    void ICollection.CopyTo(Array array, int index) => ((IList)list).CopyTo(array, index);

    bool IList.IsFixedSize => ((IList)list).IsFixedSize;

    object ICollection.SyncRoot => ((IList)list).SyncRoot;

    bool ICollection.IsSynchronized => ((IList)list).IsSynchronized;

    #endregion

    /// <summary>
    /// Repositório de estocagem dos itens da coleção.
    /// Usado como ponto de acesso ao estoque de itens durante sobrescrita de
    /// operações da coleção.
    /// </summary>
    /// <seealso cref="System.Collections.Generic.IList{T}" />
    /// <seealso cref="System.Collections.Generic.ICollection{T}" />
    /// <seealso cref="System.Collections.Generic.IEnumerable{T}" />
    /// <seealso cref="System.Collections.IEnumerable" />
    /// <seealso cref="System.Collections.IList" />
    /// <seealso cref="System.Collections.ICollection" />
    /// <seealso cref="System.Collections.Generic.IReadOnlyList{T}" />
    /// <seealso cref="System.Collections.Generic.IReadOnlyCollection{T}" />
    protected class ItemStore : IEnumerable<T>
    {
      private readonly List<T> list;

      internal ItemStore(List<T> list) => this.list = list;

      public int Count => list.Count;

      public T Get(int index) => list[index];

      public void Add(T item) => list.Add(item);

      public void AddAt(int index, T item) => list.Insert(index, item);

      public void AddMany(IEnumerable<T> items) => list.AddRange(items);

      public void Remove(T item) => list.Remove(item);

      public void RemoveAt(int index) => list.RemoveAt(index);

      public void RemoveRange(int index, int count) => list.RemoveRange(index, count);

      public void RemoveMany(IEnumerable<T> items) => items.ToArray().All(list.Remove);

      public void RemoveWhen(Predicate<T> predicate) => list.RemoveAll(predicate);

      public IEnumerator<T> GetEnumerator() => list.GetEnumerator();

      IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();
    }
  }
}
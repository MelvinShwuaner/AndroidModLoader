using System.Collections;
namespace NeoModLoader.utils.Collections;

/// <summary>
/// a list of slots which can contain a variable
/// </summary>
public sealed class SlotList<T> : IList<T>, IReadOnlyList<T>
{
    public class Slot
    {
        public T Item;

        public Slot(T item)
        {
            Item = item;
        }
    }
    public sealed class SlotStore : IList<Slot>, IReadOnlyList<Slot>
    {
        private readonly List<Slot> slots = new();
        private readonly List<int> active = new();
        private readonly List<int> open = new();
 
        public IReadOnlyList<Slot> Slots => slots;
        public IReadOnlyList<int> Active => active;
        public IReadOnlyList<int> Open => open;
 
        public int Next => open.Count > 0 ? open.Pop() : slots.Count;
 
        public bool IsEmpty(int index) => !active.Contains(index);
 
        public bool Exists(int index) => index >= 0 && index < slots.Count;
 
        public int Add(T item)
        {
            int index = Next;
            Set(index, item);
            return index;
        }
        public void Allocate(int num)
        {
            for (int i = 0; i < num; i++)
            {
                open.Add(slots.Count);
                slots.Add(null);
            }
        }
        public void Set(int index, T item)
        {
            Set(index, new Slot(item));
        }
        public void Set(int index, Slot item, int OverrideActive = -1)
        {
            Allocate(index-slots.Count+1);
            slots[index] = item;
            open.Remove(index);
            if (active.Contains(index) || OverrideActive < -1) return;
            if (OverrideActive == -1)
            {
                active.Add(index);
            }
            else
            {
                active.Insert(OverrideActive, index);
            }
        }
        public void Clear(int index)
        {
            if (!Exists(index)) return;
 
            slots[index] = null;
 
            if (active.Remove(index))
            {
                open.Add(index);
            }
        }
        public Slot Get(int slotIndex)
        {
            if (!Exists(slotIndex))
                throw new ArgumentOutOfRangeException(nameof(slotIndex));
            return slots[slotIndex];
        }

        public void Add(Slot item)
        {
            slots.Add(item);
            if (item == null)
            {
               open.Add(slots.Count-1);
            }
            else
            {
                active.Add(slots.Count-1);
            }
        }

        public void Clear()
        {
            slots.Clear();
            active.Clear();
            open.Clear();
        }

        public bool Contains(Slot item)
        {
            return slots.Contains(item);
        }

        public void CopyTo(Slot[] array, int arrayIndex)
        {
            slots.CopyTo(array, arrayIndex);
        }

        public bool Remove(Slot item)
        {
            int i = IndexOf(item);
            if (i < 0) return false;
            Clear(i);
            return true;
        }

        public int Count
        {
            get => slots.Count;
        }

        public bool IsReadOnly
        {
            get => false;
        }

        public IEnumerator<Slot> GetEnumerator()
        {
            return slots.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int IndexOf(Slot item)
        {
            return slots.IndexOf(item);
        }

        public void Insert(int index, Slot item)
        {
            if (Exists(index) && !IsEmpty(index))
            {
                Slot displaced = slots[index];
                int next = Next;
                active[active.IndexOf(index)] = next;
                Set(next, displaced, -2);
            }
            Set(index, item);
        }

        public void RemoveAt(int index)
        {
           Clear(index);
        }

        public Slot this[int index]
        {
            get => slots[index];
            set => Set(index, value);
        }
    }
 
    public readonly SlotStore Slots = new();
 
    public int Count => Slots.Active.Count;
    public bool IsReadOnly => false;
 
    public T this[int index]
    {
        get => Get(index);
        set => Set(index, value);
    }
 
    public T Get(int index)
    {
        return Slots.Slots[Slots.Active[index]].Item;
    }
 
    public void Set(int index, T value)
    {
        Slots.Set(Slots.Active[index], value);
    }
 
    public void Add(T item) => Slots.Add(item);
 
    public void Insert(int index, T item)
    {
        int slot = Slots.Next;
        Slots.Set(slot, new Slot(item), index);
    }
 
    public void RemoveAt(int index)
    {
        Slots.Clear(Slots.Active[index]);
    }
 
    public bool Remove(T item)
    {
        int index = IndexOf(item);
        if (index < 0) return false;
        RemoveAt(index);
        return true;
    }
 
    public int IndexOf(T item)
    {
        for (int i = 0; i < Slots.Active.Count; i++)
        {
            if (Equals(Slots.Slots[Slots.Active[i]].Item, item))
                return i;
        }
        return -1;
    }
 
    public bool Contains(T item) => IndexOf(item) >= 0;
 
    public void Clear() => Slots.Clear();
 
    public void CopyTo(T[] array, int arrayIndex)
    {
        if (arrayIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        if (array.Length - arrayIndex < Count)
            throw new ArgumentException("Destination array is too small.", nameof(array));
 
        for (int i = 0; i < Slots.Active.Count; i++)
            array[arrayIndex + i] = Slots.Slots[Slots.Active[i]].Item;
    }
 
    public T GetRandom()
    {
        return Count == 0 ? throw new InvalidOperationException("Cannot get a random item from an empty list.") : Slots.Slots[Slots.Active[Randy.randomInt(0, Count)]].Item;
    }
 
    public IEnumerator<T> GetEnumerator()
    {
        foreach (int slot in Slots.Active)
            yield return Slots.Slots[slot].Item;
    }
 
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
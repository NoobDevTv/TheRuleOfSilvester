using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TheRuleOfSilvester.Core
{
    public class Inventory<T> : ICollection<T> where T : Cell
    {
        public int Count => items.Where(i => !i.IsDeleted).Count();

        public int MaximumCount { get; }

        public bool IsReadOnly => false;

        public bool SpaceAvailable => Count < MaximumCount;

        public T this[int slot]
        {
            get => items.FirstOrDefault(i => i.Slot == slot && !i.IsDeleted)?.Item;
            set => Add(value, slot);
        }

        private readonly List<InventoryItem> items;
        private readonly SemaphoreSlim semaphoreSlim;


        public Inventory(int maxSlots)
        {
            items = new List<InventoryItem>();
            semaphoreSlim = new SemaphoreSlim(1, 1);
            MaximumCount = maxSlots;
        }
        public Inventory() : this(int.MaxValue)
        {

        }


        public void Add(T item, int slot)
        {
            if (!SpaceAvailable)
                throw new IndexOutOfRangeException("Not enough space available");

            InternalAdd(item, slot);
        }
        public void Add(T item)
            => Add(item, -1);

        public bool TryAdd(T item, int slot)
        {
            if (!SpaceAvailable)
                return false;

            InternalAdd(item, slot);

            return true;
        }
        public bool TryAdd(T item)
            => TryAdd(item, -1);

        public void Clear()
        {
            semaphoreSlim.Wait();
            items.Clear();
            semaphoreSlim.Release();
        }

        public bool Contains(T item)
        {
            semaphoreSlim.Wait();
            var result = items.Any(i => i.Item == item && !i.IsDeleted);
            semaphoreSlim.Release();
            return result;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            semaphoreSlim.Wait();
            items
                .Where(i => !i.IsDeleted)
                .Select(i => i.Item)
                .ToArray()
                .CopyTo(array, arrayIndex);
            semaphoreSlim.Release();
        }

        public IEnumerator<T> GetEnumerator()
            => items.Where(i => !i.IsDeleted).Select(i => i.Item).GetEnumerator();

        public bool Remove(T item)
        {
            bool returnValue = false;
            semaphoreSlim.Wait();
            var slot = items.FirstOrDefault(i => i.Item == item && !i.IsDeleted);

            if (slot != null)
            {
                slot.Item = null;
                returnValue = slot.IsDeleted = true;
            }

            semaphoreSlim.Release();

            return returnValue;
        }

        public bool RemoveAt(int pos)
        {
            bool returnValue = false;
            semaphoreSlim.Wait();
            var slot = items.FirstOrDefault(x => x.Slot == pos);

            if (slot.IsDeleted)
                return false;

            if (slot != null)
            {
                slot.Item = null;
                returnValue = slot.IsDeleted = true;
            }

            semaphoreSlim.Release();

            return returnValue;
        }

        public Inventory<T> ForEach(Action<T> action)
        {
            foreach (var item in items.Where(i => !i.IsDeleted).Select(i => i.Item))
                action(item);

            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        private void InternalAdd(T item, int slot)
        {
            InventoryItem itemSlot;

            semaphoreSlim.Wait();
            if (slot < 0)
                itemSlot = items.FirstOrDefault(i => i.IsDeleted);
            else
                itemSlot = items.FirstOrDefault(i => i.Slot == slot);

            if (itemSlot == null)
            {
                itemSlot = new InventoryItem()
                {
                    Slot = (items.Count < 1 ? 0 : items.Max(i => i.Slot)) + 1
                };

                items.Add(itemSlot);
            }


            itemSlot.Item = item;
            itemSlot.IsDeleted = false;
            semaphoreSlim.Release();
        }

        private class InventoryItem
        {
            public int Slot { get; set; }
            public T Item { get; set; }

            public bool IsDeleted { get; set; }
        }
    }
}

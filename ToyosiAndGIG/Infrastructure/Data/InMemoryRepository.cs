using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace ToyosiAndGIG.Infrastructure.Data
{
    public class InMemoryRepository<T> where T : class
    {
        private readonly ConcurrentDictionary<Guid, T> _items = new();
        private readonly Func<T, Guid> _getId;

        public InMemoryRepository(Func<T, Guid> getId)
        {
            _getId = getId;
        }

        public IEnumerable<T> GetAll() => _items.Values.ToArray();

        public T Get(Guid id) => _items.TryGetValue(id, out var v) ? v : null;

        public void Add(T item)
        {
            var id = _getId(item);
            _items[id] = item;
        }

        public bool Update(T item)
        {
            var id = _getId(item);
            if (!_items.ContainsKey(id)) return false;
            _items[id] = item;
            return true;
        }

        public bool Delete(Guid id) => _items.TryRemove(id, out _);
    }
}

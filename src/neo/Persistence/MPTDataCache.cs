
using Neo.Cryptography.MPT;
using Neo.IO;
using Neo.IO.Caching;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo.Persistence
{
    internal class MPTDataCache<TKey, TValue> : DataCache<TKey, TValue>
    where TKey : IEquatable<TKey>, ISerializable, new()
    where TValue : class, ICloneable<TValue>, ISerializable, new()
    {
        private MPTTrie<TKey, TValue> mptTrie;

        public MPTNode Root => mptTrie.Root;

        public MPTDataCache(IReadOnlyStore store, UInt256 root)
        {
            mptTrie = new MPTTrie<TKey, TValue>(store as ISnapshot, root);
        }

        protected override void AddInternal(TKey key, TValue value)
        {
            mptTrie.Put(key, value);
        }

        protected override void DeleteInternal(TKey key)
        {
            mptTrie.Delete(key);
        }

        protected override IEnumerable<(TKey, TValue)> SeekInternal(byte[] keyOrPrefix, SeekDirection direction)
        {
            ByteArrayComparer comparer = direction == SeekDirection.Forward ? ByteArrayComparer.Default : ByteArrayComparer.Reverse;
            return mptTrie.Find(keyOrPrefix).OrderBy(p => p.Key.ToArray(), comparer);
        }

        protected override TValue GetInternal(TKey key)
        {
            return mptTrie[key];
        }

        protected override TValue TryGetInternal(TKey key)
        {
            return mptTrie[key];
        }

        protected override void UpdateInternal(TKey key, TValue value)
        {
            mptTrie.Put(key, value);
        }
    }
}
﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading;
using App.Core.Extensions;
using static System.String;

namespace App.Core.Caching
{
    public partial class MemoryCacheManager :  ICacheManager
    {
        // Wwe put a special string into cache if value is null,
        // otherwise our 'Contains()' would always return false,
        // which is bad if we intentionally wanted to save NULL values.
        public const string FakeNull = "__[NULL]__";

        private readonly MemoryCache _cache;
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _keyLocks = new ConcurrentDictionary<string, SemaphoreSlim>();

        public MemoryCacheManager()
        {
            _cache = new MemoryCache("SmartStore");
        }

        public bool IsDistributedCache => false;

        private bool TryGet<T>(string key, out T value)
        {
            value = default(T);

            object obj = _cache.Get(key);

            if (obj != null)
            {
                if (obj.Equals(FakeNull))
                {
                    return true;
                }

                value = (T)obj;
                return true;
            }

            return false;
        }

        public T Get<T>(string key)
        {
            TryGet(key, out T value);

            return value;
        }

        public T Get<T>(string key, Func<T> acquirer, TimeSpan? duration = null)
        {
            if (TryGet(key, out T value))
            {
                return value;
            }

            lock (Intern(key))
            {
                // atomic operation must be outer locked
                if (!TryGet(key, out value))
                {
                    value = acquirer();
                    Put(key, value, duration);
                    return value;
                }
            }

            return value;
        }

        public IEnumerable<T> GetCollection<T>(string key)
        {
            TryGetCollection(key, out IEnumerable<T> value);

            return value;
        }

        public IEnumerable<T> GetCollection<T>(string key, Func<IEnumerable<T>> acquirer, TimeSpan? duration = null)
        {
            if (TryGetCollection(key, out IEnumerable<T> value))
            {
                return value;
            }

            lock (Intern(key))
            {
                // atomic operation must be outer locked
                if (!TryGetCollection(key, out value))
                {
                    value = acquirer();
                    Put(key, value, duration);
                    return value;
                }
            }

            return value;
        }

        private bool TryGetCollection<T>(string key, out IEnumerable<T> value)
        {
            value = default(IEnumerable<T>);

            object obj = _cache.Get(key);

            if (obj != null)
            {
                if (obj.Equals(FakeNull))
                {
                    return true;
                }

                value = (IEnumerable<T>)obj;
                return true;
            }

            return false;
        }

        //public async Task<T> GetAsync<T>(string key, Func<Task<T>> acquirer, TimeSpan? duration = null)
        //{
        //    T value;

        //    if (TryGet(key, out value))
        //    {
        //        return value;
        //    }

        //    // get the async (semaphore) locker specific to this key
        //    var keyLock = AsyncLock.Acquire(key);

        //    using (await keyLock.LockAsync())
        //    {
        //        if (!TryGet(key, out value))
        //        {
        //            value = await acquirer().ConfigureAwait(false);
        //            Put(key, value, duration);
        //            return value;
        //        }
        //    }

        //    return value;
        //}

        public void Put(string key, object value, TimeSpan? duration = null)
        {
            _cache.Set(key, value ?? FakeNull, GetCacheItemPolicy(duration));
        }

        public bool Contains(string key)
        {
            return _cache.Contains(key);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public string[] Keys(string pattern)
        {
            //Guard.NotEmpty(pattern, nameof(pattern));

            var keys = _cache.AsParallel().Select(x => x.Key);

            if (pattern.IsEmpty() || pattern == "*")
            {
                return keys.ToArray();
            }

            return keys.Where(x => x.StartsWith(pattern, StringComparison.OrdinalIgnoreCase)).ToArray();
        }

        public void RemoveByPattern(string pattern)
        {
            var keysToRemove = Keys(pattern);

            lock (_cache)
            {
                // lock atomic operation
                foreach (string key in keysToRemove)
                {
                    _cache.Remove(key);
                }
            }
        }

        public void Clear()
        {
            RemoveByPattern("*");
        }

        public virtual ISet GetHashSet(string key)
        {
            var set = Get(key, () => new MemorySet(this));
            return set;
        }

        private CacheItemPolicy GetCacheItemPolicy(TimeSpan? duration)
        {
            var absoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration;

            if (duration.HasValue)
            {
                absoluteExpiration = DateTime.UtcNow + duration.Value;
            }

            var cacheItemPolicy = new CacheItemPolicy
            {
                AbsoluteExpiration = absoluteExpiration,
                SlidingExpiration = ObjectCache.NoSlidingExpiration
            };

            return cacheItemPolicy;
        }

        //protected override void OnDispose(bool disposing)
        //{
        //    if (disposing)
        //        _cache.Dispose();
        //}
    }
}

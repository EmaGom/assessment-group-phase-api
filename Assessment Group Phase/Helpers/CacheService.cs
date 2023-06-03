using Assessment.Group.Phase.Enums;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assessment.Group.Phase.Helpers
{
    public class CacheService : ICacheService
    {
        private readonly CacheConfiguration _cacheConfiguration;
        private readonly IMemoryCache _memoryCache;
        public CacheService(IMemoryCache memoryCache, IOptions<CacheConfiguration> cacheConfiguration)
        {
            _memoryCache = memoryCache;
            _cacheConfiguration = cacheConfiguration.Value;
        }

        public void Remove(CacheKeyEnum cacheKey)
        {
            _memoryCache.Remove(cacheKey);
        }

        public IList<T> Set<T>(CacheKeyEnum cacheKey, IList<T> value)
        {
            var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(_cacheConfiguration.AbsoluteExpirationInMinutes),
                SlidingExpiration = TimeSpan.FromMinutes(_cacheConfiguration.SlidingExpirationInMinutes)
            };
            return _memoryCache.Set(cacheKey, value, memoryCacheEntryOptions);
        }

        public T Set<T>(CacheKeyEnum cacheKey, T value)
        {
            var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(_cacheConfiguration.AbsoluteExpirationInMinutes),
                SlidingExpiration = TimeSpan.FromMinutes(_cacheConfiguration.SlidingExpirationInMinutes)
            };
            return _memoryCache.Set(cacheKey, value, memoryCacheEntryOptions);
        }

        public bool TryGetValue<T>(CacheKeyEnum cacheKey, out IList<T> value)
        {
            return _memoryCache.TryGetValue(cacheKey, out value);
        }

        public bool TryGetValue<T>(CacheKeyEnum cacheKey, out T value)
        {
            return _memoryCache.TryGetValue(cacheKey, out value);
        }

        public void Update<T>(CacheKeyEnum cacheKey, IList<T> value)
        {
            if (_memoryCache.TryGetValue(cacheKey, out IList<T> currentValue))
            {
                Remove(cacheKey);
                Set(cacheKey, currentValue.Union(value));
            }
            else
            {
                Set(cacheKey, value);
            }
        }
    }
}

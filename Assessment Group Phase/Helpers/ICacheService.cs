using Assessment.Group.Phase.Enums;
using System.Collections.Generic;

namespace Assessment.Group.Phase.Helpers
{
    public interface ICacheService
    {
        bool TryGetValue<T>(CacheKeyEnum cacheKey, out IList<T> value);
        bool TryGetValue<T>(CacheKeyEnum cacheKey, out T value);
        T Set<T>(CacheKeyEnum cacheKey, T value);
        void Remove(CacheKeyEnum cacheKey);
        void Update<T>(CacheKeyEnum cacheKey, IList<T> value);
    }
}

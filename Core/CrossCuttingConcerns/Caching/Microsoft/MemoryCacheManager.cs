using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Core.CrossCuttingConcerns.Caching.Microsoft
{
    public class MemoryCacheManager : ICacheManager
    {
        private readonly IMemoryCache _memoryCache;

        public MemoryCacheManager(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public void Add(string key, object value, int duration)
        {
            _memoryCache.Set(key, value, TimeSpan.FromMinutes(duration));
        }

        public T Get<T>(string key)
        {
            
            return _memoryCache.Get<T>(key);
        }

        public object Get(string key)
        {
            return Get<object>(key);
        }

        public bool IsAdd(string key)
        {
            //boşlgelebilir oyüzden böle yazıldı
            return _memoryCache.TryGetValue(key, out _);
        }

        public void Remove(string key)
        {
          _memoryCache.Remove(key); 
        }

        public void RemoveByPattern(string pattern)
        {
            //öncelikli olarak bir cache oluşturuyorum bunu sakladıktna sonra silmek istersem silcegime dair komutu yolluyorum  ve siliyorum 
            var cacheExtiresCollectionDefination = typeof(MemoryCache).GetProperty("EntriesCollection",System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var cacheExtiresCollection = cacheExtiresCollectionDefination.GetValue(_memoryCache) as dynamic;
            List<ICacheEntry> cachecollectionvalues = new List<ICacheEntry>();
            foreach (var item in cacheExtiresCollection)
            {
                ICacheEntry cacheItemValue = item.GetType().GetProperty("Value").GetValue(item, null);
                cachecollectionvalues.Add(cacheItemValue);
            }
            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var keytoremove = cachecollectionvalues.Where(x => regex.IsMatch(x.Key.ToString())).Select(x=>x.Key).ToList();
            foreach (var key in keytoremove)
            {
                _memoryCache.Remove(key);
            }
        }
    }
}

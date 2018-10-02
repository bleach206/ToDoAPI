using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Linq;

using Newtonsoft.Json;

using Common.Interface;

namespace Common
{
    public class ETagCache : IETagCache
    {
        #region Fields
        private readonly IDistributedCache _cache;
        private readonly HttpContext _httpContext;
        #endregion

        #region Constructor

        public ETagCache(IDistributedCache cache, IHttpContextAccessor httpContext) => (_cache, _httpContext) = (cache, httpContext.HttpContext);
        #endregion

        #region Methods

        public T GetCachedObject<T>(string cacheKeyName)
        {
            var requestETag = GetRequestedETag();

            if (!string.IsNullOrWhiteSpace(requestETag))
            {                
                var cacheKey = $"{cacheKeyName}-{requestETag}";
            
                var cachedObjectJson = _cache.GetString(cacheKey);
                 
                if (!string.IsNullOrWhiteSpace(cachedObjectJson))
                {
                    T cachedObject = JsonConvert.DeserializeObject<T>(cachedObjectJson);
                    return cachedObject;
                }
            }

            return default(T);
        }

        private string GetRequestedETag() => _httpContext.Request.Headers.ContainsKey("If-None-Match") ? _httpContext.Request.Headers["If-None-Match"].FirstOrDefault() : string.Empty;         

        public bool SetCachedObject(string cacheKeyName, dynamic objectToCache)
        {
            if (!IsCacheable(objectToCache))            
                return true;            

            var requestETag = GetRequestedETag();
            var responseETag = Convert.ToBase64String(objectToCache.RowVersion);
            
            if (objectToCache != null && responseETag != null)
            {
                var cacheKey = $"{cacheKeyName}-{responseETag}";
                string serializedObjectToCache = JsonConvert.SerializeObject(objectToCache);               
                _cache.SetStringAsync(cacheKey, serializedObjectToCache, new DistributedCacheEntryOptions() { AbsoluteExpiration = DateTime.Now.AddMinutes(30) });
            }
            
            _httpContext.Response.Headers.Add("ETag", responseETag);
           
            return !(_httpContext.Request.Headers.ContainsKey("If-None-Match") && responseETag == requestETag);
        }

        private bool IsCacheable(dynamic objectToCache)
        {
            var type = objectToCache.GetType();
            return type.GetProperty("RowVersion") != null;
        }
        #endregion
    }
}

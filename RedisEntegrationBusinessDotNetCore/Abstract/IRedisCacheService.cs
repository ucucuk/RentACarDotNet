using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisEntegrationBusinessDotNetCore.Abstract
{
    public interface IRedisCacheService
    {
        Task<string> GetValueAsync(string key);
        Task<bool> SetValueAsync <T>(string key, T value);
        Task<bool> Clear(string key);
        void ClearAll();
    }
}

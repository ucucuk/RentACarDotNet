using Microsoft.AspNetCore.Mvc;
using RedisEntegrationBusinessDotNetCore.Abstract;
using RentACarDotNetCore.Application.Requests.RedisCache;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RentACarDotNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedisCacheController : ControllerBase
    {
        private readonly IRedisCacheService _redisCacheService;

        public RedisCacheController(IRedisCacheService redisCacheService)
        {
            _redisCacheService = redisCacheService;
        }

        [HttpGet("cache/{key}")]
        public async Task<IActionResult> Get(string key)
        {
            return Ok(await _redisCacheService.GetValueAsync(key));
        }

        [HttpPost("cache/set")]
        public async Task<IActionResult> Set([FromBody] RedisCacheRequest redisCacheRequest)
        {
            await _redisCacheService.SetValueAsync(redisCacheRequest.Key, redisCacheRequest.Value);
            return Ok();
        }

        [HttpDelete("cache/{key}")]
        public async Task<bool> Delete(string key)
        {
            return await _redisCacheService.Clear(key);
        }
    }
}

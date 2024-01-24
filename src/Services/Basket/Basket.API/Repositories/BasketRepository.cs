using Basket.API.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Basket.API.Repositories;

public class BasketRepository : IBasketRepository
{
    private readonly IDistributedCache _redisCache;

    public BasketRepository(IDistributedCache redisCache)
    {
        _redisCache = redisCache ?? throw new ArgumentNullException(nameof(redisCache));
    }

    public async Task<ShoppingCart?> GetBasket(string userName)
    {
        var basket = await _redisCache.GetStringAsync(userName);

   if (string.IsNullOrEmpty(basket)) return null;

        return JsonConvert.DeserializeObject<ShoppingCart>(basket);

    }

    public async Task<ShoppingCart?> UpdateBasket(ShoppingCart basket)
    {
        if (basket.UserName != null) { 
            //var options = new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(1) };
            var options = new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromDays(7) };

            await _redisCache.SetStringAsync(basket.UserName, JsonConvert.SerializeObject(basket), options);

            return await GetBasket(basket.UserName);
        }

        return null;
    }

    public async Task DeleteBasket(string userName)
    {
        await _redisCache.RemoveAsync(userName);
    }
}


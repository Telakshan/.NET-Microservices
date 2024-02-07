using Basket.API.Entities;
using Basket.API.GrpcServices;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Basket.API.Repositories;

public class BasketRepository : IBasketRepository
{
    private readonly IDistributedCache _redisCache;
    private readonly IDiscountGrpcService _discountGrpcService;

    public BasketRepository(IDistributedCache redisCache, IDiscountGrpcService discountGrpcService)
    {
        _redisCache = redisCache;
        _discountGrpcService = discountGrpcService;
    }

    public async Task<ShoppingCart?> GetBasket(string userName)
    {
        var basket = await _redisCache.GetStringAsync(userName);

        if (string.IsNullOrEmpty(basket)) return null;

        return JsonConvert.DeserializeObject<ShoppingCart>(basket);

    }

    public async Task<ShoppingCart?> UpdateBasket(ShoppingCart basket)
    {
        foreach (var item in basket.Items)
        {
            if (item.ProductName != null)
            {
                var coupon = await _discountGrpcService.GetDiscount(item.ProductName);
                item.Price -= coupon.Amount;
            }
        }

        if (basket.UserName != null) { 
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


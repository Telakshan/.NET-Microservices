using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Basket.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class BasketController: ControllerBase
{
    private readonly IBasketRepository _repository;
    private readonly IDiscountGrpcService _discountGrpcService;
    private readonly ILogger<BasketController> _logger;

    public BasketController(IBasketRepository repository, IDiscountGrpcService discountGrpcService, ILogger<BasketController> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _discountGrpcService = discountGrpcService ?? throw new ArgumentNullException(nameof(discountGrpcService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet("{userName}", Name = "GetBasket")]
    [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ShoppingCart>> GetBasket(string userName)
    {
        return Ok(await _repository.GetBasket(userName) ?? new ShoppingCart(userName));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart basket)
    {
        foreach (var item in basket.Items)
        {
            if (item.ProductName != null)
            {
                var coupon = await _discountGrpcService.GetDiscount(item.ProductName);
                item.Price -= coupon.Amount;
            }
        }

        return Ok(await _repository.UpdateBasket(basket));
    }

    [HttpDelete("{userName}", Name = "DeleteBasket")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> DeleteBasket(string userName)
    {
        await _repository.DeleteBasket(userName);

        return Ok();
    }

}

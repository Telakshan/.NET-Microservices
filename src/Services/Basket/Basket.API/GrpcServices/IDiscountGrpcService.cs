using Basket.API.Entities;
using Discount.Grpc.Protos;
using static Basket.API.GrpcServices.DiscountGrpcService;

namespace Basket.API.GrpcServices;

public interface IDiscountGrpcService
{
    public Task<CouponModel> GetDiscount(string productName);
    public Task<CouponModel> GetSelectDiscounts(List<ShoppingCartRecord> shoppingCartItems);
}

using Basket.API.Entities;
using Discount.Grpc.Protos;

namespace Basket.API.GrpcServices;

public class DiscountGrpcService: IDiscountGrpcService
{
    private readonly DiscountProtoService.DiscountProtoServiceClient _discountProtoServiceClient;

    public DiscountGrpcService(DiscountProtoService.DiscountProtoServiceClient discountProtoServiceClient)
    {
        _discountProtoServiceClient = discountProtoServiceClient ?? throw new ArgumentNullException(nameof(discountProtoServiceClient));
    }

    public async Task<CouponModel> GetDiscount(string productName)
    {
        var discountRequest = new GetDiscountRequest { ProductName = productName };

        return await _discountProtoServiceClient.GetDiscountAsync(discountRequest);
    }

    public Task<CouponModel> GetSelectDiscounts(List<ShoppingCartRecord> shoppingCartItems)
    {
        throw new NotImplementedException();
    }

    public record ShoppingCartRecord(string ProductName, string ProductId);
}

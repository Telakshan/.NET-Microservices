using AutoMapper;
using Discount.Grpc.Entities;
using Discount.Grpc.Protos;
using Discount.Grpc.Repositories;
using Grpc.Core;

namespace Discount.Grpc.Services;

public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
{
    private readonly IDiscountRepository _repository;
    private readonly ILogger<DiscountService> _logger;
    private readonly IMapper _mapper;

    public DiscountService(IDiscountRepository repository, IMapper mapper,  ILogger<DiscountService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(_mapper));
    }

    public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
    {
        var coupon = _mapper.Map<Coupon>(request.Coupon);

        var created = await _repository.CreateDiscount(coupon);

        if (!created)
        {
            throw new RpcException(new Status(StatusCode.Unavailable, $"Discount with the ProductName={request.Coupon.ProductName} couldn't be created"));
        }

        _logger.LogInformation("Discount is successfully created. ProductName: {ProductName}", coupon.ProductName);

        return _mapper.Map<CouponModel>(coupon);
    }

    public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
    {

        var removed = await _repository.DeleteDiscount(request.ProductName);

        return new DeleteDiscountResponse
        {
            Success = removed
        };
    }

    public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
    {
        var coupon = await _repository.GetDiscount(request.ProductName);

        if (coupon == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Discount with productName={request.ProductName} is not found"));
        }

        _logger.LogInformation($"Discount is retrieved for ProductName: {coupon.ProductName}, Amount: {coupon.Amount}");

        return _mapper.Map<CouponModel>(coupon);
    }

    public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
    {
        var coupon = await _repository.GetDiscount(request.Coupon.ProductName);

        if (coupon == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Discount with productName={request.Coupon.ProductName} is not found"));
        }

        var updated = await _repository.UpdateDiscount(_mapper.Map<Coupon>(request.Coupon));

        if (updated)
        {
            return request.Coupon;
        }

        throw new RpcException(new Status(StatusCode.Unavailable, $"Coupon wasn't updated for {request.Coupon.ProductName}; Database error"));
    }

    public override async Task GetAllDiscounts(GetAllDiscountsRequest request, IServerStreamWriter<CouponModel> responseStream, ServerCallContext context)
    {
        var discounts = await _repository.GetAllDiscounts();

        if (discounts == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Discount list id null"));
        }

        foreach (var discount in discounts)
        {
            var couponModel = _mapper.Map<CouponModel>(discount);

            await responseStream.WriteAsync(couponModel);
        }
    }

    public override async Task GetSelectDiscounts(GetSelectDiscountsRequest request, IServerStreamWriter<CouponModel> responseStream, ServerCallContext context)
    {
        var discounts = await _repository.GetSelectDiscounts(null);

        if (discounts == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "None of the selected items were found"));
        }

        foreach (var discount in discounts)
        {
            var couponModel = _mapper.Map<CouponModel>(discount);

            await responseStream.WriteAsync(couponModel);
        }
    }
}

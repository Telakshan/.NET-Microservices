using AutoMapper;
using Discount.Grpc.Entities;
using Discount.Grpc.Protos;
using Discount.Grpc.Repositories;
using Grpc.Core;
using Microsoft.AspNetCore.Authentication;

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

        await _repository.CreateDiscount(coupon);

        return _mapper.Map<CouponModel>(coupon);
    }

    public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
    {
        var itemToBeRemoved = await _repository.GetDiscount(request.ProductName);

        if (itemToBeRemoved == null) 
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Discount with product name {request.ProductName} is not found"));
        }

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

    public override Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
    {
        var coupon = awa
    }
}

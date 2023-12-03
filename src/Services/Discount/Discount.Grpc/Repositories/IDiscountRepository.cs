﻿using Discount.Grpc.Entities;

namespace Discount.Grpc.Repositories;

public interface IDiscountRepository
{
    Task<Coupon> GetDiscount(string productName);
    Task<bool> CreateDiscount(Coupon coupon);
    Task<bool> UpdateDiscount(Coupon coupon);
    Task<bool> DeleteDiscount(string productName);
    Task<IEnumerable<Coupon>> GetAllDiscounts();
    Task<IEnumerable<Coupon>> GetSelectDiscounts(IList<ShoppingCartRecord> products);
}

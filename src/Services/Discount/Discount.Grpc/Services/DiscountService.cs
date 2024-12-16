using Discount.Grpc.Data;
using Discount.Grpc.Models;
using Grpc.Core;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Discount.Grpc.Services
{
    public class DiscountService
        (DiscountContext discountContext, ILogger<DiscountService> logger)
        : DiscountProtoService.DiscountProtoServiceBase
    {
        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var coupon = await discountContext.Coupons
                .FirstOrDefaultAsync(c => c.ProductName == request.ProductName)
                ?? new Coupon { ProductName = "No Discount", Amount = 0, Description = "No Discount" };

            logger.LogInformation("Discount retrieved: ProductName={ProductName}, Amount={Amount}", coupon.ProductName, coupon.Amount);

            return coupon.Adapt<CouponModel>();
        }

        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            if (request.Coupon == null || string.IsNullOrWhiteSpace(request.Coupon.ProductName))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid Discount Request: ProductName is required"));

            var coupon = request.Coupon.Adapt<Coupon>();

            await discountContext.Coupons.AddAsync(coupon);
            await discountContext.SaveChangesAsync();

            logger.LogInformation("Discount created: ProductName={ProductName}", coupon.ProductName);

            return coupon.Adapt<CouponModel>();
        }


        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            if (string.IsNullOrWhiteSpace(request.Coupon?.ProductName))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid Discount Request: ProductName is required"));

            var existingCoupon = await discountContext.Coupons
                .FirstOrDefaultAsync(c => c.ProductName == request.Coupon.ProductName)
                ?? throw new RpcException(new Status(StatusCode.NotFound, $"Discount with ProductName: {request.Coupon.ProductName} not found"));

            existingCoupon.Amount = request.Coupon.Amount;
            existingCoupon.Description = request.Coupon.Description;

            discountContext.Coupons.Update(existingCoupon);
            await discountContext.SaveChangesAsync();

            logger.LogInformation("Discount updated: ProductName={ProductName}", existingCoupon.ProductName);

            return existingCoupon.Adapt<CouponModel>();
        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            var coupon = await discountContext.Coupons
                .FirstOrDefaultAsync(c => c.ProductName == request.ProductName)
                ?? throw new RpcException(new Status(StatusCode.NotFound, $"Discount with ProductName={request.ProductName} not found"));

            discountContext.Coupons.Remove(coupon);
            await discountContext.SaveChangesAsync();

            logger.LogInformation("Discount deleted: ProductName={ProductName}", coupon.ProductName);

            return new DeleteDiscountResponse { Success = true };
        }
    }
}

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
            var coupon = request.Coupon?.Adapt<Coupon>()
                ?? throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid Discount Request"));

            await discountContext.Coupons.AddAsync(coupon);
            await discountContext.SaveChangesAsync();

            logger.LogInformation("Discount created: ProductName={ProductName}", coupon.ProductName);

            return coupon.Adapt<CouponModel>();
        }

        public override Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            return base.UpdateDiscount(request, context);
        }

        public override Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            return base.DeleteDiscount(request, context);
        }
    }
}

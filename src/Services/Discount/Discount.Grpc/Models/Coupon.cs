namespace Discount.Grpc.Models
{
    public class Coupon
    {
        public int Id { get; protected set; }
        public string ProductName { get; protected set; } = default!;
        public string Description { get; protected set; } = default!;
        public int Amount { get; protected set; }
    }
}

namespace EShop.Domain.DTO
{
    public class PaymentIntentCreateRequest
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "usd";
        public List<string> PaymentMethodTypes { get; set; } = new List<string> { "card" };
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
    }
}

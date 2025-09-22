namespace EShop.Domain.DTO
{
    public class CheckoutSessionCreateRequest
    {
        public List<CheckoutLineItem> LineItems { get; set; } = new List<CheckoutLineItem>();
        public string Mode { get; set; } = "payment";
        public string SuccessUrl { get; set; } = string.Empty;
        public string CancelUrl { get; set; } = string.Empty;
        public string? CustomerEmail { get; set; }
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
    }

    public class CheckoutLineItem
    {
        public CheckoutPriceData PriceData { get; set; } = new CheckoutPriceData();
        public int Quantity { get; set; }
    }

    public class CheckoutPriceData
    {
        public string Currency { get; set; } = "usd";
        public CheckoutProductData ProductData { get; set; } = new CheckoutProductData();
        public long UnitAmount { get; set; } // Amount in cents
    }

    public class CheckoutProductData
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<string> Images { get; set; } = new List<string>();
    }
}

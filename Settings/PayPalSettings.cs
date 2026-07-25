namespace BioGamaEcuador.Settings
{
    public class PayPalSettings
    {
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = "https://api-m.sandbox.paypal.com";
        public string ReturnUrl { get; set; } = "/api/payments/paypal/confirm";
        public string CancelUrl { get; set; } = "/Orders/Cart";
    }
}

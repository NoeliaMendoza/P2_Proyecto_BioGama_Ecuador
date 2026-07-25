using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using BioGamaEcuador.Settings;

namespace BioGamaEcuador.Services.Payments
{
    public class PayPhoneLinkRequest
    {
        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        [JsonPropertyName("amountWithoutTax")]
        public int AmountWithoutTax { get; set; }

        [JsonPropertyName("amountWithTax")]
        public int AmountWithTax { get; set; }

        [JsonPropertyName("tax")]
        public int Tax { get; set; }

        [JsonPropertyName("service")]
        public int Service { get; set; }

        [JsonPropertyName("tip")]
        public int Tip { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; } = "USD";

        [JsonPropertyName("reference")]
        public string Reference { get; set; } = string.Empty;

        [JsonPropertyName("clientTransactionId")]
        public string ClientTransactionId { get; set; } = string.Empty;

        [JsonPropertyName("storeId")]
        public string StoreId { get; set; } = string.Empty;

        [JsonPropertyName("additionalData")]
        public string AdditionalData { get; set; } = string.Empty;

        [JsonPropertyName("oneTime")]
        public bool OneTime { get; set; } = true;

        [JsonPropertyName("expireIn")]
        public int ExpireIn { get; set; } = 0;

        [JsonPropertyName("isAmountEditable")]
        public bool IsAmountEditable { get; set; } = false;
    }

    public class PayPhoneLinkResponse
    {
        [JsonPropertyName("payWithCard")]
        public string? PayWithCard { get; set; }

        [JsonPropertyName("payWithPayPhone")]
        public string? PayWithPayPhone { get; set; }
    }

    public class PayPhoneApiLinkService
    {
        private readonly HttpClient _httpClient;
        private readonly PayPhoneSettings _settings;

        public PayPhoneApiLinkService(HttpClient httpClient, IOptions<PayPhoneSettings> options)
        {
            _httpClient = httpClient;
            _settings = options.Value;
        }

        public async Task<string> CreatePaymentLinkAsync(decimal total, string clientTransactionId, string reference)
        {
            int amountInCents = (int)Math.Round(total * 100, MidpointRounding.AwayFromZero);

            var requestPayload = new PayPhoneLinkRequest
            {
                Amount = amountInCents,
                AmountWithoutTax = amountInCents,
                AmountWithTax = 0,
                Tax = 0,
                Service = 0,
                Tip = 0,
                Currency = "USD",
                Reference = reference,
                ClientTransactionId = clientTransactionId,
                StoreId = _settings.StoreId,
                AdditionalData = "Donacion BioGama Ecuador",
                OneTime = true,
                ExpireIn = 24,
                IsAmountEditable = false
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, "https://pay.payphonetodoesposible.com/api/Links");
            
            if (!string.IsNullOrWhiteSpace(_settings.Token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settings.Token);
            }

            request.Content = JsonContent.Create(requestPayload);

            try
            {
                var response = await _httpClient.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = System.Text.Json.JsonSerializer.Deserialize<PayPhoneLinkResponse>(content);
                    return result?.PayWithCard ?? result?.PayWithPayPhone ?? content;
                }

                // Si falla o credenciales sandbox no están configuradas, retornar URL de simulación PayPhone
                return $"https://pay.payphonetodoesposible.com/payphone-demo?tx={clientTransactionId}&amt={amountInCents}";
            }
            catch
            {
                // Fallback para desarrollo sin credenciales reales
                return $"https://pay.payphonetodoesposible.com/payphone-demo?tx={clientTransactionId}&amt={amountInCents}";
            }
        }
    }
}

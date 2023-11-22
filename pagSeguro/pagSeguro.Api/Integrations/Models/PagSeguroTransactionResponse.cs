using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace pagSeguro.Api.Integrations.Models
{
    public partial class PagSeguroTransactionResponse
    {
        [JsonProperty("transaction")]
        public Transaction Transaction { get; set; }
        [JsonProperty("errors")]
        public Errors Errors { get; set; }

    }

     public partial class Errors
    {
        [JsonProperty("error")]
        public Error Error { get; set; }
    }
    public partial class Error
    {
        [JsonProperty("code")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }

    public partial class Transaction
    {
        [JsonProperty("date")]
        public DateTimeOffset Date { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("reference")]
        public string Reference { get; set; }

        [JsonProperty("type")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Type { get; set; }

        [JsonProperty("status")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Status { get; set; }

        [JsonProperty("lastEventDate")]
        public DateTimeOffset LastEventDate { get; set; }

        [JsonProperty("paymentMethod")]
        public PaymentMethod PaymentMethod { get; set; }

        [JsonProperty("grossAmount")]
        public string GrossAmount { get; set; }

        [JsonProperty("discountAmount")]
        public string DiscountAmount { get; set; }

        [JsonProperty("feeAmount")]
        public string FeeAmount { get; set; }

        [JsonProperty("netAmount")]
        public string NetAmount { get; set; }

        [JsonProperty("extraAmount")]
        public string ExtraAmount { get; set; }

        [JsonProperty("installmentCount")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long InstallmentCount { get; set; }

        [JsonProperty("itemCount")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long ItemCount { get; set; }

        [JsonProperty("items")]
        public Items Items { get; set; }

        [JsonProperty("sender")]
        public Sender Sender { get; set; }

        [JsonProperty("shipping")]
        public Shipping Shipping { get; set; }
        [JsonProperty("paymentLink")]
        public string PaymentLink { get; set; }

    }

    public partial class Items
    {
        [JsonProperty("item")]
        public Item Item { get; set; }
    }

    public partial class Item
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("quantity")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Quantity { get; set; }

        [JsonProperty("amount")]
        public string Amount { get; set; }
    }

    public partial class PaymentMethod
    {
        [JsonProperty("type")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Type { get; set; }

        [JsonProperty("code")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Code { get; set; }
    }

    public partial class Sender
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone")]
        public Phone Phone { get; set; }
    }

    public partial class Phone
    {
        [JsonProperty("areaCode")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long AreaCode { get; set; }

        [JsonProperty("number")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Number { get; set; }
    }

    public partial class Shipping
    {
        [JsonProperty("address")]
        public PSAddress Address { get; set; }

        [JsonProperty("type")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Type { get; set; }

        [JsonProperty("cost")]
        public string Cost { get; set; }
    }

    public partial class PSAddress
    {
        [JsonProperty("street")]
        public string Street { get; set; }

        [JsonProperty("number")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Number { get; set; }

        [JsonProperty("complement")]
        public string Complement { get; set; }

        [JsonProperty("district")]
        public string District { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }
    }

    public partial class PagSeguroTransactionResponse
    {
        public static PagSeguroTransactionResponse FromJson(string json) => 
            JsonConvert.DeserializeObject<PagSeguroTransactionResponse>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this PagSeguroTransactionResponse self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }
}
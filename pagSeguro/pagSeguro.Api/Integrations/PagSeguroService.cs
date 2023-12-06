using System.Net.Http.Headers;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;
using pagSeguro.Api.Integrations.Models;
using pagSeguro.Api.Models;
using pagSeguro.Api.Services;

namespace pagSeguro.Api.Integrations
{
    public class PagSeguroService : IPagSeguroService
    {
        private readonly ISettingService _settingService;
        private readonly ILogService _logger;
        private string _url;

        public PagSeguroService(ISettingService settingService,
             ILogService logger)
        {
            _settingService = settingService;
            _logger = logger;
            GetUrl();
        }
        private string GetUrl()
        {
            //TODO: Colocar controle por ambiente
            var url = _settingService.GetByName("PagSeguro.Url");
            _url = !string.IsNullOrEmpty(url) ? url : "https://ws.pagseguro.uol.com.br/v2/";
            return _url;
        }


        public async Task<PagSeguroBoletoResponse> ChargeBoleto(PagSeguroBoletoRequest request)
        {
            var pagSeguroBoletoResponse = new PagSeguroBoletoResponse();

            try
            {
                var token = _settingService.GetByName("PagSeguro.Token");

                token = string.IsNullOrEmpty(token) ? "19B4FC4ECF674337B90E98DE25480B6E" : token;

                using (var httpClient = new HttpClient())
                {
                    using (var req = new HttpRequestMessage(new HttpMethod("POST"), _url))
                    {
                        req.Headers.TryAddWithoutValidation("Authorization", token);
                        req.Headers.TryAddWithoutValidation("x-idempotency-key", "");
                        req.Headers.TryAddWithoutValidation("x-api-version: 4.0", "");

                        var jsonString = JsonConvert.SerializeObject(request);
                        req.Content = new StringContent(jsonString);
                        req.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                        var response = await httpClient.SendAsync(req);
                        var val = response.Content.ReadAsStringAsync().Result;

                        if (response.IsSuccessStatusCode)
                        {
                            pagSeguroBoletoResponse = JsonConvert.DeserializeObject<PagSeguroBoletoResponse>(val);

                            if (!string.IsNullOrEmpty(pagSeguroBoletoResponse.status) &&
                                pagSeguroBoletoResponse.status == "WAITING")
                            {
                                pagSeguroBoletoResponse.Success = true;
                            }
                        }
                        else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            pagSeguroBoletoResponse = JsonConvert.DeserializeObject<PagSeguroBoletoResponse>(val);
                            pagSeguroBoletoResponse.Success = false;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return pagSeguroBoletoResponse;
        }

        public async Task<PagSeguroCardResponse> ChargeCreditCard(PagSeguroCardRequest request)
        {
            var pagSeguroCardResponse = new PagSeguroCardResponse();

            try
            {
                var token = _settingService.GetByName("PagSeguro.Token");

                token = string.IsNullOrEmpty(token) ? "19B4FC4ECF674337B90E98DE25480B6E" : token;

                using (var httpClient = new HttpClient())
                {
                    using (var req = new HttpRequestMessage(new HttpMethod("POST"), _url))
                    {
                        req.Headers.TryAddWithoutValidation("Authorization", token);
                        req.Headers.TryAddWithoutValidation("x-idempotency-key", "");
                        req.Headers.TryAddWithoutValidation("x-api-version: 4.0", "");

                        var jsonString = JsonConvert.SerializeObject(request);
                        req.Content = new StringContent(jsonString);
                        req.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                        var response = await httpClient.SendAsync(req);
                        var val = response.Content.ReadAsStringAsync().Result;

                        if (response.IsSuccessStatusCode)
                        {
                            pagSeguroCardResponse = JsonConvert.DeserializeObject<PagSeguroCardResponse>(val);

                            if (!string.IsNullOrEmpty(pagSeguroCardResponse.status) &&
                                pagSeguroCardResponse.status == "AUTHORIZED")
                            {
                                pagSeguroCardResponse.Success = true;
                            }
                        }
                        else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            pagSeguroCardResponse = JsonConvert.DeserializeObject<PagSeguroCardResponse>(val);
                            pagSeguroCardResponse.Success = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return pagSeguroCardResponse;
        }

        public async Task<bool> Notification(PagSeguroCardResponse request)
        {
            var affectedRows = 0;
            var res = false;
            //var order = _context.Orders.FirstOrDefault(o => o.OrderIdentifier == request.id);

            //if (order != null)
            //{
            //    var paymentStatus = ObtemStatus(request.status);

            //    if (order.PaymentStatusId != paymentStatus)
            //    {
            //        order.PaymentStatusId = paymentStatus;
            //        affectedRows = _context.SaveChanges();
            //    }
            //}

            if (affectedRows > 0)
                res = true;

            return res;
        }

        private int ObtemStatus(string status)
        {
            var iStatus = -1;

            switch (status)
            {
                case "PAID":
                    iStatus = (int)Enums.PaymentStatus.Concluido;
                    break;
                case "AUTHORIZED":
                    iStatus = (int)Enums.PaymentStatus.EmAndamento;
                    break;
                case "DECLINED":
                    iStatus = (int)Enums.PaymentStatus.NaoAceito;
                    break;
                case "CANCELED":
                    iStatus = (int)Enums.PaymentStatus.Cancelado;
                    break;
                case "WAITING":
                    iStatus = (int)Enums.PaymentStatus.Pendente;
                    break;
                default:
                    iStatus = -1;
                    break;
            }

            return iStatus;
        }

        public async Task<string> GetSessionId()
        {
            var sessionId = string.Empty;

            try
            {
                var token = _settingService.GetByName("PagSeguroToken");
                var email = _settingService.GetByName("PagSeguroEmail");

                if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(email))
                {
                    _url = _url + "sessions?email=" + email + "&token=" + token;

                    using (var httpClient = new HttpClient())
                    {
                        using (var req = new HttpRequestMessage(new HttpMethod("POST"), _url))
                        {
                            var response = await httpClient.SendAsync(req);
                            var val = response.Content.ReadAsStringAsync().Result;

                            if (response.IsSuccessStatusCode)
                            {
                                var yourXml = XElement.Parse(val);
                                // Look up specific values by name:
                                sessionId = yourXml.Descendants().First(node => node.Name == "id").Value;

                                if (string.IsNullOrEmpty(sessionId))
                                {

                                }
                            }
                            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                            {
                            }
                        }
                    }

                }
            }
            catch (Exception)
            {

            }

            return sessionId;
        }
        public async Task<PagSeguroTransactionResponse> CreatePayment(PagSeguroTransactionRequest request)
        {
            var response = new PagSeguroTransactionResponse();

            var token = _settingService.GetByName("PagSeguroToken");
            var email = _settingService.GetByName("PagSeguroEmail");

            if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(email))
            {
                _url = GetUrl();
                _url = _url + "transactions?email=" + email + "&token=" + token;

                using (var httpClient = new HttpClient())
                {
                    using (var req = new HttpRequestMessage(new HttpMethod("POST"), _url))
                    {
                        var collection = new List<KeyValuePair<string, string>>();
                        collection.Add(new("paymentMode", "default"));
                        collection.Add(new("paymentMethod", request.PaymentMethod));
                        collection.Add(new("receiverEmail", email));
                        collection.Add(new("currency", "BRL"));
                        collection.Add(new("extraAmount", "0.00"));
                        collection.Add(new("itemId1", "0001"));
                        collection.Add(new("itemDescription1", "compra feita no site"));
                        collection.Add(new("itemAmount1", request.ItemAmount1));
                        collection.Add(new("itemQuantity1", request.ItemQuantity1));
                        collection.Add(new("notificationURL", request.NotificationURL));
                        collection.Add(new("reference", request.Reference));
                        collection.Add(new("senderName", request.SenderName));
                        collection.Add(new("senderCPF", request.SenderCPF));
                        collection.Add(new("senderAreaCode", request.SenderAreaCode));
                        collection.Add(new("senderPhone", request.SenderPhone));
                        collection.Add(new("senderEmail", request.SenderEmail));
                        collection.Add(new("senderHash", request.SenderHash));
                        collection.Add(new("shippingAddressStreet", request.ShippingAddressStreet));
                        collection.Add(new("shippingAddressNumber", request.ShippingAddressNumber));
                        collection.Add(new("shippingAddressComplement", request.ShippingAddressComplement));
                        collection.Add(new("shippingAddressDistrict", request.ShippingAddressDistrict));
                        collection.Add(new("shippingAddressPostalCode", request.ShippingAddressPostalCode));
                        collection.Add(new("shippingAddressCity", request.ShippingAddressCity));
                        collection.Add(new("shippingAddressState", request.ShippingAddressState));
                        collection.Add(new("shippingAddressCountry", "BRA"));
                        collection.Add(new("shippingType", request.ShippingType)); //ToDo: Revisar
                        //collection.Add(new("shippingCost", request.ShippingCost));
                        collection.Add(new("shippingCost", "0.00"));
                        collection.Add(new("creditCardToken", request.CreditCardToken));
                        collection.Add(new("installmentQuantity", request.InstallmentQuantity));
                        collection.Add(new("installmentValue", request.InstallmentValue));
                        collection.Add(new("noInterestInstallmentQuantity", request.NoInterestInstallmentQuantity));
                        collection.Add(new("creditCardHolderName", request.CreditCardHolderName));
                        collection.Add(new("creditCardHolderCPF", request.CreditCardHolderCPF));
                        collection.Add(new("creditCardHolderBirthDate", request.CreditCardHolderBirthDate));
                        collection.Add(new("creditCardHolderAreaCode", request.CreditCardHolderAreaCode));
                        collection.Add(new("creditCardHolderPhone", request.CreditCardHolderPhone));
                        collection.Add(new("billingAddressStreet", request.BillingAddressStreet));
                        collection.Add(new("billingAddressNumber", request.BillingAddressNumber));
                        collection.Add(new("billingAddressComplement", request.BillingAddressComplement));
                        collection.Add(new("billingAddressDistrict", request.BillingAddressDistrict));
                        collection.Add(new("billingAddressPostalCode", request.BillingAddressPostalCode));
                        collection.Add(new("billingAddressCity", request.BillingAddressCity));
                        collection.Add(new("billingAddressState", request.BillingAddressState));
                        collection.Add(new("billingAddressCountry", request.BillingAddressCountry));
                        var content = new FormUrlEncodedContent(collection);
                        req.Content = content;
                        var resp = await httpClient.SendAsync(req);
                        var val = resp.Content.ReadAsStringAsync().Result;

                        if (resp.IsSuccessStatusCode || resp.StatusCode == System.Net.HttpStatusCode.BadRequest)
                        {
                            XmlDocument doc = new XmlDocument();
                            doc.LoadXml(val);
                            string jsonText = JsonConvert.SerializeXmlNode(doc);
                            response = JsonConvert.DeserializeObject<PagSeguroTransactionResponse>(jsonText);
                        }
                        else if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            response.Errors.Error.Code = (long)resp.StatusCode;
                            response.Errors.Error.Message = "Unauthorized";
                        }
                    }
                }
            }

            return response;
        }

        public async Task<PagSeguroTransactionResponse> CheckStatus(string code)
        {
            var response = new PagSeguroTransactionResponse();

            var token = _settingService.GetByName("PagSeguroToken");
            var email = _settingService.GetByName("PagSeguroEmail");

            if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(email))
            {
                _url = GetUrl();
                _url = _url + "transactions/" + code + "?email=" + email + "&token=" + token;

                using (var httpClient = new HttpClient())
                {
                    using (var req = new HttpRequestMessage(new HttpMethod("GET"), _url))
                    {

                        var resp = await httpClient.SendAsync(req);
                        var val = resp.Content.ReadAsStringAsync().Result;

                        if (resp.IsSuccessStatusCode || resp.StatusCode == System.Net.HttpStatusCode.BadRequest)
                        {
                            XmlDocument doc = new XmlDocument();
                            doc.LoadXml(val);
                            string jsonText = JsonConvert.SerializeXmlNode(doc);
                            response = JsonConvert.DeserializeObject<PagSeguroTransactionResponse>(jsonText);
                        }
                        else if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            response.Errors.Error.Code = (long)resp.StatusCode;
                            response.Errors.Error.Message = "Unauthorized";
                        }
                    }
                }
            }

            return response;
        }
    }
}
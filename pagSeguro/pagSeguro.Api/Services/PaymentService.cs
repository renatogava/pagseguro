using Azure;
using pagSeguro.Api.Integrations;
using pagSeguro.Api.Integrations.Constants;
using pagSeguro.Api.Integrations.Models;
using pagSeguro.Api.Services.Models;

namespace pagSeguro.Api.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPagSeguroService _pagSeguroService;
        private readonly ILogService _logger;
        private readonly ISettingService _settingService;

        public PaymentService(IPagSeguroService pagSeguroService,
            ILogService logger, ISettingService settingService)
        {
            _pagSeguroService = pagSeguroService;
            _logger = logger;
            _settingService = settingService;
        }

        public async Task<CheckStatusResponse> CheckStatus(CheckStatusRequest request)
        {
            var result = new CheckStatusResponse();

            var transactionResponse = await _pagSeguroService.CheckStatus(request.TransactionId);

            switch (transactionResponse.Transaction.Status)
            {
                case (int)TransactionStatus.Paid:
                case (int)TransactionStatus.Available:
                    {
                        result.PaymentStatus = 1;

                        break;
                    }

                case (int)TransactionStatus.WaitingPayment:
                case (int)TransactionStatus.InAnalysis:
                    {
                        result.PaymentStatus = 2;

                        break;
                    }

                default:
                    {
                        result.PaymentStatus = 2;
                        break;
                    }
            }

            return result;
        }

        public async Task<ProcessPaymentResponse> ProcessPayment(ProcessPaymentRequest request)
        {
            var response = new ProcessPaymentResponse();
            var customer = request.Customer;

            if (customer != null)
            {
                var shippingAddress = request.Customer.ShippingAddress;

                var orderAmount = request.TotalPrice;

                switch (request.PaymentMethodId)
                {
                    case (int)Enums.PaymentMethod.CartaoCredito:
                        {
                            var transactionRequest = new PagSeguroTransactionRequest();

                            if (customer == null || shippingAddress == null)
                            {
                                response.Succeeded = false;
                                response.ErrorMessage = "Informações do Pagamento Inválida";
                                return response;
                            }

                            transactionRequest.PaymentMethod = "creditCard";

                            transactionRequest.Reference = Guid.NewGuid().ToString();

                            transactionRequest.ItemDescription1 = "Compra Produto no Site";
                            transactionRequest.ItemQuantity1 = "1";
                            transactionRequest.ItemAmount1 = DecimalFormat(orderAmount);
                            var installments = request.CreditCard.NumberOfPayments;

                            if (installments > 1)
                            {
                                transactionRequest.InstallmentValue = DecimalFormat(request.CreditCard.InstallmentValue);
                                transactionRequest.InstallmentQuantity = installments.ToString();
                                transactionRequest.NoInterestInstallmentQuantity = "12";
                            }
                            else
                            {
                                transactionRequest.InstallmentValue = DecimalFormat(orderAmount);
                                transactionRequest.InstallmentQuantity = "1";
                                transactionRequest.NoInterestInstallmentQuantity = "12";
                            }

                            var urlNotificationPagSeguro = _settingService.GetByName("PagSeguro.UrlNotification");
                            transactionRequest.NotificationURL = urlNotificationPagSeguro;

                            //ShippingAddress
                            transactionRequest.ShippingAddressStreet = shippingAddress.Street;
                            transactionRequest.ShippingAddressNumber = shippingAddress.Number;
                            transactionRequest.ShippingAddressComplement = shippingAddress.Complement;
                            transactionRequest.ShippingAddressDistrict = shippingAddress.Neighbourhood;
                            transactionRequest.ShippingAddressCity = shippingAddress.City;
                            transactionRequest.ShippingAddressPostalCode = shippingAddress.ZipPostalCode;
                            transactionRequest.ShippingAddressState = shippingAddress.State;
                            transactionRequest.ShippingAddressCountry = "BRL";
                            transactionRequest.ShippingCost = "0.00";
                            transactionRequest.ShippingType = "1";

                            //HoldCardData
                            transactionRequest.CreditCardHolderName = request.CreditCard.HolderName;
                            transactionRequest.CreditCardHolderCPF = request.CreditCard.HolderCpf;
                            transactionRequest.CreditCardHolderAreaCode = request.CreditCard.HolderCodeArea;
                            transactionRequest.CreditCardHolderPhone = request.CreditCard.HolderPhone;
                            transactionRequest.CreditCardHolderBirthDate = request.CreditCard.HolderBirthDate;

                            var billingAddress = (request.Customer.BillingAddress == null) ? request.Customer.ShippingAddress : request.Customer.BillingAddress;

                            //BillingAddress
                            transactionRequest.BillingAddressStreet = billingAddress.Street;
                            transactionRequest.BillingAddressNumber = billingAddress.Number;
                            transactionRequest.BillingAddressComplement = billingAddress.Complement;
                            transactionRequest.BillingAddressDistrict = billingAddress.Neighbourhood;
                            transactionRequest.BillingAddressCity = billingAddress.City;
                            transactionRequest.BillingAddressPostalCode = billingAddress.ZipPostalCode;
                            transactionRequest.BillingAddressState = billingAddress.State;
                            transactionRequest.BillingAddressCountry = "BRL";

                            //hash and token
                            transactionRequest.SenderHash = request.SenderHash;
                            transactionRequest.CreditCardToken = request.CreditCard.CreditCardToken;

                            //Sender 
                            transactionRequest.SenderName = customer.Name;
                            transactionRequest.SenderAreaCode = customer.CodeArea;
                            transactionRequest.SenderPhone = customer.Phone;
                            transactionRequest.SenderCPF = customer.CPF;
                            transactionRequest.SenderEmail = customer.Email;

                            var transactionResponse = await _pagSeguroService.CreatePayment(transactionRequest);

                            if (transactionResponse.Transaction != null)
                            {
                                switch (transactionResponse.Transaction.Status)
                                {
                                    case (int)TransactionStatus.Paid:
                                    case (int)TransactionStatus.Available:
                                        {
                                            response.Succeeded = true;
                                            response.PaymentStatus = 1;
                                            response.CreditCardInfo.TransactionId = transactionResponse.Transaction.Code;

                                            break;
                                        }

                                    case (int)TransactionStatus.WaitingPayment:
                                    case (int)TransactionStatus.InAnalysis:
                                        {
                                            var maximoTentativas = 3;
                                            var numeroTentativas = 0;
                                            var transactionStatus = TransactionStatus.WaitingPayment;

                                            while ((transactionStatus == (int)TransactionStatus.WaitingPayment ||
                                            transactionStatus == (int)TransactionStatus.InAnalysis) &&
                                            numeroTentativas < maximoTentativas)
                                            {
                                                //espera um pouquinho antes de consultar o status no PagSeguro
                                                System.Threading.Thread.Sleep(1500);

                                                transactionResponse = await _pagSeguroService.CheckStatus(transactionResponse.Transaction.Code);
                                                transactionStatus = (int)transactionResponse.Transaction.Status;

                                                numeroTentativas++;
                                            }

                                            if (transactionResponse.Transaction.Status == (int)TransactionStatus.Paid ||
                                            transactionResponse.Transaction.Status == (int)TransactionStatus.Available)
                                            {
                                                response.Succeeded = true;
                                                response.PaymentStatus = 1;
                                                response.CreditCardInfo.TransactionId = transactionResponse.Transaction.Code;
                                            }
                                            else if (transactionStatus == (int)TransactionStatus.WaitingPayment ||
                                                transactionStatus == (int)TransactionStatus.InAnalysis)
                                            {
                                                response.Succeeded = true;
                                                response.PaymentStatus = 2;
                                                response.CreditCardInfo.TransactionId = transactionResponse.Transaction.Code;
                                            }
                                            else if (transactionResponse.Transaction.Status == (int)TransactionStatus.Cancelled)
                                            {
                                                response.ErrorMessage = ("Transação Não Autorizada. Confira os dados informados e tente novamente.");
                                            }
                                            else
                                            {
                                                response.ErrorMessage = ("Transação Não Autorizada. Confira os dados informados e tente novamente.");
                                            }
                                            break;
                                        }
                                    default:
                                        response.ErrorMessage = ("Transação Não Autorizada. Confira os dados informados e tente novamente.");
                                        break;
                                }

                            }
                            else
                            {
                                response.Succeeded = false;
                                if (transactionResponse.Errors != null)
                                {
                                    response.ErrorMessage = " Message Erro: " + transactionResponse.Errors.Error.Message;
                                }
                                else
                                {
                                    response.ErrorMessage = "Transação Não Autorizada. Confira os dados informados e tente novamente.";
                                }
                            }

                            break;
                        }
                    case (int)Enums.PaymentMethod.Boleto:
                        {
                            var pagSeguroTransactionRequest = new PagSeguroTransactionRequest();
                            var pagSeguroTransactionResponse = new PagSeguroTransactionResponse();

                            if (customer == null || shippingAddress == null)
                            {
                                response.Succeeded = false;
                                response.ErrorMessage = "Informações de Pagamento Inválidas";
                                return response;
                            }

                            pagSeguroTransactionRequest.PaymentMethod = "Boleto";
                            var referenId = Guid.NewGuid().ToString();
                            pagSeguroTransactionRequest.Reference = referenId;
                            pagSeguroTransactionRequest.ItemId1 = referenId; //ToDo: avaliar
                            var description = _settingService.GetByName("PagSeguro.Description");
                            pagSeguroTransactionRequest.ItemDescription1 = !string.IsNullOrEmpty(description) ? description : "Compra Produto no Site";
                            pagSeguroTransactionRequest.ItemAmount1 = DecimalFormat(orderAmount);
                            pagSeguroTransactionRequest.ItemQuantity1 = "1";
                            var urlNotificationPagSeguro = _settingService.GetByName("PagSeguro.UrlNotification");
                            pagSeguroTransactionRequest.NotificationURL = urlNotificationPagSeguro;

                            pagSeguroTransactionRequest.SenderName = customer.Name;
                            pagSeguroTransactionRequest.SenderCPF = customer.CPF;
                            pagSeguroTransactionRequest.SenderAreaCode = customer.CodeArea;
                            pagSeguroTransactionRequest.SenderPhone = customer.Phone;
                            pagSeguroTransactionRequest.SenderEmail = customer.Email;
                            pagSeguroTransactionRequest.SenderHash = request.SenderHash;
                            pagSeguroTransactionRequest.ShippingAddressStreet = customer.ShippingAddress.Street;
                            pagSeguroTransactionRequest.ShippingAddressNumber = customer.ShippingAddress.Number;
                            pagSeguroTransactionRequest.ShippingAddressComplement = customer.ShippingAddress.Complement;
                            pagSeguroTransactionRequest.ShippingAddressDistrict = customer.ShippingAddress.Neighbourhood;
                            pagSeguroTransactionRequest.ShippingAddressCity = customer.ShippingAddress.City;
                            pagSeguroTransactionRequest.ShippingAddressPostalCode = customer.ShippingAddress.ZipPostalCode;
                            pagSeguroTransactionRequest.ShippingAddressState = customer.ShippingAddress.State;
                            pagSeguroTransactionRequest.ShippingAddressCountry = "BRL";
                            pagSeguroTransactionRequest.ShippingType = "3";
                            pagSeguroTransactionRequest.ShippingCost = "0.00";

                            pagSeguroTransactionResponse = await _pagSeguroService.CreatePayment(pagSeguroTransactionRequest);

                            if (pagSeguroTransactionResponse.Transaction != null &&
                                pagSeguroTransactionResponse.Transaction.Status == 1)
                            {
                                response.Succeeded = true;
                                response.PaymentStatus = 2;
                                response.BoletoInfo.Url = pagSeguroTransactionResponse.Transaction.PaymentLink;
                                response.BoletoInfo.TransactionId = pagSeguroTransactionResponse.Transaction.Code;
                            }
                            else
                            {
                                response.ErrorMessage = " Message Erro: " + pagSeguroTransactionResponse.Errors.Error.Message;
                            }

                            break;
                        }
                    case (int)Enums.PaymentMethod.Pix:

                        response.ErrorMessage = "Pix ainda não implementado";

                        break;

                    default:
                        response.ErrorMessage = "Nenhum método de pagamento informado";
                        break;
                }
            }

            return response;
        }

        private string DecimalFormat(decimal numeric)
        {
            return string.Format("{0:0.00}", numeric).Replace(",", ".");
        }
    }
}

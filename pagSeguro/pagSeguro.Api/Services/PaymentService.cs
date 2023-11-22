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

        public async Task<ProcessPaymentResponse> ProcessPayment(ProcessPaymentRequest request)
        {
            var response = new ProcessPaymentResponse();
            var customer = request.Customer;

            if (customer != null)
            {
                var address = request.Customer.Address;

                var orderAmount = request.TotalPrice;

                switch (request.PaymentMethodId)
                {
                    case (int)Enums.PaymentMethod.CartaoCredito:

                        var transactionRequest = new PagSeguroTransactionRequest();

                        if (customer == null || address == null)
                        {
                            response.Succeeded = false;
                            response.ErrorMessage = "Informações do Pagamento Inválida";
                            return response;
                        }

                        transactionRequest.PaymentMethod = "creditCard";

                        transactionRequest.Reference = Guid.NewGuid().ToString();

                        var description = _settingService.GetByName("PagSeguro.Description");
                        transactionRequest.ItemDescription1 = !string.IsNullOrEmpty(description) ? description : "Compra Produto na Farmácia";
                        transactionRequest.ItemQuantity1 = "1";
                        transactionRequest.ItemAmount1 = DecimalFormat(orderAmount);
                        var installments = request.CreditCard.NumberOfPayments;

                        if (installments > 1)
                        {
                            var installmentValue = orderAmount / installments;
                            transactionRequest.InstallmentValue = DecimalFormat(installmentValue);
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
                        transactionRequest.ShippingAddressStreet = address.Street;
                        transactionRequest.ShippingAddressNumber = address.Number;
                        transactionRequest.ShippingAddressComplement = address.Complement;
                        transactionRequest.ShippingAddressDistrict = address.Neighbourhood;
                        transactionRequest.ShippingAddressCity = address.City;
                        transactionRequest.ShippingAddressPostalCode = address.ZipPostalCode;
                        transactionRequest.ShippingAddressState = address.State;
                        transactionRequest.ShippingAddressCountry = "BRL";
                        transactionRequest.ShippingCost = "0.00";
                        transactionRequest.ShippingType = "1";

                        //HoldCardData
                        transactionRequest.CreditCardHolderName = request.CreditCard.HolderName;
                        transactionRequest.CreditCardHolderCPF = request.CreditCard.HolderCpf;
                        transactionRequest.CreditCardHolderAreaCode = request.CreditCard.HolderCodeArea;
                        transactionRequest.CreditCardHolderPhone = request.CreditCard.HolderPhone;
                        transactionRequest.CreditCardHolderBirthDate = request.CreditCard.HolderBirthDate;

                        //BillingAddress
                        transactionRequest.BillingAddressStreet = address.Street;
                        transactionRequest.BillingAddressNumber = address.Number;
                        transactionRequest.BillingAddressComplement = address.Complement;
                        transactionRequest.BillingAddressDistrict = address.Neighbourhood;
                        transactionRequest.BillingAddressCity = address.City;
                        transactionRequest.BillingAddressPostalCode = address.ZipPostalCode;
                        transactionRequest.BillingAddressState = address.State;
                        transactionRequest.BillingAddressCountry = "BRL";

                        //hash and token
                        transactionRequest.SenderHash = request.SenderHash;
                        transactionRequest.CreditCardToken = request.CreditCard.CreditCardToken;

                        //Sender 
                        transactionRequest.SenderName = request.CreditCard.HolderName;
                        transactionRequest.SenderAreaCode = request.CreditCard.HolderCodeArea;
                        transactionRequest.SenderCPF = request.CreditCard.HolderCpf;
                        transactionRequest.SenderPhone = request.CreditCard.HolderPhone;
                        transactionRequest.SenderEmail = customer.Email;

                        var transactionResponse = await _pagSeguroService.CreatePayment(transactionRequest);

                        if (transactionResponse.Transaction != null)
                        {
                            switch (transactionResponse.Transaction.Status)
                            {
                                case (int)TransactionStatus.Paid:
                                case (int)TransactionStatus.Available:
                                    {
                                        //ProcessOrder(request, response, customer, shippingPrice, subTotalDiscount,
                                        //    orderAmount, address, shoppingcarts, transactionResponse);
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
                                            //ProcessOrder(request, response, customer, shippingPrice, subTotalDiscount,
                                            //    orderAmount, address, shoppingcarts, transactionResponse);
                                        }
                                        else if (transactionStatus == (int)TransactionStatus.WaitingPayment ||
                                            transactionStatus == (int)TransactionStatus.InAnalysis)
                                        {
                                            //não faz nada
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

                    case (int)Enums.PaymentMethod.Boleto:

                        response.ErrorMessage = "Boleto ainda não implementado";

                        break;

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

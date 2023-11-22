namespace pagSeguro.Api.Enums
{
    public enum PaymentStatus
    {
        Pendente = 1,     //WAITING
        EmAndamento = 2,  //AUTHORIZED
        Concluido = 3,    //PAID
        Cancelado = 4,    //CANCELED
        NaoAceito = 5     //DECLINED
    }
}

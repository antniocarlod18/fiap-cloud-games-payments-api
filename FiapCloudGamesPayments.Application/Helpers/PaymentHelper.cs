namespace FiapCloudGamesPayments.Application.Helpers;

public static class PaymentHelper
{
    private static readonly string[] PaymentMethods = new[]
    {
        "CreditCard",
        "DebitCard",
        "Pix",
        "Boleto",
        "Wallet"
    };

    private static readonly Random Random = new Random();

    public static string GetRandomPaymentMethod()
    {
        var index = Random.Next(PaymentMethods.Length);
        return PaymentMethods[index];
    }
}

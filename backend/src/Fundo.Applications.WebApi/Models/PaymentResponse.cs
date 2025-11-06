namespace Fundo.Applications.WebApi.Models
{
    public class PaymentResponse
    {
        public decimal LoanAmount { get; set; }
        public decimal CurrentBalance { get; set; }
        public string Status { get; set; }
    }
}

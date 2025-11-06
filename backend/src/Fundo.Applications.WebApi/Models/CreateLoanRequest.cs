namespace Fundo.Applications.WebApi.Models
{
    public class CreateLoanRequest
    {
        public decimal Amount { get; set; }
        public string ApplicantName { get; set; } = string.Empty;
    }
}

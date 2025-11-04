using Fundo.Domain.Entities;

namespace Fundo.Application.Interfaces
{
    public interface ILoanService
    {
        Task<IEnumerable<Loan>> GetAllAsync();
        Task<Loan?> GetByIdAsync(int id);
        Task<Loan> CreateAsync(decimal amount, string applicantName);
        Task<Loan?> MakePaymentAsync(int id, decimal paymentAmount);
    }
}

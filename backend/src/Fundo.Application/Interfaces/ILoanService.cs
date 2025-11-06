using Fundo.Domain;
using Fundo.Domain.Entities;

namespace Fundo.Application.Interfaces
{
    public interface ILoanService
    {
        Task<PaginatedResponse<Loan>> GetPagedAsync(int pageNumber, int pageSize);
        Task<Loan?> GetByIdAsync(int id);
        Task<Loan> CreateAsync(decimal amount, string applicantName);
        Task<Loan?> MakePaymentAsync(int id, decimal paymentAmount);
    }
}

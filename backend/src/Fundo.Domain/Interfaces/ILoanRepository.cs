using Fundo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fundo.Domain.Interfaces
{
    public interface ILoanRepository
    {
        Task<PaginatedResponse<Loan>> GetPagedAsync(int pageNumber, int pageSize);
        Task<Loan?> GetByIdAsync(int id);
        Task<Loan> AddAsync(Loan loan);
        Task UpdateAsync(Loan loan);
    }
}

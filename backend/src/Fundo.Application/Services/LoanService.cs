using Fundo.Application.Interfaces;
using Fundo.Domain.Entities;
using Fundo.Domain.Interfaces;

namespace Fundo.Application.Services
{
    public class LoanService : ILoanService
    {
        private readonly ILoanRepository _loanRepository;

        public LoanService(ILoanRepository loanRepository)
        {
            _loanRepository = loanRepository;
        }

        public async Task<Loan> CreateAsync(decimal amount, string applicantName)
        {
            if (amount <= 0) throw new ArgumentException("Amount must be greater than zero.", nameof(amount));
            if (string.IsNullOrWhiteSpace(applicantName)) throw new ArgumentException("Applicant name is required.", nameof(applicantName));

            var loan = new Loan
            {
                Amount = amount,
                CurrentBalance = amount,
                ApplicantName = applicantName.Trim(),
                Status = "active"
            };

            return await _loanRepository.AddAsync(loan);
        }

        public async Task<IEnumerable<Loan>> GetAllAsync()
        {
            return await _loanRepository.GetAllAsync();
        }

        public async Task<Loan?> GetByIdAsync(int id)
        {
            return await _loanRepository.GetByIdAsync(id);
        }

        public async Task<Loan?> MakePaymentAsync(int id, decimal paymentAmount)
        {
            if (paymentAmount <= 0) throw new ArgumentException("Payment amount must be greater than zero.", nameof(paymentAmount));

            var loan = await _loanRepository.GetByIdAsync(id);

            if (loan == null) return null;

            if (loan.Status == "paid")
                throw new InvalidOperationException("Loan already paid.");

            if (paymentAmount > loan.CurrentBalance)
                throw new ArgumentException("Payment exceeds remaining balance.", nameof(paymentAmount));

            loan.CurrentBalance -= paymentAmount;
            if (loan.CurrentBalance == 0)
                loan.Status = "paid";

            await _loanRepository.UpdateAsync(loan);

            return loan;
        }
    }
}

using Fundo.Application.Interfaces;
using Fundo.Domain.Entities;

namespace Fundo.Application.Services
{
    public class LoanService : ILoanService
    {
        private static readonly List<Loan> _loans = new();
        private static int _nextId = 1;

        public Task<Loan> CreateAsync(decimal amount, string applicantName)
        {
            if (amount <= 0) throw new ArgumentException("Amount must be greater than zero.", nameof(amount));
            if (string.IsNullOrWhiteSpace(applicantName)) throw new ArgumentException("Applicant name is required.", nameof(applicantName));

            var loan = new Loan
            {
                Id = _nextId++,
                Amount = amount,
                CurrentBalance = amount,
                ApplicantName = applicantName.Trim(),
                Status = "active"
            };

            _loans.Add(loan);

            return Task.FromResult(loan);
        }

        public Task<IEnumerable<Loan>> GetAllAsync()
        {
            IEnumerable<Loan> copy;
            copy = _loans.Select(l => l).ToList();
            return Task.FromResult(copy);
        }

        public Task<Loan?> GetByIdAsync(int id)
        {
            Loan? loan;
            loan = _loans.FirstOrDefault(l => l.Id == id);
            return Task.FromResult(loan is null ? null : loan);
        }

        public Task<Loan?> MakePaymentAsync(int id, decimal paymentAmount)
        {
            if (paymentAmount <= 0) throw new ArgumentException("Payment amount must be greater than zero.", nameof(paymentAmount));

            var loan = _loans.FirstOrDefault(l => l.Id == id);

            if (loan == null) return Task.FromResult<Loan?>(null);

            if (loan.Status == "paid")
                throw new InvalidOperationException("Loan already paid.");

            if (paymentAmount > loan.CurrentBalance)
                throw new ArgumentException("Payment exceeds remaining balance.", nameof(paymentAmount));

            loan.CurrentBalance -= paymentAmount;
            if (loan.CurrentBalance == 0)
                loan.Status = "paid";

            return Task.FromResult(loan);

        }
    }
}

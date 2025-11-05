using FluentAssertions;
using Fundo.Application.Services;
using Fundo.Domain.Entities;
using Fundo.Domain.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Fundo.Services.Tests.Unit.LoanServices
{
    public class LoanServiceTests
    {
        private readonly Mock<ILoanRepository> _mockRepository;
        private readonly LoanService _service;

        public LoanServiceTests()
        {
            _mockRepository = new Mock<ILoanRepository>();
            _service = new LoanService(_mockRepository.Object);
        }

        [Fact]
        public async Task CreateAsync_WithValidData_ShouldCreateLoanSuccessfully()
        {
            // Arrange
            var amount = 10000m;
            var applicantName = "Jane Doe";
            var expectedLoan = new Loan
            {
                Id = 1,
                Amount = amount,
                CurrentBalance = amount,
                ApplicantName = applicantName,
                Status = "active"
            };

            _mockRepository
                .Setup(x => x.AddAsync(It.IsAny<Loan>()))
                .ReturnsAsync((Loan loan) =>
                {
                    loan.Id = 1;
                    return loan;
                });

            // Act
            var result = await _service.CreateAsync(amount, applicantName);

            // Assert
            result.Should().NotBeNull();
            result.Amount.Should().Be(amount);
            result.CurrentBalance.Should().Be(amount);
            result.ApplicantName.Should().Be(applicantName);
            result.Status.Should().Be("active");
            result.Id.Should().BeGreaterThan(0);

            _mockRepository.Verify(
                x => x.AddAsync(It.Is<Loan>(l =>
                    l.Amount == amount &&
                    l.CurrentBalance == amount &&
                    l.ApplicantName == applicantName &&
                    l.Status == "active"
                )),
                Times.Once
            );
        }

        [Fact]
        public async Task CreateAsync_WithNameContainingSpaces_ShouldTrimName()
        {
            // Arrange
            var amount = 5000m;
            var applicantName = "  Jane Doe  ";

            _mockRepository
                .Setup(x => x.AddAsync(It.IsAny<Loan>()))
                .ReturnsAsync((Loan loan) => loan);

            // Act
            var result = await _service.CreateAsync(amount, applicantName);

            // Assert
            result.ApplicantName.Should().Be("Jane Doe");
            _mockRepository.Verify(
                x => x.AddAsync(It.Is<Loan>(l => l.ApplicantName == "Jane Doe")),
                Times.Once
            );
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public async Task CreateAsync_WithInvalidAmount_ShouldThrowArgumentException(decimal amount)
        {
            // Arrange
            var applicantName = "Jane Doe";

            // Act
            var act = async () => await _service.CreateAsync(amount, applicantName);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Amount must be greater than zero.*")
                .WithParameterName("amount");

            _mockRepository.Verify(x => x.AddAsync(It.IsAny<Loan>()), Times.Never);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task CreateAsync_WithInvalidName_ShouldThrowArgumentException(string applicantName)
        {
            // Arrange
            var amount = 5000m;

            // Act
            var act = async () => await _service.CreateAsync(amount, applicantName);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Applicant name is required.*")
                .WithParameterName("applicantName");

            _mockRepository.Verify(x => x.AddAsync(It.IsAny<Loan>()), Times.Never);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllLoans()
        {
            // Arrange
            var expectedLoans = new List<Loan>
        {
            new Loan { Id = 1, Amount = 5000m, CurrentBalance = 5000m, ApplicantName = "Jane Doe", Status = "active" },
            new Loan { Id = 2, Amount = 10000m, CurrentBalance = 7000m, ApplicantName = "John Doe", Status = "active" },
            new Loan { Id = 3, Amount = 3000m, CurrentBalance = 0m, ApplicantName = "Pedro", Status = "paid" }
        };

            _mockRepository
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(expectedLoans);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().BeEquivalentTo(expectedLoans);
            _mockRepository.Verify(x => x.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_WithNoLoans_ShouldReturnEmptyList()
        {
            // Arrange
            _mockRepository
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<Loan>());

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _mockRepository.Verify(x => x.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WithExistingId_ShouldReturnLoan()
        {
            // Arrange
            var loanId = 1;
            var expectedLoan = new Loan
            {
                Id = loanId,
                Amount = 8000m,
                CurrentBalance = 8000m,
                ApplicantName = "Jane Doe",
                Status = "active"
            };

            _mockRepository
                .Setup(x => x.GetByIdAsync(loanId))
                .ReturnsAsync(expectedLoan);

            // Act
            var result = await _service.GetByIdAsync(loanId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedLoan);
            _mockRepository.Verify(x => x.GetByIdAsync(loanId), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistingId_ShouldReturnNull()
        {
            // Arrange
            var loanId = 999;
            _mockRepository
                .Setup(x => x.GetByIdAsync(loanId))
                .ReturnsAsync((Loan?)null);

            // Act
            var result = await _service.GetByIdAsync(loanId);

            // Assert
            result.Should().BeNull();
            _mockRepository.Verify(x => x.GetByIdAsync(loanId), Times.Once);
        }

        [Fact]
        public async Task MakePaymentAsync_WithValidPayment_ShouldReduceBalance()
        {
            // Arrange
            var loanId = 1;
            var paymentAmount = 3000m;
            var loan = new Loan
            {
                Id = loanId,
                Amount = 10000m,
                CurrentBalance = 10000m,
                ApplicantName = "John Doe",
                Status = "active"
            };

            _mockRepository
                .Setup(x => x.GetByIdAsync(loanId))
                .ReturnsAsync(loan);

            _mockRepository
                .Setup(x => x.UpdateAsync(It.IsAny<Loan>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.MakePaymentAsync(loanId, paymentAmount);

            // Assert
            result.Should().NotBeNull();
            result.CurrentBalance.Should().Be(7000m);
            result.Status.Should().Be("active");

            _mockRepository.Verify(x => x.GetByIdAsync(loanId), Times.Once);
            _mockRepository.Verify(
                x => x.UpdateAsync(It.Is<Loan>(l =>
                    l.Id == loanId &&
                    l.CurrentBalance == 7000m &&
                    l.Status == "active"
                )),
                Times.Once
            );
        }

        [Fact]
        public async Task MakePaymentAsync_WithFullPayment_ShouldSetStatusToPaid()
        {
            // Arrange
            var loanId = 1;
            var paymentAmount = 5000m;
            var loan = new Loan
            {
                Id = loanId,
                Amount = 5000m,
                CurrentBalance = 5000m,
                ApplicantName = "John Doe",
                Status = "active"
            };

            _mockRepository
                .Setup(x => x.GetByIdAsync(loanId))
                .ReturnsAsync(loan);

            _mockRepository
                .Setup(x => x.UpdateAsync(It.IsAny<Loan>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.MakePaymentAsync(loanId, paymentAmount);

            // Assert
            result.Should().NotBeNull();
            result!.CurrentBalance.Should().Be(0m);
            result.Status.Should().Be("paid");

            _mockRepository.Verify(
                x => x.UpdateAsync(It.Is<Loan>(l =>
                    l.CurrentBalance == 0m &&
                    l.Status == "paid"
                )),
                Times.Once
            );
        }

        [Fact]
        public async Task MakePaymentAsync_WithSequenceOfPartialPayments_ShouldAccumulateCorrectly()
        {
            // Arrange
            var loanId = 1;
            var loan = new Loan
            {
                Id = loanId,
                Amount = 10000m,
                CurrentBalance = 10000m,
                ApplicantName = "Jane Doe",
                Status = "active"
            };

            _mockRepository
                .Setup(x => x.GetByIdAsync(loanId))
                .ReturnsAsync(loan);

            _mockRepository
                .Setup(x => x.UpdateAsync(It.IsAny<Loan>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.MakePaymentAsync(loanId, 3000m);
            await _service.MakePaymentAsync(loanId, 2000m);
            var result = await _service.MakePaymentAsync(loanId, 5000m);

            // Assert
            result.Should().NotBeNull();
            result!.CurrentBalance.Should().Be(0m);
            result.Status.Should().Be("paid");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-500)]
        public async Task MakePaymentAsync_WithInvalidAmount_ShouldThrowArgumentException(decimal paymentAmount)
        {
            // Arrange
            var loanId = 1;

            // Act
            var act = async () => await _service.MakePaymentAsync(loanId, paymentAmount);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Payment amount must be greater than zero.*")
                .WithParameterName("paymentAmount");

            _mockRepository.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Never);
            _mockRepository.Verify(x => x.UpdateAsync(It.IsAny<Loan>()), Times.Never);
        }

        [Fact]
        public async Task MakePaymentAsync_WithNonExistingLoan_ShouldReturnNull()
        {
            // Arrange
            var loanId = 999;
            var paymentAmount = 1000m;

            _mockRepository
                .Setup(x => x.GetByIdAsync(loanId))
                .ReturnsAsync((Loan?)null);

            // Act
            var result = await _service.MakePaymentAsync(loanId, paymentAmount);

            // Assert
            result.Should().BeNull();
            _mockRepository.Verify(x => x.GetByIdAsync(loanId), Times.Once);
            _mockRepository.Verify(x => x.UpdateAsync(It.IsAny<Loan>()), Times.Never);
        }

        [Fact]
        public async Task MakePaymentAsync_OnPaidLoan_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var loanId = 1;
            var paymentAmount = 100m;
            var loan = new Loan
            {
                Id = loanId,
                Amount = 5000m,
                CurrentBalance = 0m,
                ApplicantName = "John Doe",
                Status = "paid"
            };

            _mockRepository
                .Setup(x => x.GetByIdAsync(loanId))
                .ReturnsAsync(loan);

            // Act
            var act = async () => await _service.MakePaymentAsync(loanId, paymentAmount);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Loan already paid.");

            _mockRepository.Verify(x => x.GetByIdAsync(loanId), Times.Once);
            _mockRepository.Verify(x => x.UpdateAsync(It.IsAny<Loan>()), Times.Never);
        }

        [Fact]
        public async Task MakePaymentAsync_WithAmountExceedingBalance_ShouldThrowArgumentException()
        {
            // Arrange
            var loanId = 1;
            var paymentAmount = 6000m;
            var loan = new Loan
            {
                Id = loanId,
                Amount = 10000m,
                CurrentBalance = 5000m,
                ApplicantName = "Jane Doe",
                Status = "active"
            };

            _mockRepository
                .Setup(x => x.GetByIdAsync(loanId))
                .ReturnsAsync(loan);

            // Act
            var act = async () => await _service.MakePaymentAsync(loanId, paymentAmount);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Payment exceeds remaining balance.*")
                .WithParameterName("paymentAmount");

            _mockRepository.Verify(x => x.GetByIdAsync(loanId), Times.Once);
            _mockRepository.Verify(x => x.UpdateAsync(It.IsAny<Loan>()), Times.Never);
        }
    }
}

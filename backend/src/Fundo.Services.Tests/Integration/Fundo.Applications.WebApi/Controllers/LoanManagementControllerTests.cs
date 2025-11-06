using FluentAssertions;
using Fundo.Applications.WebApi.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Fundo.Services.Tests.Integration
{
    public class LoanManagementControllerTests : IClassFixture<WebApplicationFactory<Fundo.Applications.WebApi.Startup>>
    {
        private readonly HttpClient _client;

        public LoanManagementControllerTests(WebApplicationFactory<Fundo.Applications.WebApi.Startup> factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task GetBalances_ShouldReturnExpectedResult()
        {
            var response = await _client.GetAsync("/loan");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetById_WithExistingLoan_ShouldReturnOkAndLoan()
        {
            //arrange
            var createRequest = new
            {
                Amount = 7500m,
                ApplicantName = "Test Name"
            };
            var createResponse = await _client.PostAsJsonAsync("/loan", createRequest);
            var createdLoan = await createResponse.Content.ReadFromJsonAsync<LoanResponse>();

            //act
            var response = await _client.GetAsync($"/loan/{createdLoan!.Id}");
            var loan = await response.Content.ReadFromJsonAsync<LoanResponse>();

            //assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            loan.Should().NotBeNull();
            loan!.Id.Should().Be(createdLoan.Id);
            loan.Amount.Should().Be(7500m);
            loan.ApplicantName.Should().Be("Test Name");
        }

        [Fact]
        public async Task GetById_WithNonExistingLoan_ShouldReturnNotFound()
        {
            //act
            var response = await _client.GetAsync("/loan/999999");

            //assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Create_WithValidData_ShouldReturnCreatedLoan()
        {
            //arrange
            var request = new
            {
                Amount = 10000m,
                ApplicantName = "Test Name"
            };

            //act
            var response = await _client.PostAsJsonAsync("/loan", request);
            var loan = await response.Content.ReadFromJsonAsync<LoanResponse>();

            //assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            loan.Should().NotBeNull();
            loan!.Amount.Should().Be(10000m);
            loan.CurrentBalance.Should().Be(10000m);
            loan.ApplicantName.Should().Be("Test Name");
            loan.Status.Should().Be("active");
            loan.Id.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task Create_WithZeroAmount_ShouldReturnBadRequest()
        {
            //arrange
            var request = new
            {
                Amount = 0m,
                ApplicantName = "Test Name"
            };

            //act
            var response = await _client.PostAsJsonAsync("/loan", request);

            //assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Create_WithNegativeAmount_ShouldReturnBadRequest()
        {
            //arrange
            var request = new
            {
                Amount = -1000m,
                ApplicantName = "Test Name"
            };

            //act
            var response = await _client.PostAsJsonAsync("/loan", request);

            //assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Create_WithEmptyName_ShouldReturnBadRequest()
        {
            //arrange
            var request = new
            {
                Amount = 5000m,
                ApplicantName = ""
            };

            //act
            var response = await _client.PostAsJsonAsync("/loan", request);

            //assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task MakePayment_WithValidAmount_ShouldReturnOkAndUpdateBalance()
        {
            //arrange
            var createRequest = new
            {
                Amount = 10000m,
                ApplicantName = "Test Name"
            };
            var createResponse = await _client.PostAsJsonAsync("/loan", createRequest);
            var createdLoan = await createResponse.Content.ReadFromJsonAsync<LoanResponse>();

            var paymentRequest = new
            {
                PaymentAmount = 3000m
            };

            //act
            var response = await _client.PostAsJsonAsync(
                $"/loan/{createdLoan!.Id}/payment",
                paymentRequest
            );
            var updatedLoan = await response.Content.ReadFromJsonAsync<LoanResponse>();

            //assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            updatedLoan.Should().NotBeNull();
            updatedLoan!.CurrentBalance.Should().Be(7000m);
            updatedLoan.Status.Should().Be("active");
        }

        [Fact]
        public async Task MakePayment_WithFullAmount_ShouldSetStatusToPaid()
        {
            //arrange
            var createRequest = new
            {
                Amount = 5000m,
                ApplicantName = "Test Name"
            };
            var createResponse = await _client.PostAsJsonAsync("/loan", createRequest);
            var createdLoan = await createResponse.Content.ReadFromJsonAsync<LoanResponse>();

            var paymentRequest = new
            {
                PaymentAmount = 5000m
            };

            //act
            var response = await _client.PostAsJsonAsync(
                $"/loan/{createdLoan!.Id}/payment",
                paymentRequest
            );
            var updatedLoan = await response.Content.ReadFromJsonAsync<LoanResponse>();

            //assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            updatedLoan.Should().NotBeNull();
            updatedLoan!.CurrentBalance.Should().Be(0m);
            updatedLoan.Status.Should().Be("paid");
        }

        [Fact]
        public async Task MakePayment_WithZeroAmount_ShouldReturnBadRequest()
        {
            //arrange
            var createRequest = new
            {
                Amount = 5000m,
                ApplicantName = "Test Name"
            };
            var createResponse = await _client.PostAsJsonAsync("/loan", createRequest);
            var createdLoan = await createResponse.Content.ReadFromJsonAsync<LoanResponse>();

            var paymentRequest = new
            {
                PaymentAmount = 0m
            };

            //act
            var response = await _client.PostAsJsonAsync(
                $"/loan/{createdLoan!.Id}/payment",
                paymentRequest
            );

            //assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task MakePayment_WithNegativeAmount_ShouldReturnBadRequest()
        {
            //arrange
            var createRequest = new
            {
                Amount = 5000m,
                ApplicantName = "Test Name"
            };
            var createResponse = await _client.PostAsJsonAsync("/loan", createRequest);
            var createdLoan = await createResponse.Content.ReadFromJsonAsync<LoanResponse>();

            var paymentRequest = new
            {
                PaymentAmount = -1000m
            };

            //act
            var response = await _client.PostAsJsonAsync(
                $"/loan/{createdLoan!.Id}/payment",
                paymentRequest
            );

            //assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task MakePayment_WithAmountExceedingBalance_ShouldReturnBadRequest()
        {
            //arrange
            var createRequest = new
            {
                Amount = 5000m,
                ApplicantName = "Test Name"
            };
            var createResponse = await _client.PostAsJsonAsync("/loan", createRequest);
            var createdLoan = await createResponse.Content.ReadFromJsonAsync<LoanResponse>();

            var paymentRequest = new
            {
                PaymentAmount = 6000m
            };

            //act
            var response = await _client.PostAsJsonAsync(
                $"/loan/{createdLoan!.Id}/payment",
                paymentRequest
            );

            //assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task MakePayment_OnAlreadyPaidLoan_ShouldReturnBadRequest()
        {
            //arrange
            var createRequest = new
            {
                Amount = 5000m,
                ApplicantName = "Test Name"
            };
            var createResponse = await _client.PostAsJsonAsync("/loan", createRequest);
            var createdLoan = await createResponse.Content.ReadFromJsonAsync<LoanResponse>();

            await _client.PostAsJsonAsync(
                $"/loan/{createdLoan!.Id}/payment",
                new { PaymentAmount = 5000m }
            );

            var secondPaymentRequest = new
            {
                PaymentAmount = 100m
            };

            //act
            var response = await _client.PostAsJsonAsync(
                $"/loan/{createdLoan.Id}/payment",
                secondPaymentRequest
            );

            //assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task MakePayment_OnNonExistingLoan_ShouldReturnNotFound()
        {
            //arrange
            var paymentRequest = new
            {
                PaymentAmount = 1000m
            };

            //act
            var response = await _client.PostAsJsonAsync(
                "/loan/999999/payment",
                paymentRequest
            );

            //assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
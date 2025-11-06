using Fundo.Application.Interfaces;
using Fundo.Applications.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fundo.Applications.WebApi.Controllers
{
    [ApiController]
    [Route("/loan")]
    public class LoanManagementController : ControllerBase
    {
        private readonly ILoanService _loanService;

        public LoanManagementController(ILoanService loanService)
        {
            _loanService = loanService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoanResponse>>> Get() {
            var loans = await _loanService.GetAllAsync();
            var loansResponse = loans.Select(x => new LoanResponse { Id = x.Id, Amount = x.Amount, CurrentBalance = x.CurrentBalance, ApplicantName = x.ApplicantName, Status = x.Status });
            return Ok(loansResponse);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<LoanResponse>> GetById(int id)
        {
            var loan = await _loanService.GetByIdAsync(id);
            if (loan == null) return NotFound(new { message = "Loan not found." });

            var loanResponse = new LoanResponse { Id = loan.Id, Amount = loan.Amount, CurrentBalance = loan.CurrentBalance, ApplicantName = loan.ApplicantName, Status = loan.Status };

            return Ok(loan);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLoanRequest request)
        {
            try
            {
                var created = await _loanService.CreateAsync(request.Amount, request.ApplicantName);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/payment")]
        public async Task<ActionResult<PaymentResponse>> MakePayment(int id, [FromBody] PaymentRequest request)
        {
            try
            {
                var updated = await _loanService.MakePaymentAsync(id, request.PaymentAmount);
                if (updated == null) return NotFound(new { message = "Loan not found." });

                var response = new PaymentResponse { LoanAmount = updated.Amount, CurrentBalance = updated.CurrentBalance, Status = updated.Status };

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
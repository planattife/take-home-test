using Fundo.Application.Interfaces;
using Fundo.Domain.Entities;
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
        public async Task<IActionResult> Get() {
            var loans = await _loanService.GetAllAsync();
            return Ok(loans);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var loan = await _loanService.GetByIdAsync(id);
            if (loan == null) return NotFound(new { message = "Loan not found." });
            return Ok(loan);
        }

        public class CreateLoanRequest
        {
            public decimal Amount { get; set; }
            public string ApplicantName { get; set; } = string.Empty;
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

        public class PaymentRequest
        {
            public decimal PaymentAmount { get; set; }
        }

        [HttpPost("{id}/payment")]
        public async Task<IActionResult> MakePayment(int id, [FromBody] PaymentRequest request)
        {
            try
            {
                var updated = await _loanService.MakePaymentAsync(id, request.PaymentAmount);
                if (updated == null) return NotFound(new { message = "Loan not found." });
                return Ok(updated);
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
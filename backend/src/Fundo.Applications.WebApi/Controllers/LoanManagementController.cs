using Fundo.Applications.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Fundo.Applications.WebApi.Controllers
{
    [ApiController]
    [Route("/loan")]
    public class LoanManagementController : ControllerBase
    {
        private static readonly List<Loan> _loans = new();
        private static int _nextId = 1;

        [HttpGet]
        public ActionResult<IEnumerable<Loan>> Get() {  
            return Ok(_loans);
        }

        [HttpGet("{id}")]
        public ActionResult<Loan> GetById(int id)
        {
            var loan = _loans.FirstOrDefault(l => l.Id == id);
            if (loan == null)
                return NotFound(new { message = "Loan not found." });

            return Ok(loan);
        }

        [HttpPost]
        public ActionResult<Loan> Create([FromBody] Loan request)
        {
            if (request.Amount <= 0)
                return BadRequest(new { message = "Amount must be greater than zero." });

            if (string.IsNullOrWhiteSpace(request.ApplicantName))
                return BadRequest(new { message = "Applicant name is required." });

            var loan = new Loan
            {
                Id = _nextId++,
                Amount = request.Amount,
                CurrentBalance = request.Amount,
                ApplicantName = request.ApplicantName,
                Status = "active"
            };

            _loans.Add(loan);
            return CreatedAtAction(nameof(GetById), new { id = loan.Id }, loan);
        }

        [HttpPost("{id}/payment")]
        public ActionResult<Loan> MakePayment(int id, [FromBody] PaymentRequest payment)
        {
            var loan = _loans.FirstOrDefault(l => l.Id == id);

            if (loan == null)
                return NotFound(new { message = "Loan not found." });

            if (loan.Status == "paid")
                return BadRequest(new { message = "Loan already paid." });

            if (payment.PaymentAmount <= 0)
                return BadRequest(new { message = "Payment amount must be greater than zero." });

            if (payment.PaymentAmount > loan.CurrentBalance)
                return BadRequest(new { message = "Payment exceeds remaining balance." });

            loan.CurrentBalance -= payment.PaymentAmount;

            if (loan.CurrentBalance == 0)
                loan.Status = "paid";

            return Ok(loan);
        }

        public class PaymentRequest
        {
            public decimal PaymentAmount { get; set; }
        }
    }
}
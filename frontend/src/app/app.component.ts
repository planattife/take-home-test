import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { Loan, LoanService } from './services/loan.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatButtonModule],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  displayedColumns: string[] = [
    'loanAmount',
    'currentBalance',
    'applicant',
    'status',
  ];
  loans: Loan[] = [];


  constructor(private loanService: LoanService) {}

  ngOnInit(): void {
    this.loanService.getLoans().subscribe({
      next: (data) => (this.loans = data),
      error: (err) => console.error('Failed to retrieve loans.', err),
    });
  }

}

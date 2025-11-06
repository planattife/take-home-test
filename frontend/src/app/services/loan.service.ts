import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Loan {
  amount: number;
  currentBalance: number;
  applicantName: string;
  status: string;
}

export interface PaginatedResult<T> {
  data: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

@Injectable({
  providedIn: 'root'
})
export class LoanService {
  private apiUrl = 'http://localhost:54089/loan';

  constructor(private http: HttpClient) {}

  getLoans(pageNumber: number, pageSize: number): Observable<PaginatedResult<Loan>> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize);

    return this.http.get<PaginatedResult<Loan>>(this.apiUrl, { params });
  }
}

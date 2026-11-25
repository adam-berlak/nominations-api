import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  Nomination,
  CreateNominationRequest,
  ScheduleRequest,
  ScheduleResult
} from '../models/nomination';

const API_URL = 'https://nominations-api-production.up.railway.app/api/nominations';

@Injectable({
  providedIn: 'root'
})
export class NominationService {
  constructor(private http: HttpClient) {}

  getAll(): Observable<Nomination[]> {
    return this.http.get<Nomination[]>(API_URL);
  }

  create(request: CreateNominationRequest): Observable<Nomination> {
    return this.http.post<Nomination>(API_URL, request);
  }

  schedule(segment: string, request: ScheduleRequest): Observable<ScheduleResult> {
    return this.http.post<ScheduleResult>(`${API_URL}/schedule/${segment}`, request);
  }
}

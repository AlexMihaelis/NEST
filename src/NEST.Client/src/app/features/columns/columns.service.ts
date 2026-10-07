import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Column } from './column.model';

// Сервис отвечает за HTTP-запросы, связанные с колонками
@Injectable({
  providedIn: 'root'
})
export class ColumnsService {
  // Базовый URL API для работы с колонками конкретной доски
  private readonly apiUrl = `${environment.apiUrl}/api/boards`;

  constructor(private readonly http: HttpClient) {}

  getByBoardId(boardId: string): Observable<Column[]> {
    return this.http.get<Column[]>(`${this.apiUrl}/${boardId}/columns`);
  }
}

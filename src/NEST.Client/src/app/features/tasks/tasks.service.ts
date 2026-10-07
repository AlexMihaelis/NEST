import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Task } from './task.model';

// Сервис отвечает за HTTP-запросы, связанные с задачами
@Injectable({
  providedIn: 'root'
})
export class TasksService {
  // Базовый URL API для работы с задачами колонок
  private readonly apiUrl = `${environment.apiUrl}/api/columns`;

  constructor(private readonly http: HttpClient) {}

  // Получает список задач конкретной колонки с backend
  getByColumnId(columnId: string): Observable<Task[]> {
    return this.http.get<Task[]>(`${this.apiUrl}/${columnId}/tasks`);
  }
}

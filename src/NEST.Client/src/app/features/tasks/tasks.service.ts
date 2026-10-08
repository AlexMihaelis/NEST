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
  private readonly tasksApiUrl = `${environment.apiUrl}/api/tasks`;
  private readonly columnsApiUrl = `${environment.apiUrl}/api/columns`;

  constructor(private readonly http: HttpClient) {}

  // Получает список задач конкретной колонки с backend
  getByColumnId(columnId: string): Observable<Task[]> {
    return this.http.get<Task[]>(`${this.columnsApiUrl}/${columnId}/tasks`);
  }

  // Обновляет существующую задачу на backend
  update(
    taskId: string,
    task: {
      name: string;
      description: string;
      priority: number;
      deadline: string | null;
      isCompleted: boolean;
      taskContextId: string | null;
    }
  ): Observable<void> {
    return this.http.put<void>(`${this.tasksApiUrl}/${taskId}`, task);
  }

  // Удаляет существующую задачу на backend
  delete(taskId: string): Observable<void> {
    return this.http.delete<void>(`${this.tasksApiUrl}/${taskId}`);
  }
}

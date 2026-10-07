import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { Board } from './board.model';

// Сервис отвечает за HTTP-запросы, связанные с досками
// Компоненты будут обращаться к сервису, а не работать с HttpClient напрямую
@Injectable({
  providedIn: "root"
})
export class BoardsService {
  // Базовый URL API берём из environment, чтобы адрес backend можно было менять в зависимости от окружения
  private readonly apiUrl = `${environment.apiUrl}/api/boards`;

  constructor(private readonly http: HttpClient) {}

  // Получает список всех досок с backend
  // HttpClient возвращает Observable, который выдаст массив Board после выполнения HTTP-запроса
  getAll() : Observable<Board[]> {
    return this.http.get<Board[]>(this.apiUrl);
  }

  // Получает одну доску по ее Id
  getById(boardId: string): Observable<Board> {
    return this.http.get<Board>(`${this.apiUrl}/${boardId}`);
  }
}

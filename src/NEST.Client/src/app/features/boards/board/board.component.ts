import { Component, computed, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { forkJoin } from 'rxjs';

import { Board } from '../board.model';
import { BoardsService } from '../boards.service';
import { Column } from '../../columns/column.model';
import { ColumnsService } from '../../columns/columns.service';
import { Task } from '../../tasks/task.model';
import { TasksService } from '../../tasks/tasks.service';

@Component({
  selector: 'app-board',
  templateUrl: './board.component.html',
  styleUrl: './board.component.scss'
})
export class BoardComponent implements OnInit {
  // Храним текущую доску
  // Изначально доска ещё не загружена
  protected readonly board = signal<Board | null>(null);

  // Храним список колонок текущей доски
  protected readonly columns = signal<Column[]>([]);

  // Храним список задач текущей доски
  protected readonly tasks = signal<Task[]>([]);

  // Группируем задачи по идентификатору колонки
  // Для каждой колонки получаем только те задачи, которые принадлежат этой колонке
  protected readonly tasksByColumn = computed(() => {
    const tasks = this.tasks();

    return new Map(
      this.columns().map(column => [
        column.id,
        tasks.filter(task => task.columnId === column.id)
      ])
    );
  });

  // Храним идентификатор доски из URL
  protected boardId = '';

  constructor(
    private readonly route: ActivatedRoute,
    private readonly boardsService: BoardsService,
    private readonly columnsService: ColumnsService,
    private readonly tasksService: TasksService
  ) {}

  // Angular вызывает этот метод после создания компонента
  ngOnInit(): void {
    // Получаем boardId из параметра текущего маршрута
    this.boardId = this.route.snapshot.paramMap.get('boardId') ?? '';

    // Загружаем доску с backend
    this.boardsService.getById(this.boardId).subscribe({
      next: (board) => {
        // Сохраняем полученную доску в signal
        this.board.set(board);
      },
      error: (error) => {
        console.error('Ошибка при получении доски:', error);
      }
    });

    // Загружаем колонки текущей доски с backend
    this.columnsService.getByBoardId(this.boardId).subscribe({
      next: (columns) => {
        // Сохраняем полученные колонки в signal
        this.columns.set(columns);

        // Для каждой колонки загружаем её задачи с backend
        const taskRequests = columns.map(column =>
          this.tasksService.getByColumnId(column.id)
        );

        forkJoin(taskRequests).subscribe({
          next: (tasksByColumn) => {
            // Объединяем задачи всех колонок в один массив
            const tasks = tasksByColumn.flat();

            this.tasks.set(tasks);
          },
          error: (error) => {
            console.error('Ошибка при получении задач:', error);
          }
        });
      },
      error: (error) => {
        console.error('Ошибка при получении колонок:', error);
      }
    });
  }
}

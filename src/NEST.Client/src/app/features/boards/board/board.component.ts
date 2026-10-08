import { Component, computed, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { forkJoin } from 'rxjs';
import { Board } from '../board.model';
import { BoardsService } from '../boards.service';
import { Column } from '../../columns/column.model';
import { ColumnsService } from '../../columns/columns.service';
import { Task } from '../../tasks/task.model';
import { TasksService } from '../../tasks/tasks.service';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-board',
  imports: [ReactiveFormsModule],
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

  // Храним задачу, которую сейчас редактируем
  // Если null — модальное окно закрыто
  protected readonly editingTask = signal<Task | null>(null);

  // Храним задачу, которую сейчас собираемся удалить
  // Если null — модальное окно удаления закрыто
  protected readonly deletingTask = signal<Task | null>(null);

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

        // Для каждой колонки загружаем ее задачи с backend
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

  // Открывает модальное окно и заполняет форму данными выбранной задачи
  protected openEditTask(task: Task): void {
    this.editTaskForm.setValue({
      name: task.name,
      description: task.description,
      priority: task.priority,
      deadline: this.toLocalDateTimeInput(task.deadline),
      isCompleted: task.isCompleted,
      taskContextId: task.taskContextId
    });

    this.editingTask.set(task);
  }

  // Закрывает модальное окно редактирования
  protected closeEditTask(): void {
    this.editingTask.set(null);
  }

  // Форма редактирования задачи
  protected readonly editTaskForm = new FormGroup({
    name: new FormControl('', { nonNullable: true }),
    description: new FormControl('', { nonNullable: true }),
    priority: new FormControl(0, { nonNullable: true }),
    deadline: new FormControl<string | null>(null),
    isCompleted: new FormControl(false, { nonNullable: true }),
    taskContextId: new FormControl<string | null>(null)
  });

  // Сохраняет изменения выбранной задачи на backend.
  protected saveTask(): void {
    const task = this.editingTask();

    if (!task) {
      return;
    }

    const formValue = this.editTaskForm.getRawValue();

    // datetime-local хранит локальное время без часового пояса
    // Перед отправкой преобразуем его в UTC ISO-строку, которую корректно принимает backend и PostgreSQL
    const updateRequest = {
      ...formValue,
      deadline: this.toUtcIso(formValue.deadline)
    };

    this.tasksService.update(task.id, updateRequest).subscribe({
      next: () => {
        // Обновляем задачу локально после успешного сохранения
        this.tasks.update(tasks =>
          tasks.map(currentTask =>
            currentTask.id === task.id
              ? {
                ...currentTask,
                ...updateRequest
              }
              : currentTask
          )
        );

        // Закрываем модальное окно
        this.closeEditTask();
      },
      error: (error) => {
        console.error('Ошибка при обновлении задачи:', error);
      }
    });
  }

  // Открывает модальное окно подтверждения удаления
  protected deleteTask(task: Task): void {
    this.deletingTask.set(task);
  }

  // Закрывает модальное окно подтверждения удаления
  protected closeDeleteTask(): void {
    this.deletingTask.set(null);
  }

  // Удаляет задачу после подтверждения пользователя
  protected confirmDeleteTask(): void {
    const task = this.deletingTask();

    if (!task) {
      return;
    }

    this.tasksService.delete(task.id).subscribe({
      next: () => {
        // Убираем удаленную задачу из локального списка
        this.tasks.update(tasks =>
          tasks.filter(currentTask => currentTask.id !== task.id)
        );

        // Закрываем модальное окно после успешного удаления
        this.closeDeleteTask();
      },
      error: (error) => {
        console.error('Ошибка при удалении задачи:', error);
      }
    });
  }

  // Преобразует дату из backend (UTC) в формат, который понимает input type="datetime-local"
  private toLocalDateTimeInput(value: string | null): string | null {
    if (!value) {
      return null;
    }

    const date = new Date(value);

    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');

    return `${year}-${month}-${day}T${hours}:${minutes}`;
  }

  // Преобразует локальное значение datetime-local в UTC ISO, чтобы backend и PostgreSQL получили корректную дату
  private toUtcIso(value: string | null): string | null {
    if (!value) {
      return null;
    }

    return new Date(value).toISOString();
  }
}

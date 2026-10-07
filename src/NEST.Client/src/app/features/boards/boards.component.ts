import { Component, computed, OnInit, signal } from '@angular/core';
import { Board } from './board.model';
import { BoardsService } from './boards.service';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-boards',
  imports: [RouterLink],
  templateUrl: './boards.component.html',
  styleUrl: './boards.component.scss'
})
export class BoardsComponent implements OnInit {
  // Храним список досок в signal
  // Когда значение signal изменится, Angular автоматически обновит шаблон
  protected readonly boards = signal<Board[]>([]);
  // Текущая страница пагинации
  protected readonly currentPage = signal(1);

  // Количество досок на одной странице
  private readonly pageSize = 6;

  constructor(private readonly boardsService: BoardsService) {}

  // Angular вызывает этот метод после создания компонента
  // Здесь загружаем доски с backend
  ngOnInit(): void {
    this.boardsService.getAll().subscribe({
      next: (boards) => {
        // Обновляем значение signal полученным списком досок
        this.boards.set(boards);
      },
      error: (error) => {
        console.error('Ошибка при получении досок:', error);
      }
    });
  }

  // Доски, которые должны отображаться на текущей странице
  // computed автоматически пересчитывается, когда изменяется boards или currentPage
  protected readonly paginatedBoards = computed(() => {
    const startIndex = (this.currentPage() - 1) * this.pageSize;
    const endIndex = startIndex + this.pageSize;

    return this.boards().slice(startIndex, endIndex);
  });

  // Общее количество страниц
  protected readonly totalPages = computed(() =>
    Math.ceil(this.boards().length / this.pageSize)
  );

  // Переходим на предыдущую страницу
  protected previousPage(): void {
    if (this.currentPage() > 1) {
      this.currentPage.update(page => page - 1);
    }
  }

  // Переходим на следующую страницу
  protected nextPage(): void {
    if (this.currentPage() < this.totalPages()) {
      this.currentPage.update(page => page + 1);
    }
  }

  // Ограничиваем длину названия доски
  // Если название длиннее лимита, обрезаем его и добавляем "..."
  protected truncateName(name: string): string {
    const maxLength = 25;

    return name.length > maxLength
      ? `${name.slice(0, maxLength)}...`
      : name;
  }

  // Ограничиваем длину описания доски
  // Если описание длиннее лимита, обрезаем его и добавляем "..."
  protected truncateDescription(description: string): string {
    const maxLength = 80;

    return description.length > maxLength
      ? `${description.slice(0, maxLength)}...`
      : description;
  }
}

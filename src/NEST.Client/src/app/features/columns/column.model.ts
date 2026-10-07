// Модель колонки на стороне Angular
// Соответствует данным колонки, которые приходят с backend
export interface Column {
  id: string;
  name: string;
  position: number;
  boardId: string;
}

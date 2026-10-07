// Модель задачи на стороне Angular
// Соответствует сущности Task, которую возвращает backend
export interface Task {
  id: string;
  name: string;
  description: string;
  priority: number;
  deadline: string | null;
  isCompleted: boolean;
  createdAt: string;
  updatedAt: string;
  taskContextId: string | null;
  columnId: string;
  position: number;
}

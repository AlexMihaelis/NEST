// Модель доски на стороне Angular
// Соответствует BoardDto из NEST.Application
export interface Board {
  id: string;
  name: string;
  description: string | null;
  userId: string;
  createdAt: string;
  updatedAt: string;
}

import { Routes } from '@angular/router';

import { BoardsComponent } from './features/boards/boards.component';
import { BoardComponent } from './features/boards/board/board.component';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'boards',
    pathMatch: 'full'
  },
  {
    path: 'boards',
    component: BoardsComponent
  },
  {
    path: 'boards/:boardId',
    component: BoardComponent
  }
];

import { Routes } from '@angular/router';
import { Home } from '../features/home/home';
import { MemeberList } from '../features/members/memeber-list/memeber-list';
import { Lists } from '../features/lists/lists';
import { Messages } from '../features/messages/messages';
import { authGuard } from '../core/guards/auth-guard';

export const routes: Routes = [
    {path: '', component: Home},
    {
        path: '',
        runGuardsAndResolvers: 'always',
        children: [
            {path: 'members', component: MemeberList, canActivate: [authGuard]},
            {path: 'members/:id', component: MemeberList},
            {path: 'lists', component: Lists},
            {path: 'messages', component: Messages}
        ]
    },
    {path: '**', component: Home}
];
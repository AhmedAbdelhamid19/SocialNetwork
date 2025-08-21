import { Routes } from '@angular/router';
import { Home } from '../features/home/home';
import { MemeberList } from '../features/members/memeber-list/memeber-list';
import { Lists } from '../features/lists/lists';
import { Messages } from '../features/messages/messages';

export const routes: Routes = [
    {path: '', component: Home},
    {path: 'members', component: MemeberList},
    {path: 'members/:id', component: MemeberList},
    {path: 'lists', component: Lists},
    {path: 'messages', component: Messages},
    {path: '**', component: Home}
];
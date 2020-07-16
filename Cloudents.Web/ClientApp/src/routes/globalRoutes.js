import * as routeName from './routeNames'
export const globalRoutes = [
    {
        path:'*',
        name: routeName.notFound,
        component: () => import(`./../components/notFound2.vue`) 
    }
]
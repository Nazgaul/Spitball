export const globalRoutes = [
    {
        path:'*',
        component: () => import(`./../components/notFound2.vue`) 
    }
]
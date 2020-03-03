export const globalRoutes = [
    {
        path: "test",
        component: () => import('./../components/test.vue')
    },
    {
        path:'*',
        component: () => import(`./../components/notFound2.vue`) 
    }
]
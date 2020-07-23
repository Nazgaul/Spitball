// import {lazyComponent,staticComponents} from './routesUtils.js';
// import store from '../store';

export const itemRoutes = [
    {
        path: "/document/:courseName/:name/:id",
        name: "document",
        components: {
            default: () => import('../components/pages/itemPage/item.vue'),
            banner: () => import('../components/pages/layouts/banner/bannerWrapper.vue'),
            header: () => import('../components/pages/layouts/header/header.vue'),
            // sideMenu: () => import('../components/pages/layouts/sideMenu/sideMenu.vue'),
        },
        props: {
            default: (route) => ({
                id: route.params.id
            }),
        }
        // beforeEnter: (to, from, next) => {
        //     let itemId = to.params?.id;

        //     store.dispatch('documentRequest',itemId)
        //         .then(()=>{
        //             // debugger
        //         })
        //         .catch((err)=>{
        //             // debugger
        //         })
        // }
    },
]
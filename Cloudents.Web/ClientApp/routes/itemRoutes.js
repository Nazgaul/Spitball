// import {lazyComponent,staticComponents} from './routesUtils.js';

export const itemRoutes = [
    {
        path: "/document/:id/:name/",
        name: "document",
        components: {
            default: () => import('../components/pages/itemPage/item.vue'),
            banner: () => import('../components/pages/layouts/banner/bannerWrapper.vue'),
            header: () => import('../components/pages/layouts/header/header.vue'),
        },
        props: {
            default: (route) => ({
                id: route.params.id
            }),
        }
    },
]
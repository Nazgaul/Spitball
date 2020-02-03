import {staticComponents} from './routesUtils.js';

export const itemRoutes = [
    {
        path: "/document/:courseName/:name/:id",
        name: "document",
        components: {
            default: () => import(`../components/pages/itemPage/item.vue`),
            ...staticComponents(['banner', 'header', 'sideMenu'])
        },
        props: {
            default: (route) => ({
                id: route.params.id
            }),
        }
    },
]
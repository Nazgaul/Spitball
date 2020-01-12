import {lazyComponent,staticComponents} from './routesUtils.js';

export const itemRoutes = [
    {
        path: "/document/:courseName/:name/:id",
        name: "document",
        components: {
            default: lazyComponent('pages/itemPage/item'),
            ...staticComponents(['banner', 'header', 'sideMenu'])
        },
        props: {
            default: (route) => ({
                id: route.params.id
            }),
        }
    },
]
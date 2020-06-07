import {staticComponents} from './routesUtils.js';

export const questionRoutes = [
    {
        path: "/question/:id",
        components: {
            default: () => import(`../components/question/question-details/questionDetails.vue`),
            ...staticComponents(['banner', 'header', 'sideMenu'])
        },
        name: "question",
        props: {
            default: (route) => ({
                id: route.params.id
            })
        }
    },
]
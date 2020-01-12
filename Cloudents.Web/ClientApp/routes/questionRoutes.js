import {lazyComponent,staticComponents} from './routesUtils.js';

export const questionRoutes = [
    {
        path: "/question/:id",
        components: {
            default: lazyComponent('question/question-details/questionDetails'),
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
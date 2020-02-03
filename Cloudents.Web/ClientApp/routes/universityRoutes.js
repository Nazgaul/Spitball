import {staticComponents} from './routesUtils.js';

export const universityRoutes = [
    {
        path: "/university/",
        name: "setUniversity",
        children: [
            {
                path: '',
                redirect: 'add',
                meta: {
                    requiresAuth: true
                }
            },
            {
                path: 'add',
                name: 'addUniversity',
                component: () => import(`../components/university/addUniversity/addUniversity.vue`),
                meta: {
                    requiresAuth: true
                }
            },
            {
                path: '*',
                redirect: 'add',
                meta: {
                    requiresAuth: true
                }
            }
        ],
        components: {
            default: () => import(`../components/university/university.vue`),
            ...staticComponents(['banner', 'header', 'sideMenu'])
        },
        meta: {
            requiresAuth: true
        }
    },
]
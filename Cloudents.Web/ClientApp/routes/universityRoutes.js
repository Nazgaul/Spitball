import {lazyComponent,staticComponents} from './routesUtils.js';

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
                component: lazyComponent('university/addUniversity/addUniversity'),
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
            default: lazyComponent('university/university'),
            ...staticComponents(['banner', 'header', 'sideMenu'])
        },
        meta: {
            requiresAuth: true
        }
    },
]
import { staticComponents } from './routesUtils.js';

export const courseRoutes = [
    {
        path: "/courses/",
        name: "setCourse",
        children: [
            {
                path: '',
                redirect: 'edit',
                meta: {
                    requiresAuth: true
                }
            },
            {
                path: 'add',
                name: 'addCourse',
                component: () => import('../components/courses/addCourses/addCourses.vue'),
                meta: {
                    requiresAuth: true
                }
            },
            {
                path: 'edit',
                name: 'editCourse',
                component: () => import('../components/courses/editCourses/editCourses.vue'),
                meta: {
                    requiresAuth: true
                }
            },
            {
                path: '*',
                redirect: 'edit',
                meta: {
                    requiresAuth: true
                }
            }
        ],
        components: {
            default: () => import('../components/courses/courses.vue'),
            ...staticComponents(['banner', 'header', 'sideMenu'])

        },
        meta: {
            requiresAuth: true
        }
    },
]
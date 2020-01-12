import {lazyComponent,staticComponents} from './routesUtils.js';

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
                component: lazyComponent('courses/addCourses/addCourses'),
                meta: {
                    requiresAuth: true
                }
            },
            {
                path: 'edit',
                name: 'editCourse',
                component: lazyComponent('courses/editCourses/editCourses'),
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
            default: lazyComponent('courses/courses'),
            ...staticComponents(['banner', 'header', 'sideMenu'])
        },
        meta: {
            requiresAuth: true
        }
    },
]
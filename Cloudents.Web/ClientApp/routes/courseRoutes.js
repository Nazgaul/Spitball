// import {lazyComponent,staticComponents} from './routesUtils.js';

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
                component: () => import('../components/courses/addCourses/addCourses'),
                meta: {
                    requiresAuth: true
                }
            },
            {
                path: 'edit',
                name: 'editCourse',
                component: () => import('../components/courses/editCourses/editCourses'),
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
            default: () => import('../components/courses/courses'),
            banner: () => import('../components/pages/layouts/banner/bannerWrapper'),
            header: () => import('../components/pages/layouts/header/header'),
            sideMenu: () => import('../components/pages/layouts/sideMenu/sideMenu'),
        },
        meta: {
            requiresAuth: true
        }
    },
]
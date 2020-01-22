import {lazyComponent, staticComponents} from './routesUtils.js';

export const registrationRoutes = [
    {
        path: "/login/",
        name: "login",
        components: {
            default: lazyComponent('pages/authenticationPage/authenticationPage')
        },
        children: [
            {
                path: '',
                name: "login",
                component: lazyComponent('loginPageNEW/components/getStarted')
            },
            // {
            //     path: 'set-email',
            //     name: "setEmail",
            //     component: lazyComponent('loginPageNEW/components/setEmail')
            // },
            // {
            //     path: 'set-password',
            //     name: "setEmail",
            //     component: lazyComponent('loginPageNEW/components/setEmail')
            // },
            // {
            //     path: 'signin',
            //     name: 'signin',
            //     component: lazyComponent('auth/register/registerType/registerType')
            // },
            {
                path: 'resetpassword',
                name: 'resetpassword',
                component: lazyComponent('pages/register/student/registerStudentSchool')
            },
            // {
            //     path: '*',
            //     redirect: 'login'
            // },
        ],
    },

    {
        path: "/register/",
        name: "register",
        components: {
            default: lazyComponent('pages/authenticationPage/authenticationPage')
        },
        children: [
            {
                path: '',
                name: "register",
                component: lazyComponent('loginPageNEW/components/getStarted')
            },
            {
                path: 'type',
                name: 'registerType',
                component: lazyComponent('pages/register/registerType/registerType')
            },
            {
                path: 'student-school',
                name: 'studentSchool',
                component: lazyComponent('pages/register/student/registerStudentSchool')
            },
            {
                path: 'student-college',
                name: 'studentCollege',
                component: lazyComponent('pages/register/student/registerStudentCollege')
            },
            {
                path: 'parent',
                name: 'parent',
                component: lazyComponent('pages/register/parent/registerParent')
            },
        ],
        beforeEnter: (to, from, next) => {
            if(global.isAuth) {
                next(false);
            } else {
                next();
            }
        }
    },

    {
        path: "/student-or-tutor",
        components: {
            default: lazyComponent('studentOrTutor/studentOrTutor'),
            ...staticComponents(['banner', 'header', 'sideMenu'])
        },
        name: "studentTutor",
        meta: {
            requiresAuth: true
        },
        props: {
            default: (route) => ({
                id: route.params.id
            })
        }
    },
]
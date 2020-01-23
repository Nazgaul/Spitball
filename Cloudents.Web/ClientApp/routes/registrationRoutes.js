import {lazyComponent, staticComponents} from './routesUtils.js';

export const registrationRoutes = [
    {
        path: '/login/',
        name: 'login',
        components: {
            default: lazyComponent('pages/authenticationPage/authenticationPage')
        },
        children: [
            {
                path: '',
                name: 'getStarted',
                component: lazyComponent('loginPageNEW/components/getStarted')
            },
            {
                path: 'set-email',
                name: 'setEmail',
                component: lazyComponent('loginPageNEW/components/setEmail')
            },
            {
                path: 'set-password',
                name: 'setPassword',
                component: lazyComponent('loginPageNEW/components/setPassword')
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
        path: '/register/',
        name: 'register',
        components: {
            default: lazyComponent('pages/authenticationPage/authenticationPage')
        },
        children: [
            {
                path: '',
                name: 'getStarted',
                component: lazyComponent('loginPageNEW/components/getStarted')
            },
            {
                path: 'step1',
                name: 'step1',
                component: lazyComponent('loginPageNEW/components/setEmailPassword')
            },
            {
                path: 'email-confirmed',
                name: 'EmailConfirmed',
                component: lazyComponent('loginPageNEW/components/EmailConfirmed')
            },
            {
                path: 'set-phone',
                name: 'setPhone',
                component: lazyComponent('loginPageNEW/components/setPhone')
            },
            {
                path: 'verify-phone',
                name: 'VerifyPhone',
                component: lazyComponent('loginPageNEW/components/VerifyPhone')
            },
            {
                path: 'register-type',
                name: 'registerType',
                component: lazyComponent('pages/authenticationPage/register/registerType/registerType')
            },
            {
                path: 'student-school',
                name: 'studentSchool',
                component: lazyComponent('pages/authenticationPage/register/student/registerStudentSchool'),
                children: [
                    {
                        path: 'course',
                        name: 'registerCourse',
                    },
                    {
                        path: '',
                        redirect: { name: 'registerCourse' } 
                    },
                ]
            },
            {
                path: 'student-college/',
                name: 'studentCollege',
                component: lazyComponent('pages/authenticationPage/register/student/registerStudentCollege'),
                children: [
                    {
                        path: 'university',
                        name: 'registerUniversity',
                        meta: {nextStep: 'course'},
                        component: lazyComponent('pages/authenticationPage/register/registerUniversity/registerUniversity'),
                    },
                    {
                        path: 'course',
                        name: 'registerCourse',
                        component: lazyComponent('pages/authenticationPage/register/registerCourse/registerCourse')
                    },
                    {
                        path: '',
                        redirect: { name: 'registerUniversity' } 
                    },
                ]
            },
            {
                path: 'parent',
                name: 'parent',
                component: lazyComponent('pages/authenticationPage/register/parent/registerParent'),
                children: [
                    {
                        path: 'student-college',
                        name: 'studentCollege',
                    },
                    {
                        path: 'student-college',
                        name: 'studentCollege',
                    },
                ]
            },
            {
                path: 'teacher',
                name: 'teacher',
                component: lazyComponent('pages/authenticationPage/register/parent/registerTeacher'),
                children: [
                    {
                        path: 'step1',
                        name: 'step1',
                    },
                    {
                        path: 'student-college',
                        name: 'studentCollege',
                    },
                ]
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
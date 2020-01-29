import {lazyComponent, staticComponents} from './routesUtils.js';
import * as routename from "./RouteName.js";

//const RegiserEmailConfirmed = 'emailConfirmed';
export const registrationRoutes = [
    {
        path: '/signin',
        components: {
            default: lazyComponent('pages/authenticationPage/authenticationPage')
        },
        children: [
            {
                path: '',
                name: 'login',
                component: lazyComponent('pages/authenticationPage/getStarted')
            },
            {
                path: 'set-email',
                name: 'setEmail',
                component: lazyComponent('pages/authenticationPage/login/setEmail')
            },
            {
                path: 'set-password',
                name: 'setPassword',
                component: lazyComponent('pages/authenticationPage/login/setPassword')
            },
            {
                path: 'forgot-password',
                name: 'forgotPassword',
                component: lazyComponent('pages/authenticationPage/login/forgotPass')
            },
            {
                path: 'reset-password',
                name: 'resetPassword',
                component: lazyComponent('pages/authenticationPage/login/resetPassword')
            },
        ],
        beforeEnter: (to, from, next) => {
            //TODO why do we need this
            if(global.isAuth) {
                next(false);
            } else {
                next();
            }
        }
    },

    {
        path: '/register',
        components: {
            default: lazyComponent('pages/authenticationPage/authenticationPage'),
        },
        children: [
            {
                path: '',
                name: 'register',
                component: lazyComponent('pages/authenticationPage/getStarted')
            },
            {
                path: 'personal-details',
                name: 'setEmailPassword',
                component: lazyComponent('pages/authenticationPage/register/setEmailPassword/setEmailPassword')
            },
            {
                path: 'email-confirmed',
                name: routename.RegiserEmailConfirmed,
                component: lazyComponent('pages/authenticationPage/register/emailConfirmed/EmailConfirmed')
            },
            {
                path: 'set-phone',
                name: 'setPhone',
                component: lazyComponent('pages/authenticationPage/register/setPhone/setPhone')
            },
            {
                path: 'verify-phone',
                name: 'verifyPhone',
                component: lazyComponent('pages/authenticationPage/register/verifyPhone/VerifyPhone')
            },
            {
                path: 'register-type',
                name: 'registerType',
                component: lazyComponent('pages/authenticationPage/register/registerType/registerType')
            },
            {
                path: 'student-school',
                name: 'studentSchool',
                components: {
                    default: lazyComponent('pages/authenticationPage/register/student/registerStudentSchool'),
                    registerButtons: lazyComponent('pages/authenticationPage/register/registerButtons/registerButtons'),
                },
                children: [
                    {
                        path: 'course',
                        name: 'registerCourse',
                        meta: {nextStep: 'feed', backStep: 'registerType'},
                        component: lazyComponent('pages/authenticationPage/register/registerCourse/registerCourse')
                    },
                    {
                        path: '',
                        redirect: { name: 'registerCourse' } 
                    },
                ]
            },
            {
                path: 'student-college',
                name: 'studentCollege',
                components: {
                    default: lazyComponent('pages/authenticationPage/register/student/registerStudentCollege'),
                    registerButtons: lazyComponent('pages/authenticationPage/register/registerButtons/registerButtons'),
                },
                children: [
                    {
                        path: 'university',
                        name: 'registerUniversity',
                        meta: {nextStep: 'registerCourseCollege', backStep: 'registerType'},
                        component: lazyComponent('pages/authenticationPage/register/registerUniversity/registerUniversity'),
                    },
                    {
                        path: 'course',
                        name: 'registerCourseCollege',
                        meta: {nextStep: 'feed', backStep: 'registerUniversity'},
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
                components: {
                    default: lazyComponent('pages/authenticationPage/register/parent/registerParent'),
                    registerButtons: lazyComponent('pages/authenticationPage/register/registerButtons/registerButtons'),
                },
                children: [
                    {
                        path: 'course',
                        name: 'registerCourseParent',
                        meta: {nextStep: 'feed', backStep: 'registerType'},
                    },
                    {
                        path: '',
                        redirect: { name: 'registerCourseParent' } 
                    },
                ]
            },
            {
                path: 'teacher',
                name: 'teacher',
                components: {
                    default: lazyComponent('pages/authenticationPage/register/parent/registerTeacher'),
                }
            },
        ],
        beforeEnter: (to, from, next) => {
            if(global.isAuth) {
                //TODO why do we need this
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
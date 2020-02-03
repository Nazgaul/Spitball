import {lazyComponent, staticComponents} from './routesUtils.js';
import * as routeName from "./RouteName.js";

export const registrationRoutes = [
    {
        path: '/signin',
        components: {
            default: lazyComponent('pages/authenticationPage/authenticationPage')
        },
        children: [
            {
                path: '',
                name: routeName.Login,
                component: lazyComponent('pages/authenticationPage/getStarted')
            },
            {
                path: 'set-email',
                name: routeName.LoginSetEmail,
                component: lazyComponent('pages/authenticationPage/login/setEmail')
            },
            {
                path: 'set-password',
                name: routeName.LoginSetPassword,
                component: lazyComponent('pages/authenticationPage/login/setPassword')
            },
            {
                path: 'email-confirmed',
                name: routeName.LoginEmailConfirmed,
                component: lazyComponent('pages/authenticationPage/register/emailConfirmed/emailConfirmed'),

            },
            {
                path: 'forgot-password',
                name: routeName.LoginForgotPassword,
                component: lazyComponent('pages/authenticationPage/login/forgotPass')
            },
            {
                path: 'reset-password',
                name: routeName.LoginResetPassword,
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
                name: routeName.Register,
                component: lazyComponent('pages/authenticationPage/getStarted')
            },
            {
                path: 'personal-details',
                name: routeName.RegisterSetEmailPassword,
                component: lazyComponent('pages/authenticationPage/register/setEmailPassword/setEmailPassword')
            },
            {
                path: 'email-confirmed',
                name: routeName.RegisterEmailConfirmed,
                component: lazyComponent('pages/authenticationPage/register/emailConfirmed/emailConfirmed')
            },
            {
                path: 'set-phone',
                name: routeName.RegisterSetPhone,
                component: lazyComponent('pages/authenticationPage/register/setPhone/setPhone')
            },
            {
                path: 'verify-phone',
                name: routeName.RegisterVerifyPhone,
                component: () => import(`../components/pages/authenticationPage/register/verifyPhone/VerifyPhone.vue`) //lazyComponent('Pages/authenticationPage/register/verifyPhone/verifyPhone')
            },
            {
                path: 'type',
                name: routeName.RegisterType,
                component: lazyComponent('pages/authenticationPage/register/registerType/registerType')
            },
            {
                path: 'student-school',
                name: routeName.RegisterStudentSchool,
                component: lazyComponent('pages/authenticationPage/register/student/registerStudentSchool'),
                children: [
                    {
                        path: 'course',
                        name: routeName.RegisterCourse,
                        meta: {nextStep: 'feed', backStep: routeName.RegisterType},
                        component: lazyComponent('pages/authenticationPage/register/registerCourse/registerCourse')
                    },
                    {
                        path: '',
                        redirect: { name: routeName.RegisterCourse } 
                    },
                ]
            },
            {
                path: 'student-college',
                name: routeName.RegisterStudentCollege,
                component: lazyComponent('pages/authenticationPage/register/student/registerStudentCollege'),
                children: [
                    {
                        path: 'university',
                        name: routeName.RegisterUniversity,
                        meta: {nextStep: routeName.RegisterCourseCollege, backStep: routeName.RegisterType},
                        component: lazyComponent('pages/authenticationPage/register/registerUniversity/registerUniversity'),
                    },
                    {
                        path: 'course',
                        name: routeName.RegisterCourseCollege,
                        meta: {nextStep: 'feed', backStep: routeName.RegisterUniversity},
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
                name: routeName.RegisterParent,
                component: lazyComponent('pages/authenticationPage/register/parent/registerParent'),
                children: [
                    {
                        path: 'course',
                        name: routeName.RegisterCourseParent,
                        meta: {nextStep: 'feed', backStep: routeName.RegisterType},
                    },
                    {
                        path: '',
                        redirect: { name: 'registerCourseParent' } 
                    },
                ]
            },
            // V2 register  teacher
            // {
            //     path: 'teacher',
            //     name: 'teacher',
            //     components: {
            //         default: lazyComponent('pages/authenticationPage/register/parent/registerTeacher'),
            //     }
            // },
        ],
        // beforeEnter: (to, from, next) => {
        //     debugger
        //     if(global.isAuth) {
        //         //TODO why do we need this
        //         next(false);
        //     } else {
        //         next();
        //     }
        // }
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
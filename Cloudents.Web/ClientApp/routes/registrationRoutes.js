import { staticComponents} from './routesUtils.js';
import * as routeName from "./routeNames.js";

export const registrationRoutes = [
    {
        path: '/signin',
        components: {
            default: () => import(`../components/pages/authenticationPage/authenticationPage.vue`)
        },
        children: [
            {
                path: '',
                name: routeName.Login,
                component: () => import(`../components/pages/authenticationPage/getStarted.vue`)
            },
            {
                path: 'set-email',
                name: routeName.LoginSetEmail,
                component: () => import(`../components/pages/authenticationPage/login/setEmail.vue`)
            },
            {
                path: 'set-password',
                name: routeName.LoginSetPassword,
                component: () => import(`../components/pages/authenticationPage/login/setPassword.vue`)
            },
            {
                path: 'email-confirmed',
                name: routeName.LoginEmailConfirmed,
                component:() => import(`../components/pages/authenticationPage/register/emailConfirmed/emailConfirmed2.vue`) //() => import(`../components/${path}.vue`)('pages/authenticationPage/register/emailConfirmed/emailConfirme21234234234d'),

            },
            {
                path: 'forgot-password',
                name: routeName.LoginForgotPassword,
                component: () => import(`../components/pages/authenticationPage/login/forgotPass.vue`)
            },
            {
                path: 'reset-password',
                name: routeName.LoginResetPassword,
                component: () => import(`../components/pages/authenticationPage/login/resetPassword.vue`)
            },
        ],
        beforeEnter: (to, from, next) => {
            //TODO why do we need this
            if(global.isAuth) {
                next('/feed');
            } else {
                next();
            }
        }
    },

    {
        path: '/register',
        components: {
            default: () => import(`../components/pages/authenticationPage/authenticationPage.vue`)
        },
        children: [
            {
                path: '',
                name: routeName.Register,
                component: () => import(`../components/pages/authenticationPage/getStarted.vue`)
            },
            {
                path: 'personal-details',
                name: routeName.RegisterSetEmailPassword,
                component: () => import(`../components/pages/authenticationPage/register/setEmailPassword/setEmailPassword.vue`)
            },
            {
                path: 'email-confirmed',
                name: routeName.RegisterEmailConfirmed,
                component: () => import(`../components/pages/authenticationPage/register/emailConfirmed/emailConfirmed2.vue`)
            },
            {
                path: 'set-phone',
                name: routeName.RegisterSetPhone,
                component: () => import(`../components/pages/authenticationPage/register/setPhone/setPhone.vue`)
            },
            {
                path: 'verify-phone',
                name: routeName.RegisterVerifyPhone,
                component: () => import(`../components/pages/authenticationPage/register/verifyPhone/verifyPhone2.vue`) //() => import(`../components/${path}.vue`)('Pages/authenticationPage/register/verifyPhone/verifyPhone')
            },
            {
                path: 'type',
                name: routeName.RegisterType,
                component: () => import(`../components/pages/authenticationPage/register/registerType/registerType.vue`)
            },
            {
                path: 'student-school',
                name: routeName.RegisterStudentSchool,
                component: () => import(`../components/pages/authenticationPage/register/student/registerStudentSchool.vue`),
                children: [
                    {
                        path: 'course',
                        name: routeName.RegisterCourse,
                        meta: {nextStep: 'feed', backStep: routeName.RegisterType, dynamicClass: true},
                        component: () => import(`../components/pages/authenticationPage/register/registerCourse/registerCourse.vue`)
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
                component: () => import(`../components/pages/authenticationPage/register/student/registerStudentCollege.vue`),
                children: [
                    {
                        path: 'university',
                        name: routeName.RegisterUniversity,
                        meta: {nextStep: routeName.RegisterCourseCollege, backStep: routeName.RegisterType, dynamicClass: true},
                        component: () => import(`../components/pages/authenticationPage/register/registerUniversity/registerUniversity.vue`),
                    },
                    {
                        path: 'course',
                        name: routeName.RegisterCourseCollege,
                        meta: {nextStep: 'feed', backStep: routeName.RegisterUniversity, dynamicClass: true},
                        component: () => import(`../components/pages/authenticationPage/register/registerCourse/registerCourse.vue`)
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
                component: () => import(`../components/pages/authenticationPage/register/parent/registerParent.vue`),
                children: [
                    {
                        path: 'course',
                        name: routeName.RegisterCourseParent,
                        meta: {nextStep: {name: routeName.Feed,query:{filter:'Tutor'}}, backStep: routeName.RegisterType, dynamicClass: true},
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
            //         default: () => import(`../components/${path}.vue`)('pages/authenticationPage/register/parent/registerTeacher'),
            //     }
            // },
        ],
        // removed restriction to register routes
        // beforeEnter: (to, from, next) => {
        //     if(global.isAuth) {
        //         //TODO why do we need this
        //         next('/feed');
        //     } else {
        //         next();
        //     }
        // }
    },
]
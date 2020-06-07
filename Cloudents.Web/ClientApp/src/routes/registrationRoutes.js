import * as routeName from "./routeNames.js";

export const registrationRoutes = [
    {
        path: '/signin',
        components: {
            default: () => import(`../components/pages/authenticationPage/authenticationPage.vue`)
        },
        children: [
            // {
            //     path: '',
            //     name: routeName.Login,
            //     component: () => import(`../components/pages/authenticationPage/getStarted.vue`)
            // },
            // {
            //     path: 'set-email',
            //     name: routeName.LoginSetEmail,
            //     component: () => import(`../components/pages/authenticationPage/login/setEmail.vue`)
            // },
            // {
            //     path: 'set-password',
            //     name: routeName.LoginSetPassword,
            //     component: () => import(`../components/pages/authenticationPage/login/setPassword.vue`)
            // },
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
        path: '/register2',
        components: {
            default: () => import(`../components/pages/authenticationPage/authenticationPage2.vue`)
        },
        beforeEnter: (to, from, next) => {
            if(global.isAuth) {
                next('/');
            } else {
                next();
            }
        }
    }
]
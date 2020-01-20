import {lazyComponent,staticComponents} from './routesUtils.js';

export const registrationRoutes = [
    {
        path: "/register",
        alias: ['/signin', '/resetpassword'],
        components: {
            default: lazyComponent('pages/authenticationPage/authenticationPage')
        },
        name: "registration",
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

import { staticComponents } from './routesUtils.js';
import * as routeName from "./routeNames.js";
import store from '../store';

export const marketingRoutes = [
    {
        path: "/marketing",
        name: routeName.Marketing,
        components: {
            default: () => import('../components/pages/marketing/marketing.vue'),
            ...staticComponents(['banner', 'header', 'sideMenu']),
        },
        meta: {
            requiresAuth: true
        },
        beforeEnter: (to, from, next) => {
            if(store.getters.getUserLoggedInStatus && store.getters.accountUser.isTutor){
                next()
                return
            }
            next('/')
        }
    },
    {
        path: "/coupon",
        name: routeName.Coupon,
        components: {
            default: () => import('../components/pages/marketing/coupon.vue'),
            ...staticComponents(['banner', 'header', 'sideMenu']),
        },
        meta: {
            requiresAuth: true
        },
        beforeEnter: (to, from, next) => {
            if(store.getters.getUserLoggedInStatus && store.getters.accountUser.isTutor){
                next()
                return
            }
            next('/')
        }
    }
]

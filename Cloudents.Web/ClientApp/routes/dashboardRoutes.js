import { staticComponents } from './routesUtils.js';
import store from '../store';
import * as routeName from "./routeNames.js";

const dashboardPages = {
   default: () => import('../components/pages/dashboardPage/dashboardPage.vue'),
   ...staticComponents(['banner', 'header', 'sideMenu'])
};

export const dashboardRoutes = [
   {
       path: "/my-followers",
       components: dashboardPages,
       name: "myFollowers",
       props: {
           default: (route) => ({
               component: route.name,
           })
       },
       meta: {
           requiresAuth: true,
           showMobileFooter: true,
       },
   },
   {
       path: "/my-sales",
       components: dashboardPages,
       name: "mySales",
       props: {
           default: (route) => ({
               component: route.name,
           })
       },
       meta: {
           requiresAuth: true,
           showMobileFooter: true,
       },
       beforeEnter: (to, from, next) => {
            if(store.getters.accountUser?.isSold){
                next()
                return
            }
            next('/')
        },
   },
   {
       path: "/my-content",
       components: dashboardPages,
       name: "myContent",
       props: {
           default: (route) => ({
               component: route.name,
           })
       },
       meta: {
           requiresAuth: true,
           showMobileFooter: true,
       },
   },
   {
       path: "/my-purchases",
       components: dashboardPages,
       name: "myPurchases",
       props: {
           default: (route) => ({
               component: route.name,
           })
       },
       meta: {
           requiresAuth: true,
           showMobileFooter: true,
       },
   },
   {
        path: "/study-rooms",
        name: "myStudyRooms",
        components: {
            default: () => import(`../components/pages/dashboardPage/myStudyRooms/myStudyRooms.vue`),
            ...staticComponents(['banner', 'header', 'sideMenu'])
        },
        meta: {
            type: 'private',
            requiresAuth: true,
            showMobileFooter: true,
        },
    },
    {
        path: "/study-rooms-broadcast",
        name: "myStudyRoomsBroadcast",
        components: {
            default: () => import(`../components/pages/dashboardPage/myStudyRooms/myStudyRooms.vue`),
            ...staticComponents(['banner', 'header', 'sideMenu'])
        },
        props: {
            default: (route) => ({
                component: route.name,
            }),
        },
        meta: {
            type: 'broadcast',
            requiresAuth: true,
            showMobileFooter: true,
        },
    },
    {
        path: "/my-calendar",
        components: dashboardPages,
        name: "myCalendar",
        props: {
            default: (route) => ({
                component: route.name,
            })
        },
        meta: {
            requiresAuth: true,
            showMobileFooter: true,
        },
    },
    {
        path: '/dashboard',
        name: 'dashboardTeacher',
        components: {
            default: () => import('../components/pages/dashboardPage/dashboardTeacher/dashboard.vue'),
            ...staticComponents(['banner', 'header', 'sideMenu'])
        },
        beforeEnter: (to, from, next) => {
            if(store.getters.getIsTeacher){
                next()
                return
            }
            // Redirect to root
            next('/')
        },
        meta: {
            showMobileFooter: true,
            requiresAuth: true,

        },
    },
    // {
    //     path: "/my-coupons",
    //     name: routeName.MyCoupons,
    //     components: {
    //         default: () => import('../components/pages/dashboardPage/myCoupons/myCoupons.vue'),
    //         ...staticComponents(['banner', 'header', 'sideMenu']),
    //     },
    //     beforeEnter: (to, from, next) => {
    //         if(store.getters.getIsTeacher){
    //             next()
    //             return
    //         }
    //         // Redirect to root
    //         next('/')
    //     },
    //     meta: {
    //         showMobileFooter: true,
    //         requiresAuth: true,

    //     }
    // },
    {
        path: "/wallet",
        redirect: { name: routeName.MySales }
    }
]
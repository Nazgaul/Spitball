import { staticComponents } from './routesUtils.js';
import store from '../store';

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
       components: dashboardPages,
       name: "myStudyRooms",
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
            if(store.getters.getUserLoggedInStatus2 && store.getters.accountUser.isTutor){
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
    {
        path: "/wallet",
        components: {
            default: () => import('../components/wallet/wallet.vue'),
            ...staticComponents(['banner', 'header', 'sideMenu'])

        },
        name: "wallet",
        meta: {
            requiresAuth: true
        },
    },
]
// import {lazyComponent,staticComponents} from './routesUtils.js';

const dashboardPages = {
   default: () => import('../components/pages/dashboardPage/dashboardPage.vue'),
   banner: () => import('../components/pages/layouts/banner/bannerWrapper.vue'),
   header: () => import('../components/pages/layouts/header/header.vue'),
   sideMenu: () => import('../components/pages/layouts/sideMenu/sideMenu.vue'),
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
        path: "/wallet",
        components: {
            default: () => import('../components/wallet/wallet.vue'),
            banner: () => import('../components/pages/layouts/banner/bannerWrapper.vue'),
            header: () => import('../components/pages/layouts/header/header.vue'),
            sideMenu: () => import('../components/pages/layouts/sideMenu/sideMenu.vue'),
        },
        name: "wallet",
        meta: {
            requiresAuth: true
        },
    },
]
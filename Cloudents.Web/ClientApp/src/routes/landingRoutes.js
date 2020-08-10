import {staticComponents} from './routesUtils.js';
import store from '../store'
import * as routeNames from "./routeNames.js";

export const landingRoutes = [
    {
        path: "/",
        name: routeNames.HomePage,
        components: {
            default: () => import(`../components/pages/homePage/homePage.vue`),
            ...staticComponents(['header', 'footer']),
        },
        beforeEnter: (to, from, next) => {    
            if(store.getters.getUserLoggedInStatus){
                let nextRoute = {name: routeNames.Learning};
                if(store.getters.getIsTeacher){
                    nextRoute = {name: routeNames.Dashboard};
                }
                next({name: nextRoute.name,query: to.query});
                return;
            }
            next();
        },
    },
    {
        // this route is when user try login from homePage, fix navigation duplicate and render logic of beforeEnter
        path: "/loginRedirect",
        name: routeNames.LoginRedirect,
        beforeEnter: (to, from, next) => {
            if(store.getters.getUserLoggedInStatus){
                let nextRoute = {name: routeNames.Learning};
                if(store.getters.getIsTeacher){
                    nextRoute = {name: routeNames.Dashboard};
                }
                next({name: nextRoute.name,query: to.query});
                return;
            }
            next("/");
        },
    },
    // {
    //     path: "/learn",
    //     name: routeNames.Learning,
    //     components: {
    //         default: () => import(`../components/landingPage/learn.vue`),
    //         ...staticComponents(['header', 'footer']),
    //     },
    //     beforeEnter: (to, from, next) => {
    //         if(store.getters.getUserLoggedInStatus){
    //             let nextRoute = {name: routeNames.Feed};
    //             if(store.getters.getIsTeacher){
    //                 nextRoute = {name: routeNames.Dashboard};
    //             }
    //             next({name: nextRoute.name,query: to.query});
    //             return;
    //         }
    //         next();
    //     },
        
    // },
    {
        path: "/tutor-list/:course?",
        name: "tutorLandingPage",
        beforeEnter: (to, from, next) => {
            next({name: routeNames.Learning, params : to.params});
        }
    },
    {
        path: "/learn/:course?",
        name: routeNames.Learning,
        components: {
            default: () => import(`../components/tutorLandingPage/tutorLandingPage.vue`),
            ...staticComponents(['header', 'footer']),
        },
        meta: {
            showMobileFooter: true, 
            headerSlot: (global.country == 'IN')? 'phoneNumberSlot' : '',
        }
    }
]
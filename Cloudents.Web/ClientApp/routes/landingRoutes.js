import {staticComponents} from './routesUtils.js';
import store from '../store'
import * as routeNames from "./routeNames.js";

export const landingRoutes = [
    {
        path: "/",
        // name: "landingPage",
        components: {
            default: () => import(`../components/landingPage/pages/homePage.vue`),
            ...staticComponents(['banner', 'header', 'footer']),
        },
        meta:{
            headerSlot: (global.siteName === 'frymo')? '': 'becomeTutorSlot',
        },
        beforeEnter: (to, from, next) => {    
            if(store.getters.getUserLoggedInStatus){
                let nextRoute = {name: routeNames.Feed};
                if(store.getters.accountUser.userType === 'Teacher'){
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
                let nextRoute = {name: routeNames.Feed};
                if(store.getters.accountUser.userType === 'Teacher'){
                    nextRoute = {name: routeNames.Dashboard};
                }
                next({name: nextRoute.name,query: to.query});
                return;
            }
            next();
        },
    },
    {
        path: "/tutor-list/:course?",
        name: routeNames.TutorList,
        components: {
            default: () => import(`../components/tutorLandingPage/tutorLandingPage.vue`),
            ...staticComponents(['banner', 'header', 'footer']),
        },
        meta: {
            showMobileFooter: true, 
            headerSlot: (global.country == 'US')? '':'phoneNumberSlot',
        }
    }
]
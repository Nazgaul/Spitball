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
                if(store.getters.accountUser.userType === 'Teacher' && store.getters.accountUser.isTutor){
                    nextRoute = {name: routeNames.Dashboard};
                }
                if(store.getters.accountUser.userType === 'Parent'){
                    nextRoute = {name: routeNames.TutorList};
                }
                next(nextRoute);
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
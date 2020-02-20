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
                if(store.getters.accountUser.userType === 'Parent'){
                    nextRoute = {name: routeNames.Feed,query:{filter:'Tutor'}};
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




    // {
    //     path: "/",
    //     name: "landingPage",
    //     components: {
    //         default: () => import(`../components/pages/landingPage/landingPage.vue`),
    //         ...staticComponents(['banner', 'header', 'footer']),
    //     },

    //     beforeEnter: (to, from, next) => {        
    //         if(store.getters.getUserLoggedInStatus){
    //             let nextRoute;
    //             if(store.getters.accountUser.userType === 'Teacher'){
    //                 nextRoute = {name: routeNames.Dashboard};
    //             }
    //             if(store.getters.accountUser.userType === 'Parent'){
    //                 nextRoute = {name: routeNames.Feed,query:{filter:'Tutor'}};
    //             }
    //             if(store.getters.accountUser.userType === 'UniversityStudent' || store.getters.accountUser.userType === 'HighSchoolStudent'){
    //                 nextRoute = {name: routeNames.Feed};
    //             }  
    //             next(nextRoute);
    //         }else{
    //             next();
    //         }
    //     },
    //     children:[
    //         {
    //             path: '',
    //             component: () => import(`../components/landingPage/pages/homePage.vue`),
    //             meta:{
    //                 headerSlot: (global.siteName === 'frymo')? '': 'becomeTutorSlot',
    //             }
    //         },
    //         {
    //             path: "/tutor-list/:course?",
    //             name: "tutorLandingPage",
    //             components: {
    //                 default: () => import(`../components/tutorLandingPage/tutorLandingPage.vue`)
    //             },
    //             meta: {
    //                 showMobileFooter: true, 
    //                 headerSlot: (global.country == 'US')? '':'phoneNumberSlot',
    //             }
    //         }
            
    //     ]
    // },
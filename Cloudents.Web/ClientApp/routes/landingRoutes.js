import {staticComponents} from './routesUtils.js';

export const landingRoutes = [
    {
        path: "/",
        name: "landingPage",
        components: {
            default: () => import(`../components/pages/landingPage/landingPage.vue`),
            ...staticComponents(['banner', 'header', 'footer']),
        },
        children:[
            {
                path: '',
                component: () => import(`../components/landingPage/pages/homePage.vue`),
                meta:{
                    headerSlot: (global.siteName === 'frymo')? '': 'becomeTutorSlot',
                }
            },
            {
                path: "/tutor-list/:course?",
                name: "tutorLandingPage",
                components: {
                    default: () => import(`../components/tutorLandingPage/tutorLandingPage.vue`)
                },
                meta: {
                    showMobileFooter: true, 
                    headerSlot: (global.country == 'US')? '':'phoneNumberSlot',
                }
            }
            
        ]
    },
]
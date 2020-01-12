import {lazyComponent,staticComponents} from './routesUtils.js';

export const landingRoutes = [
    {
        path: "/",
        name: "landingPage",
        components: {
            default: lazyComponent('pages/landingPage/landingPage'),
            ...staticComponents(['banner', 'header', 'footer']),
        },
        children:[
            {
                path: '',
                component: lazyComponent('landingPage/pages/homePage')
            },
            {
                path: "/tutor-list/:course?",
                name: "tutorLandingPage",
                components: {
                    default: lazyComponent('tutorLandingPage/tutorLandingPage')
                },
                meta: {
                    showMobileFooter: true, 
                }
            }
            
        ]
    },
]
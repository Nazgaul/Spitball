
import { staticComponents } from './routesUtils.js';
import * as routeName from "./routeNames.js";
import store from '../store';

export const marketingRoutes = [
    {
        path: "/marketing",
        components: {
            default: () => import('../components/pages/marketingOld/marketing.vue'),
            ...staticComponents(['banner', 'header', 'sideMenu']),
        },
        children: [
            {
                path: '',
                name: routeName.Marketing,
                components: {
                    marketing: () => import('../components/pages/marketingOld/marketingPage/marketingPage.vue'),
                }
            },
            {
                path: 'promote',
                components: {
                    promote: () => import('../components/pages/marketingOld/promote/promote.vue'),
                },
                children: [
                    {
                        path: '',
                        name: routeName.MarketingPromote,
                        components: {
                            stepper: () => import('../components/pages/marketingOld/promote/promoteStteper.vue')
                        }
                    }
                ]
            }
        ],
        beforeEnter: (to, from, next) => {
            if(store.getters.getUserLoggedInStatus && store.getters.accountUser.isTutor){
                next()
                return
            }
            // Redirect to root
            next('/')
        },
        meta: {
            requiresAuth: true
        }
    }
]

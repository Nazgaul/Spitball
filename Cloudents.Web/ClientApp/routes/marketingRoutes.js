
import { staticComponents } from './routesUtils.js';
import * as routeName from "./routeNames.js";

export const marketingRoutes = [
    {
        path: "/marketing",
        components: {
            default: () => import('../components/pages/marketing/marketing.vue'),
            ...staticComponents(['banner', 'header', 'sideMenu']),
        },
        children: [
            {
                path: '',
                name: routeName.Marketing,
                components: {
                    marketing: () => import('../components/pages/marketing/marketingPage/marketingPage.vue'),
                }
            },
            {
                path: 'promote',
                name: routeName.MarketingPromote,
                components: {
                    promote: () => import('../components/pages/marketing/marketingPromote/marketingPromote.vue'),
                }
            }
        ],
        meta: {
            requiresAuth: true
        }
    }
]

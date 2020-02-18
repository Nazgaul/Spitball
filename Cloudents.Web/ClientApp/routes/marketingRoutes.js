
import { staticComponents } from './routesUtils.js';
import * as routeName from "./routeNames.js";

export const marketingRoutes = [
    {
        path: "/marketing",
        name: routeName.Marketing,
        components: {
            default: () => import('../components/pages/marketing/marketing.vue'),
            ...staticComponents(['banner', 'header', 'sideMenu'])
        },
        meta: {
            requiresAuth: true
        }
    }
]

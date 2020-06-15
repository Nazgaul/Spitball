import {staticComponents} from './routesUtils.js';
import * as routeName from './routeNames'

export const feedRoutes = [
    {
        path: "/feed",
        name: routeName.Feed,
        components: {
            // default: () => import('../components/results/feeds/Feeds.vue'),
            default: () => import('../views/feed.vue'),
            ...staticComponents(['banner','header', 'sideMenu'])
        },
        meta: {
            isAcademic: true,
            showMobileFooter: true,
            analytics: {
                pageviewTemplate(route) {
                    return {
                        title: route.path.slice(1).charAt(0).toUpperCase() + route.path.slice(2),
                        path: route.path,
                        location: window.location.href
                    };
                }
            }
        }
    },
]
import {staticComponents} from './routesUtils.js';

const feedPage = {
    default: () => import('../components/results/feeds/Feeds.vue'),
    ...staticComponents(['banner', 'header', 'sideMenu'])
};
export const feedRoutes = [
    {
        path: "/" + 'feed',
        name: "feed",
        components: feedPage,
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
import {lazyComponent,staticComponents} from './routesUtils.js';

function dynamicPropsFn(route) {
    let newName = route.path.slice(1);

    return {
        name: newName,
        query: route.query,
        params: route.params
    };
}
const resultProps = {
    default: dynamicPropsFn,
};
const feedPage = {
    default: lazyComponent('results/feeds/Feeds'),
    ...staticComponents(['banner', 'header', 'sideMenu'])
};
export const feedRoutes = [
    {
        path: "/" + 'feed',
        name: "feed",
        components: feedPage,
        props: resultProps,
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
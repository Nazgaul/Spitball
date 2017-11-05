import HomePage from "./components/home/home.vue";
import * as RouteTypes from "./routeTypes";
import showItem from "./components/preview/Item.vue";
//import resultPageHeader from "./components/header/header.vue";
//import resultPageNavbar from "./components/navbar/TheNavbar.vue";

const resultContent = () => import("./components/results/Result.vue");
const resultPageHeader = () => import(/* webpackChunkName: "group-foo"*/"./components/header/header.vue");
const resultPageNavbar = () => import(/* webpackChunkName: "group-foo"*/"./components/navbar/TheNavbar.vue");
const bookDetails = () => import("./components/results/ResultBookDetails.vue");
//const showItem = () => import("./components/preview/Item.vue");
const foodDetails = () => import("./components/results/ResultFoodDetails.vue");
const notFound = () => import("./components/results/notFound.vue");
//const settings = () => import("./components/settings/settings.vue");
const settings = () => import("./components/settings/searchItem.vue");
function dynamicPropsFn(route) {
    return {
        name: route.path.slice(1),
        query: route.query,
        filterOptions: route.query.filter || 'all',
        sort: route.query.sort || 'relevance',
        userText: route.params.q,
        params: route.params,
        hasExtra: route.path.slice(1).includes('food')
    }
}
function dynamicDetailsPropsFn(route) {
    return {
        name: route.name,
        query: route.query,
        filterOptions: 'all',
        sort: 'price',
        id: route.params.id,
        params: route.params
    }
}

const resultPage = { navbar: resultPageNavbar, default: resultContent, header: resultPageHeader }
const bookDetailsPage = { navbar: resultPageNavbar, default: bookDetails, header: resultPageHeader }
const foodDetailsPage = { navbar: resultPageNavbar, default: foodDetails, header: resultPageHeader }
const notFoundPage = { navbar: resultPageNavbar, header: resultPageHeader, default: notFound }
const resultProps = { default: dynamicPropsFn, navbar: (route) => ({ userText: route.query.q }), header: (route) => ({ userText: route.query.q, name: route.path.slice(1) }) }
const notFoundProps = { navbar: (route) => ({ userText: "" }), header: (route) => ({ userText: "", name: route.path.slice(1) }) }
const bookDetailsProps = { ...resultProps, default: dynamicDetailsPropsFn, header: (route) => ({ userText: route.query.q, name: 'book' }) }
const foodDetailsProps = { ...resultProps, default: (route) => ({ position: { lat: Number.parseFloat(route.query.lat, 10), lng: Number.parseFloat(route.query.lng, 10) } }), header: (route) => ({ userText: route.query.q, name: 'food' }) }
export const routes = [
    {
        path: "/", component: HomePage, name: "home"
    },

    {
        path: "/result", name: "result", alias: [
            '/' + RouteTypes.questionRoute, '/' + RouteTypes.flashcardRoute,
            '/' + RouteTypes.notesRoute, '/' + RouteTypes.tutorRoute, '/' + RouteTypes.bookRoute,
            '/' + RouteTypes.jobRoute, '/' + RouteTypes.foodRoute
        ], components: resultPage, props: resultProps, meta: { fetch: true, isUpdated: false }
    },
    { path: "/book/:type/:id", name: RouteTypes.bookDetailsRoute, components: bookDetailsPage, props: bookDetailsProps },
    { path: "/food/show", name: RouteTypes.foodDetailsRoute, components: foodDetailsPage, props: foodDetailsProps },
    {
        path: "/not-found", name: "notFound", components: notFoundPage, alias: [
            '/' + RouteTypes.postRoute, '/' + RouteTypes.uploadRoute, '/' + RouteTypes.chatRoute,
            '/' + RouteTypes.createFlashcard, '/' + RouteTypes.coursesRoute, '/' + RouteTypes.likesRoute
        ]
    },
    { path: "/" + RouteTypes.settingsRoute, name: RouteTypes.settingsRoute, component: settings, props: { searchApi:'getUniversities',type:'university'}},
    { path: "/item/:id", name: "item", component: showItem,props:true},
];
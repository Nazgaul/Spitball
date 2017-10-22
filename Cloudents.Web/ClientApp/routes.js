import HomePage from "./components/home/home.vue";
import * as RouteTypes from "./routeTypes";


const resultContent = () => import(/* webpackChunkName: "group-foo" */ "./components/results/Result.vue");
const resultPageHeader = () => import(/* webpackChunkName: "group-foo" */ "./components/header/header.vue");
const resultPageNavbar = () => import(/* webpackChunkName: "group-foo" */ "./components/general/TheNavbar.vue");
const bookDetails = () => import("./components/results/ResultBookDetails.vue");
const notFound = () => import("./components/results/notFound.vue");
function dynamicPropsFn(route) {
    return {
        name: route.path.slice(1),
        query: route.query,
        filterOptions: route.query.filter || 'all',
        sort: route.query.sort || 'relevance',
        userText: route.params.q,
        params: route.params
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
function homePropsFn(route) {
    return {
        metaType: route.meta.searchType,
        metaText: route.meta.userText
    }
}
const resultPage = { navbar: resultPageNavbar, default: resultContent, header: resultPageHeader }
const bookDetailsPage = { navbar: resultPageNavbar, default: bookDetails, header: resultPageHeader }
const notFoundPage = { navbar: resultPageNavbar, header: resultPageHeader, default: notFound }
const resultProps = { default: dynamicPropsFn, navbar: (route) => ({ userText: route.query.q }), header: (route) => ({ userText: route.query.q,name:route.path.slice(1) }) }
const notFoundProps = {  navbar: (route) => ({ userText: "" }), header: (route) => ({ userText: "",name:route.path.slice(1) }) }
const bookDetailsProps = { ...resultProps, default: dynamicDetailsPropsFn, header: (route) => ({ userText: route.query.q, name: 'book' })}
export const routes = [
    {
        path: "/", component: HomePage, name: "home", meta: { searchType: 'ask' }, props: homePropsFn
    },

    {
        path: "/result", name: "result", alias: [
            '/' + RouteTypes.questionRoute, '/' + RouteTypes.flashcardRoute,
            '/' + RouteTypes.notesRoute, '/' + RouteTypes.tutorRoute, '/' + RouteTypes.bookRoute,
            '/' + RouteTypes.jobRoute, '/'+RouteTypes.foodRoute
        ], components: resultPage, props: resultProps, meta: { fetch: true, isUpdated: false }
    },
    { path: "/book/:type/:id", name: RouteTypes.bookDetailsRoute, components: bookDetailsPage, props: bookDetailsProps },
    {
        path: "/not-found", name: "notFound", components: notFoundPage, alias: [
            '/' + RouteTypes.postRoute, '/' + RouteTypes.uploadRoute, '/' + RouteTypes.chatRoute,
            '/' + RouteTypes.createFlashcard, '/' + RouteTypes.coursesRoute, '/' + RouteTypes.likesRoute,
            '/' + RouteTypes.settingsRoute
        ]
    }



];
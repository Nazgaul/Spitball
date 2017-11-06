﻿import HomePage from "./components/home/home.vue";
import * as RouteTypes from "./routeTypes";
import showItem from "./components/preview/Item.vue";

const resultContent = () => import("./components/results/Result.vue");
const bookDetails = () => import("./components/results/ResultBookDetails.vue");
//const showItem = () => import("./components/preview/Item.vue");
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

const resultPage = {  default: resultContent }
const bookDetailsPage = { default: bookDetails }
const notFoundPage = { default: notFound }
const resultProps = { default: dynamicPropsFn}
const bookDetailsProps = { ...resultProps, default: dynamicDetailsPropsFn }
export const routes = [
    {
        path: "/", component: HomePage, name: "home", meta: { showHeader: false, showSidebar: false }
    },

    {
        path: "/result", name: "result", alias: [
            '/' + RouteTypes.questionRoute, '/' + RouteTypes.flashcardRoute,
            '/' + RouteTypes.notesRoute, '/' + RouteTypes.tutorRoute, '/' + RouteTypes.bookRoute,
            '/' + RouteTypes.jobRoute, '/' + RouteTypes.foodRoute
        ], components: resultPage, props: resultProps, meta: {showHeader:true,showSidebar:true}
    },
    { path: "/book/:type/:id", name: RouteTypes.bookDetailsRoute, components: bookDetailsPage, props: bookDetailsProps, meta: { pageName: 'book',showHeader: true, showSidebar: true } },
    {
        path: "/not-found", name: "notFound", components: notFoundPage, alias: [
            '/' + RouteTypes.postRoute, '/' + RouteTypes.uploadRoute, '/' + RouteTypes.chatRoute,         
            '/' + RouteTypes.createFlashcard, '/' + RouteTypes.coursesRoute, '/' + RouteTypes.likesRoute
        ], meta: { showHeader: true, showSidebar: true }
    },
    { path: "/" + RouteTypes.settingsRoute, name: RouteTypes.settingsRoute, component: settings, props: { searchApi: 'getUniversities', type: 'university' }, meta: { pageName: RouteTypes.settingsRoute, showHeader: true, showSidebar: true }},
    { path: "/item/:university/:courseId/:courseName/:id/:itemName", name: "item", component: showItem, props: true, meta: { pageName: RouteTypes.notesRoute, showHeader: true, showSidebar: false }},
];
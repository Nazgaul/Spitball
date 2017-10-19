import HomePage from "./components/home/home.vue";
import * as RouteTypes from "./routeTypes";


const SectionsPage = () => import("./components/pages/page.vue");
const resultPage = () => import("./components/pages/results/Result.vue");
const bookDetails = () => import("./components/pages/results/ResultBookDetails.vue");
function dynamicPropsFn(route) {
    return {
        name: route.name,
        query: route.query,
        filterOptions: route.query.filter || 'all',
        sort: route.query.sort || 'relevance',
        id: route.params.id,
        userText: route.params.q,
        params: route.params
    }
}
function homePropsFn(route) {
    return {
        metaType: route.meta.searchType,
        metaText: route.meta.userText
    }
}

export const routes = [
    { path: "/", component: HomePage, name: "home", meta: {searchType: 'ask' }, props: homePropsFn },
    {
        path: "/result", component: SectionsPage, name: "result", meta
        , children: [
            { path: "ask", name: RouteTypes.questionRoute, component: resultPage,  props: dynamicPropsFn },
            { path: "flashcard", name: RouteTypes.flashcardRoute, component: resultPage,  props: dynamicPropsFn },
            { path: "note", name: RouteTypes.notesRoute, component: resultPage,  props: dynamicPropsFn},
            { path: "tutor", name: RouteTypes.tutorRoute, component: resultPage, props: dynamicPropsFn },
            {path: "book", name: RouteTypes.bookRoute, component: resultPage, props: dynamicPropsFn},
            { path: "food", name: RouteTypes.foodRoute, component: resultPage, meta: { location: true }, props: dynamicPropsFn },
            { path: "job", name: RouteTypes.jobRoute, component: resultPage, props: dynamicPropsFn },
            { path: 'book/:type/:id', name: RouteTypes.bookDetailsRoute, component: bookDetails, props: dynamicPropsFn },
            { path: "not-found", name: RouteTypes.uploadRoute, component: resultPage },
            { path: "not-found", name: RouteTypes.postRoute, component: resultPage },
            { path: "not-found", name: RouteTypes.createFlashcard, component: resultPage },
            { path: "not-found", name: RouteTypes.chatRoute, component: resultPage },
            { path: "not-found", name: RouteTypes.coursesRoute, component: resultPage },
            { path: "not-found", name: RouteTypes.likesRoute, component: resultPage },
            { path: "not-found", name: RouteTypes.settingsRoute, component: resultPage }
        ]
    }

];
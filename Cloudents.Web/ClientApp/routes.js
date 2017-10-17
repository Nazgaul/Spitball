import HomePage from "./components/home/home.vue";
import * as RouteTypes from "./routeTypes";



const SectionsPage = () => import("./components/pages/page.vue");
const resultPage = () => import("./components/pages/results/Result.vue");
const bookDetails = () => import("./components/pages/results/ResultBookDetails.vue");
const meta = { userText: "" }
const resultMeta = { ...meta, load: "newResultPage" }
function dynamicPropsFn(route) {
    return {
        name: route.name,
        currentQuery: route.query,
        filterOptions: route.query.filter || 'all',
        sort: route.query.sort || 'relevance'
    }
}
export const routes = [
    { path: "/", component: HomePage, name: "home", meta: Object.assign({}, { ...meta, searchType: '' }) },
    {
        path: "/result", component: SectionsPage, name: "result", meta
        , children: [
            { path: "ask", name: RouteTypes.questionRoute, component: resultPage, meta: { userText: "", load: "newResultPage" }, props: dynamicPropsFn },
            { path: "flashcard", name: RouteTypes.flashcardRoute, component: resultPage, meta: { ...resultMeta }, props: dynamicPropsFn },
            { path: "note", name: RouteTypes.notesRoute, component: resultPage, meta: { ...resultMeta }, props: dynamicPropsFn},
            { path: "tutor", name: RouteTypes.tutorRoute, component: resultPage, meta: Object.assign({}, resultMeta),props: dynamicPropsFn },
            {path: "book", name: RouteTypes.bookRoute, component: resultPage, meta: Object.assign({}, resultMeta), props: dynamicPropsFn},
            { path: "food", name: RouteTypes.foodRoute, component: resultPage, meta: Object.assign({ location: true }, resultMeta), props: dynamicPropsFn },
            { path: "job", name: RouteTypes.jobRoute, component: resultPage, meta: Object.assign({}, resultMeta), props: dynamicPropsFn },
            { path: 'book/:type/:id', name: RouteTypes.bookDetailsRoute, component: bookDetails, props: dynamicPropsFn, meta: { load: "bookDetails" } },
            { path: "not-found", name: RouteTypes.uploadRoute, component: resultPage },
            { path: "not-found", name: RouteTypes.postRoute, component: resultPage },
            { path: "not-found", name: RouteTypes.createFlashcard, component: resultPage},
            { path: "not-found", name: RouteTypes.chatRoute, component: resultPage },
            { path: "not-found", name: RouteTypes.coursesRoute, component: resultPage},
            { path: "not-found", name: RouteTypes.likesRoute, component: resultPage},
            { path: "not-found", name: RouteTypes.settingsRoute, component: resultPage}


        ]
    }

];
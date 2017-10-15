import HomePage from "./components/home/home.vue";
import * as RouteTypes from "./routeTypes";



const SectionsPage = () => import("./components/pages/page.vue");
const resultPage = () => import("./components/pages/results/Result.vue");
const meta = { userText: "" }
const resultMeta = { ...meta, load: "newResultPage" }

export const routes = [
    { path: "/", component: HomePage, name: "home", meta: Object.assign({}, { ...meta, searchType: '' }) },
    {
        path: "/result", component: SectionsPage, name: "result", meta
        , children: [
            { path: "ask", name: RouteTypes.questionRoute, component: resultPage, meta: { userText: "", load: "newResultPage" } },
            { path: "flashcard", name: RouteTypes.flashcardRoute, component: resultPage, meta: { ...resultMeta } },
            { path: "note", name: RouteTypes.notesRoute, component: resultPage, meta: { ...resultMeta } },
            { path: "tutor", name: RouteTypes.tutorRoute, component: resultPage, meta: Object.assign({}, resultMeta) },
            { path: "book", name: RouteTypes.bookRoute, component: resultPage, meta: Object.assign({}, resultMeta) },
            { path: "food", name: RouteTypes.foodRoute, component: resultPage, meta: Object.assign({ location: true }, resultMeta) },
            { path: "job", name: RouteTypes.jobRoute, component: resultPage, meta: Object.assign({}, resultMeta) },

            { path: "not-found", name: RouteTypes.uploadRoute, component: resultPage, meta: Object.assign({ location: true }, resultMeta) },
            { path: "not-found", name: RouteTypes.postRoute, component: resultPage, meta: Object.assign({ location: true }, resultMeta) },
            { path: "not-found", name: RouteTypes.createFlashcard, component: resultPage, meta: Object.assign({ location: true }, resultMeta) },
            { path: "not-found", name: RouteTypes.chatRoute, component: resultPage, meta: Object.assign({ location: true }, resultMeta) },
            { path: "not-found", name: RouteTypes.coursesRoute, component: resultPage, meta: Object.assign({ location: true }, resultMeta) },
            { path: "not-found", name: RouteTypes.likesRoute, component: resultPage, meta: Object.assign({ location: true }, resultMeta) },
            { path: "not-found", name: RouteTypes.settingsRoute, component: resultPage, meta: Object.assign({ location: true }, resultMeta) }


        ]
    }

];
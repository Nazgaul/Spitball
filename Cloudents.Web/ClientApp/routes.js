import HomePage from "./components/home/home.vue";
import * as RouteTypes from "./routeTypes";



const SectionsPage = () => import("./components/pages/page.vue");
//const AskPage = () => import("./components/pages/results/ResultAsk.vue");
//const FlashcardPage = () => import( "./components/pages/results/ResultFlashcard.vue");
//const NotePage = () => import( "./components/pages/results/ResultNote.vue");
//const TutorPage = () => import("./components/pages/results/ResultTutor.vue");
const resultPage = () => import("./components/pages/results/Result.vue");
//const BookPage = () => import("./components/pages/results/ResultBook.vue");
//const PurchasePage = () => import("./components/pages/results/ResultPurchase.vue");
const meta = { userText: "" }
const resultMeta = {...meta,load:"newResultPage"}

export const routes = [
    { path: "/", component: HomePage, name: "home", meta: Object.assign({}, { ...meta,searchType:''}) },
    {
        path: "/result", component: SectionsPage, name: "result" ,meta
        , children: [
            { path: "ask", name: RouteTypes.questionRoute, component: resultPage, meta: Object.assign({}, resultMeta)},
            { path: "flashcard", name: RouteTypes.flashcardRoute, component: resultPage, meta: Object.assign({}, resultMeta) },
            { path: "note", name: RouteTypes.notesRoute, component: resultPage, meta: Object.assign({}, resultMeta) },
            { path: "tutor", name: RouteTypes.tutorRoute, component: resultPage, meta: Object.assign({}, resultMeta) },
            { path: "book", name: RouteTypes.bookRoute, component: resultPage, meta: Object.assign({}, resultMeta) },
            { path: "food", name: RouteTypes.foodRoute, component: resultPage, meta: Object.assign({location:true}, resultMeta) },
            { path: "job", name: RouteTypes.jobRoute, component: resultPage, meta: Object.assign({}, resultMeta) }
        ]
    }

];
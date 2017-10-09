import HomePage from "./components/home/home.vue";
import * as RouteTypes from "./routeTypes";



const SectionsPage = () => import("./components/pages/page.vue");
const AskPage = () => import("./components/pages/results/ResultAsk.vue");
const FlashcardPage = () => import( "./components/pages/results/ResultFlashcard.vue");
const NotePage = () => import( "./components/pages/results/ResultNote.vue");
const TutorPage = () => import("./components/pages/results/ResultTutor.vue");
const JobPage = () => import("./components/pages/results/ResultJob.vue");
const BookPage = () => import("./components/pages/results/ResultBook.vue");
const PurchasePage = () => import("./components/pages/results/ResultPurchase.vue");
const meta = { userText: "" }
const resultMeta = {...meta,load:"newResultPage"}

export const routes = [
    { path: "/", component: HomePage, name: "home", meta: Object.assign({}, { ...meta,searchType:''}) },
    {
        path: "/result", component: SectionsPage, name: "result" ,meta
        , children: [
            { path: "ask", name: RouteTypes.questionRoute, component: AskPage, meta: Object.assign({}, resultMeta)},
            { path: "flashcard", name: RouteTypes.flashcardRoute, component: FlashcardPage, meta: Object.assign({}, resultMeta) },
            { path: "note", name: RouteTypes.notesRoute, component: NotePage, meta: Object.assign({}, resultMeta) },
            { path: "tutor", name: RouteTypes.tutorRoute, component: TutorPage, meta: Object.assign({}, resultMeta) },
            { path: "book", name: RouteTypes.bookRoute, component: BookPage, meta: Object.assign({}, resultMeta) },
            { path: "purchase", name: RouteTypes.purchaseRoute, component: PurchasePage, meta: Object.assign({}, resultMeta) },
            { path: "job", name: RouteTypes.jobRoute, component: JobPage, meta: Object.assign({}, resultMeta) }
        ]
    }

];
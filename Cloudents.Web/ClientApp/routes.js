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
const meta = {userText:""}

export const routes = [
    { path: "/", component: HomePage, name: "home",meta },
    {
        path: "/result", component: SectionsPage, name: "result" ,meta
        , children: [
            { path: "ask", name: RouteTypes.questionRoute, component: AskPage,meta},
            { path: "flashcard", name: RouteTypes.flashcardRoute, component: FlashcardPage, meta },
            { path: "note", name: RouteTypes.notesRoute, component: NotePage, meta },
            { path: "tutor", name: RouteTypes.tutorRoute, component: TutorPage, meta },
            { path: "book", name: RouteTypes.bookRoute, component: BookPage, meta },
            { path: "purchase", name: RouteTypes.purchaseRoute, component: PurchasePage, meta },
            { path: "job", name: RouteTypes.jobRoute, component: JobPage, meta }
        ]
    }

];
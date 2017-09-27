import HomePage from "./components/home/home.vue";


export const questionRoute = "ask";
export const flashcardRoute = "flashcard";
export const notesRoute = "note";
export const tutorRoute = "tutor";
export const bookRoute = "book";
export const purchaseRoute = "purchase";
export const jobRoute = "job";
export const postChatRoute = "chat";

const SectionsPage = () => import("./components/pages/page.vue");
const AskPage = () => import("./components/pages/results/ask.vue");
const FlashcardPage = () => import( "./components/pages/results/flashcard.vue");
const NotePage = () => import( "./components/pages/results/note.vue");
const TutorPage = () => import("./components/pages/results/tutor.vue");
const JobPage = () => import("./components/pages/results/job.vue");
const BookPage = () => import("./components/pages/results/book.vue");
const PurchasePage = () => import("./components/pages/results/purchase.vue");
const meta = {userText:""}

export const routes = [
    { path: "/", component: HomePage, name: "home",meta },
    {
        path: "/result", component: SectionsPage, name: "result" ,meta
        , children: [
            { path: "ask", name: questionRoute, component: AskPage,meta},
            { path: "flashcard", name: flashcardRoute, component: FlashcardPage, meta },
            { path: "note", name: notesRoute, component: NotePage, meta },
            { path: "tutor", name: tutorRoute, component: TutorPage, meta },
            { path: "book", name: bookRoute, component: BookPage, meta },
            { path: "purchase", name: purchaseRoute, component: PurchasePage, meta },
            { path: "job", name: jobRoute, component: JobPage, meta }
        ]
    }

];
import HomePage from "./components/home/home.vue";


export const questionRoute = "ask";
export const flashcardRoute = "flashcard";
export const notesRoute = "note";
export const tutorRoute = "tutor";
export const bookRoute = "book";
export const purchaseRoute = "purchase";
export const jobRoute = "job";
export const postChatRoute = "chat";

const SectionsPage = () => import("./components/sections/sections.vue");
const AskPage = () => import("./components/sections/results/ask.vue");
const FlashcardPage = () => import( "./components/sections/results/flashcard.vue");
const NotePage = () => import( "./components/sections/results/note.vue");
const TutorPage = () => import("./components/sections/results/tutor.vue");
const JobPage = () => import("./components/sections/results/job.vue");
const BookPage = () => import("./components/sections/results/book.vue");
const PurchasePage = () => import("./components/sections/results/purchase.vue");


export const routes = [
    { path: "/", component: HomePage, name: "home" },
    {
        path: "/result", component: SectionsPage, name: "result"
        , children: [
            { path: "ask", name: questionRoute, component: AskPage},
            { path: "flashcard", name: flashcardRoute, component: FlashcardPage },
            { path: "note", name: notesRoute, component: NotePage },
            { path: "tutor", name: tutorRoute, component: TutorPage },
            { path: "book", name: bookRoute, component: BookPage },
            { path: "purchase", name: purchaseRoute, component: PurchasePage },
            { path: "job", name: jobRoute, component: JobPage }
        ]
    }

];
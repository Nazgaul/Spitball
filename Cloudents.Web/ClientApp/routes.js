import HomePage from "./components/home/home.vue";
import SectionsPage from "./components/sections/sections.vue";
import AskPage from "./components/sections/results/ask.vue";
import FlashcardPage from "./components/sections/results/flashcard.vue";
import NotePage from "./components/sections/results/note.vue";
import TutorPage from "./components/sections/results/tutor.vue";
import JobPage from "./components/sections/results/job.vue";
import BookPage from "./components/sections/results/book.vue";
import PurchasePage from "./components/sections/results/purchase.vue";

export const questionRoute = "ask";
export const flashcardRoute = "flashcard";
export const notesRoute = "note";
export const tutorRoute = "tutor";
export const bookRoute = "book";
export const purchaseRoute = "purchase";
export const jobRoute = "job";

//TODO: need to add
export const postChatRoute = "chat";

export const routes = [
    { path: "/", component: HomePage, name: "home" },
    {
        path: "/result", component: SectionsPage, name: "result"
        , children: [
            { path: "ask", name: questionRoute, component: AskPage },
            { path: "flashcard", name: flashcardRoute, component: FlashcardPage },
            { path: "note", name: notesRoute, component: NotePage },
            { path: "tutor", name: tutorRoute, component: TutorPage },
            { path: "book", name: bookRoute, component: BookPage },
            { path: "purchase", name: purchaseRoute, component: PurchasePage },
            { path: "job", name: jobRoute, component: JobPage }
        ]
    },

];
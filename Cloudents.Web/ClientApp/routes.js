import HomePage from "./components/home/home.vue";
import SectionsPage from "./components/sections/sections.vue";
import AskPage from "./components/sections/results/ask.vue";
import FlashcardPage from "./components/sections/results/flashcard.vue";
import NotePage from "./components/sections/results/note.vue";
import TutorPage from "./components/sections/results/tutor.vue";
import JobPage from "./components/sections/results/job.vue";
import BookPage from "./components/sections/results/book.vue";
import PurchasePage from "./components/sections/results/purchase.vue";

export const routes = [
    { path: "/", component: HomePage, name: "home" },
    {
        path: "/result", component: SectionsPage, name: "result"
        , children: [
            { path: "ask", name: "ask", component: AskPage },
            { path: "flashcard", name: "flashcard", component: FlashcardPage },
            { path: "notes", name: "notes", component: NotePage },
            { path: "tutor", name: "tutor", component: TutorPage },
            { path: "book", name: "book", component: BookPage },
            { path: "purchase", name: "purchase", component: PurchasePage },
            { path: "job", name: "job", component: JobPage }
        ]
    },

];
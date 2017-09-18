import Vue from 'vue';
import App from './components/app/app.vue';
import HomePage from "./components/home/home.vue";
import SectionsPage from "./components/sections/sections.vue";
import askPage from "./components/sections//results/ask.vue";
import flashcardPage from "./components/sections/results/flashcard.vue";
import NotePage from "./components/sections/results/note.vue";
import TutorPage from "./components/sections/results/tutor.vue";
import JobPage from "./components/sections/results/job.vue";
import BookPage from "./components/sections/results/book.vue";
import PurchasePage from "./components/sections/results/purchase.vue";

import VueRouter from 'vue-router';
import VueResource from 'vue-resource';
import VueCarousel from 'vue-carousel';
Vue.use(VueRouter);
Vue.use(VueResource);
Vue.use(VueCarousel);

const routes = [
    { path: "/", component: HomePage },
    {
        path: "/result", component: SectionsPage, activeClass: 'active',
        children: [
            { path: 'ask', component: askPage,name:'ask' },
            { path: 'flashcard', component: flashcardPage, name: 'flashcard' },
            { path: 'notes', component: NotePage, name: 'notes' },
            { path: 'tutor', component: TutorPage, name: 'tutor' },
            { path: 'job', component: JobPage, name: 'job' },
            { path: 'book', component: BookPage, name: 'book' },
            { path: 'purchase', component: PurchasePage, name: 'purchase' }
        ]
    }


];

const router = new VueRouter({
    mode: "history",
    routes: routes
});
new Vue({
    el: "#app",
    router: router,
    render: h => h(App)
})
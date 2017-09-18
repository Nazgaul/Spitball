import Vue from 'vue';
import App from './components/app/app.vue';
import state from './store/flow';
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
import Vuex from 'vuex';
import * as route from './routes';
Vue.use(VueRouter);
Vue.use(VueResource);
Vue.use(VueCarousel);
Vue.use(Vuex);


console.log(state);
const store = new Vuex.Store({
    modules: { state }
    //mutations: {
    //    increment(state) {
    //        state.count++;
    //        console.log(store.state.count)
    //    }
    //}
});

const router = new VueRouter({
    mode: "history",
    routes: route.routes
});
new Vue({
    el: "#app",
    router: router,
    render: h => h(App),
    store: store
})
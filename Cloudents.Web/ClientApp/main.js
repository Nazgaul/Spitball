import Vue from 'vue';
import App from './components/app/app.vue';
import HomePage from "./components/home/home.vue";
import SectionsPage from "./components/sections/sections.vue";

import VueRouter from 'vue-router';
import VueResource from 'vue-resource';
import VueCarousel from 'vue-carousel';
import Vuex from 'vuex';
Vue.use(VueRouter);
Vue.use(VueResource);
Vue.use(VueCarousel);
Vue.use(Vuex);


const routes = [
    { path: "/", component: HomePage, name: "home" },
    { path: "/result", component: SectionsPage, name: "result" }
];
export const store = new Vuex.Store({
    state: {
        count: 0
    },
    mutations: {
        increment(state) {
            state.count++;
            console.log(store.state.count)
        }
    }
});

const router = new VueRouter({
    mode: "history",
    routes: routes
});
new Vue({
    el: "#app",
    router: router,
    render: h => h(App),
    store: store
})
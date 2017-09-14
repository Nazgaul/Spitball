import Vue from 'vue';
import App from './components/app/app.vue';
import HomePage from "./components/home/home.vue";
import SectionsPage from "./components/sections/sections.vue";

import VueRouter from 'vue-router'
import VueResource from 'vue-resource'
Vue.use(VueRouter);
Vue.use(VueResource);


const routes = [
    { path: "/", component: HomePage },
    { path: "/result", component: SectionsPage }


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
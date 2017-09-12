import Vue from 'vue'
import App from './components/app/app.vue.html'
import HomePage from "./components/home/home.vue.html"
import VueRouter from 'vue-router'
import VueResource from 'vue-resource'

Vue.use(VueRouter);
Vue.use(VueResource);


const routes = [
    { path: "/", component: HomePage }
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
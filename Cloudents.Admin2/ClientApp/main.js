import Vue from "vue";
import VueRouter from "vue-router"
import App from './App.vue'
import { routes } from './routes'

Vue.config.productionTip = false
Vue.use(VueRouter)

console.log(routes);

const router = new VueRouter({
    mode: "history",
    routes: routes,
});

const app = new Vue({
    router: router,
    render: h => h(App),
});

export {
    app,
    router
};
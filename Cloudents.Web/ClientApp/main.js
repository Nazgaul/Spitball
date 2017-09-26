import Vue from 'vue';
import App from './components/app/app.vue';
import store from './store';
import searchStore from './store/search';


import VueRouter from 'vue-router';
import VueResource from 'vue-resource';
import VueCarousel from 'vue-carousel';
import * as route from './routes';
Vue.use(VueRouter);
Vue.use(VueResource);
Vue.use(VueCarousel);




const router = new VueRouter({
    mode: "history",
    routes: route.routes
});
new Vue({
    el: "#app",
    router: router,
    render: h => h(App),
    store
})
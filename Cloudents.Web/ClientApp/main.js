import Vue from 'vue';
import App from './components/app/app.vue';
import store from './store';
import scroll from './components/helpers/infinateScroll.vue'
import VueRouter from 'vue-router';
import VueCarousel from 'vue-carousel';
import * as route from './routes';
Vue.use(VueRouter);
Vue.use(VueCarousel);
Vue.component('scroll-list',scroll);

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
router.beforeResolve((to, from, next) => {
    store.dispatch('fetchingData', to);
    from.meta.userText = store.getters.userText;
    next();
})
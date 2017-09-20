import Vue from 'vue';
import App from './components/app/app.vue';
import state from './store/flow';
import searchStore from './store/search';


import VueRouter from 'vue-router';
import VueResource from 'vue-resource';
import VueCarousel from 'vue-carousel';
import Vuex from 'vuex';
import * as route from './routes';
Vue.use(VueRouter);
Vue.use(VueResource);
Vue.use(VueCarousel);
Vue.use(Vuex);


const store = new Vuex.Store({
    modules: { state, searchStore }
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
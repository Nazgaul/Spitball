import Vue from 'vue';
import App from './components/app/app.vue';
import state from './store/flow';


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
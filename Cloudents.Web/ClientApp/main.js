import Vue from 'vue';
import App from './components/app/app.vue';
import store from './store';
import scroll from './components/helpers/infinateScroll.vue'
import VueRouter from 'vue-router';

const vuetifyComponents = {
    VApp,
    VGrid,
    VChip,
    VToolbar,
    VNavigationDrawer,
    VList
};
import {
    Vuetify,
    VApp,
    VGrid,
    VChip,
    VToolbar,
    VNavigationDrawer,
    VList

} from 'vuetify'
import * as route from './routes';

//import './main.styl'
import('../wwwroot/content/main.less');
Vue.use(VueRouter);
Vue.use(Vuetify,
    {
        components: vuetifyComponents
    });
Vue.component('scroll-list', scroll);

const router = new VueRouter({
    mode: "history",
    routes: route.routes
});
new Vue({
    el: "#app",
    router: router,
    render: h => h(App),
    store
});
router.beforeResolve((to, from, next) => {
    if (to.meta.load) store.dispatch(to.meta.load, to)
    //TODO not good as wanted save on the route name and not on current only
    from.meta.userText = store.getters.userText;
    next();
})
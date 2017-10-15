import Vue from 'vue';
import App from './components/app/app.vue';
import store from './store';
import scroll from './components/helpers/infinateScroll.vue'
import VueRouter from 'vue-router';
import {
    Vuetify,
    VApp,
    VGrid,
    VChip,
    VToolbar,
    VNavigationDrawer
} from 'vuetify'
import * as route from './routes';

//import './main.styl'
import('../wwwroot/content/main.less');
Vue.use(VueRouter);
Vue.use(Vuetify,
    {
        components: {
            VApp,
            VGrid,
            VChip,
            VToolbar,
            VNavigationDrawer
        }
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
    from.meta.userText = store.getters.userText;
    next();
})
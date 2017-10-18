import Vue from 'vue';
import App from './components/app/app.vue';
import store from './store';
import scroll from './components/helpers/infinateScroll.vue'
import VueRouter from 'vue-router';
import vueMoment from "vue-moment";
import vueAdsense from 'vue-adsense';

const vuetifyComponents = {
    VApp,
    VGrid,
    VChip,
    VToolbar,
    VNavigationDrawer,
    VList,
    VBadge,
    VProgressCircular,
    VSubheader,
    VBtn
};
import {
    Vuetify,
    VApp,
    VGrid,
    VChip,
    VToolbar,
    VNavigationDrawer,
    VList,
    VBadge,
    VProgressCircular,
    VSubheader,
    VBtn

} from 'vuetify'
import * as route from './routes';

//import './main.styl'
import('../wwwroot/content/main.less');
Vue.use(VueRouter);
Vue.use(Vuetify,
    {
        components: vuetifyComponents
    });
Vue.use(vueMoment);
Vue.component('scroll-list', scroll);
Vue.component('adsense', vueAdsense);

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
    //from.params.userText = store.getters.userText;
    //from = { ...from, params: { userText: store.getters.userText } }
    //console.log(from)
    next();
})
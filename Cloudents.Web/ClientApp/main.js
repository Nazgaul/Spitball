import Vue from 'vue';
import App from './components/app/app.vue';
import store from './store';
const scroll = () => import('./components/helpers/infinateScroll.vue');
import VScroll from 'vuetify/es5/directives/scroll';
import GeneralPage from './components/helpers/generalPage.vue'
import VueRouter from 'vue-router';
import vueMoment from "vue-moment";
import vueAdsense from 'vue-adsense';

const vuetifyComponents = {
    VApp,
    //VNavigationDrawer,
    VGrid,
    VChip,
    VToolbar,
    VList,
    VAvatar,
    //VBadge,
    VCard,
    VCarousel,
    VProgressCircular,
    VProgressLinear,
    VSubheader,
    VDivider,
    VSnackbar,
    VDialog,
    VTextField,
    VBtn,
    VTooltip,
    VMenu,
    VSwitch,
    VTabs
};
import {
    Vuetify,
    VApp,
    //VNavigationDrawer,
    VGrid,
    VChip,
    VToolbar,
    VList,
    VTextField,
    VAvatar,
    //VBadge,
    VCard,
    VCarousel,
    VProgressCircular,
    VProgressLinear,
    VSubheader,
    VDivider,
    VSnackbar,
    VDialog,
    VBtn,
    VTooltip,
    VMenu,
    VSwitch,
    VTabs

} from 'vuetify'
import * as route from './routes';

Vue.use(VueRouter);
Vue.use(Vuetify,
    {
        directives:{VScroll},
        components: vuetifyComponents
    });
Vue.use(vueMoment);
Vue.component('scroll-list', scroll);
Vue.component('adsense', vueAdsense);
Vue.component('general-page', GeneralPage);

const router = new VueRouter({
    mode: "history",
    routes: route.routes,
    scrollBehavior(to, from, savedPosition) {
        if (savedPosition) {
            return savedPosition;
        } else {
            return { x: 0, y: 0 }
        }
    }
});
new Vue({
    el: "#app",
    router: router,
    render: h => h(App),
    store
});

﻿import Vue from 'vue';
import App from './components/app/app.vue';
import store from './store';
import scroll from './components/helpers/infinateScroll.vue'
import VueRouter from 'vue-router';
import vueMoment from "vue-moment";
import vueAdsense from 'vue-adsense';
//import VueIntercom from 'vue-intercom';

const vuetifyComponents = {
    VApp,
    VNavigationDrawer,
    VGrid,
    VChip,
    VToolbar,
    VList,
    //VBadge,
    VCard,
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
VSwitch
};
import {
    Vuetify,
    VApp,
    VNavigationDrawer,
    VGrid,
    VChip,
    VToolbar,
    VList,
    VTextField,
    //VBadge,
    VCard,
    VProgressCircular,
    VProgressLinear,
    VSubheader,
    VDivider,
    VSnackbar,
    VDialog,
    VBtn,
    VTooltip,
    VMenu,
    VSwitch

} from 'vuetify'
import * as route from './routes';

//import './main.styl'
//import('../wwwroot/content/main.less');
Vue.use(VueRouter);
Vue.use(Vuetify,
    {
        components: vuetifyComponents
    });
Vue.use(vueMoment);
//Vue.use(VueIntercom, { appId: 'njmpgayv' });
Vue.component('scroll-list', scroll);
Vue.component('adsense', vueAdsense);

const router = new VueRouter({
    mode: "history",
    routes: route.routes,
    scrollBehavior(to, from, savedPosition) {
        if (savedPosition) {
            return savedPosition
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

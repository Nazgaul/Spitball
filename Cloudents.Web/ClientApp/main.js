import Vue from 'vue';
import App from './components/app/app.vue';
import store from './store';
import scroll from './components/helpers/infinateScroll.vue'
import VueRouter from 'vue-router';
import vueMoment from "vue-moment";
import vueAdsense from 'vue-adsense';
import * as VueGoogleMaps from 'vue2-google-maps'
import VueIntercom from 'vue-intercom';

const vuetifyComponents = {
    VApp,
    VNavigationDrawer,
    VGrid,
    VChip,
    VToolbar,
    VList,
    VBadge,
    VProgressCircular,
    VSubheader,
    VDivider,
    VSnackbar,
    VDialog,
    VTextField,
    VBtn
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
    VBadge,
    VProgressCircular,
    VSubheader,
    VDivider,
    VSnackbar,
    VDialog,
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
Vue.use(VueIntercom, { appId: 'njmpgayv' });
Vue.use(VueGoogleMaps, {
    load: {
        key: 'AIzaSyAoFR5uWJy1cf76q-J46EoEbFVZCaLk93w',
        //libraries: 'places', // This is required if you use the Autocomplete plugin
        // OR: libraries: 'places,drawing'
        // OR: libraries: 'places,drawing,visualization'
        // (as you require)
    }
})
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
    store,
    watch: {
        '$intercom.ready': function() {
            this.$intercom.boot({
                //user_id: 1,
                //name: 'yifat',
                //email: 'yifat@cloudents.com',
            });
            this.$intercom.show();
        }
    }
});

import Vue from "vue";
import App from "./components/app/app.vue";
import store from "./store";
const scroll = () => import("./components/helpers/infinateScroll.vue");
import VScroll from "vuetify/es5/directives/scroll";
import GeneralPage from "./components/helpers/generalPage.vue"
import VueRouter from "vue-router";
import vueAdsense from "vue-adsense";


//NOTE: put changes in here in webpack vendor as well
const vuetifyComponents = {
    VApp,
    //VNavigationDrawer,
    VGrid,
    VChip,
    VCheckbox,
    VToolbar,
    VList,
    VAvatar,
    //VBadge,
    VExpansionPanel,
    VCard,
    VCarousel,
    VProgressCircular,
    VProgressLinear,
    VSubheader,
    VDivider,
    VDialog,
    VTextField,
    VBtn,
    VTooltip,
    VMenu,
    VSwitch,
    VTabs,
    VIcon
};
import {
    Vuetify,
    VApp,
    //VNavigationDrawer,
    VGrid,
    VChip,
    VCheckbox,
    VToolbar,
    VList,
    VTextField,
    VAvatar,
    //VBadge,
    VExpansionPanel,
    VCard,
    VCarousel,
    VProgressCircular,
    VProgressLinear,
    VSubheader,
    VDivider,
    VDialog,
    VBtn,
    VTooltip,
    VMenu,
    VSwitch,
    VTabs,
    VIcon

} from "vuetify"
import * as route from "./routes";
import VueLazyload from 'vue-lazyload'

Vue.use(VueLazyload, {
    lazyComponent: true,
    preLoad: 1,
    attempt: 1
});
Vue.use(VueRouter);
Vue.use(Vuetify,
    {
        directives:{VScroll},
        components: vuetifyComponents
    });
Vue.component("scroll-list", scroll);
Vue.component("adsense", vueAdsense);
Vue.component("general-page", GeneralPage);

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

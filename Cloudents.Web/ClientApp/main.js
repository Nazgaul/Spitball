import Vue from "vue";
import App from "./components/app/app.vue";
import store from "./store";
const scroll = () => import("./components/helpers/infinateScroll.vue");
import VScroll from "vuetify/es5/directives/scroll";
const GeneralPage = () => import("./components/helpers/generalPage.vue");
import VueRouter from "vue-router";
const vueAdsense = () => import("vue-adsense");
import VueAnalytics from "vue-analytics";
import WebFont from "webfontloader";

//NOTE: put changes in here in webpack vendor as well
const vuetifyComponents = {
    VApp,
    VGrid,
    VChip,
    VToolbar,
    VList,
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
    VGrid,
    VChip,
    VToolbar,
    VList,
    VTextField,
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

WebFont.load({
    google: {
        families: ["Open+Sans:300,400,600,700"]
    }
});

Vue.use(VueLazyload, {
    lazyComponent: true,
    preLoad: 1.8,
    attempt: 1
});
Vue.use(VueRouter);
Vue.use(Vuetify,
    {
        directives:{VScroll},
        components: vuetifyComponents
    });
Vue.component("scroll-list", scroll);
Vue.component("adsense",  vueAdsense);
Vue.component("general-page", GeneralPage);
//Vue.prototype.$version = window.version;


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
Vue.use(VueAnalytics,
    {
        id: 'UA-100723645-2',
        router,
        autoTracking: {
            exception: true
        }
    });
new Vue({
    el: "#app",
    router: router,
    render: h => h(App),
    store
});
//This is for cdn fallback do not touch
global.mainCdn = true;

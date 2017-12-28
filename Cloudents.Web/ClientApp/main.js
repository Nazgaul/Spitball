﻿import Vue from "vue";
import App from "./components/app/app.vue";
import store from "./store";
const scroll = () => import("./components/helpers/infinateScroll.vue");
import VScroll from "vuetify/es5/directives/scroll";
const GeneralPage = () => import("./components/helpers/generalPage.vue");
import VueRouter from "vue-router";
const vueAdsense = () => import("vue-adsense");
import VueAnalytics from "vue-analytics";
import WebFont from "webfontloader";
import checkBox from "./components/app/sb-checkbox"

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
    VIcon,
    VCheckbox
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
    VIcon,
    VCheckbox

} from "vuetify"
import * as route from "./routes";
import VueLazyload from 'vue-lazyload'

//TODO: server side fix
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
//Vue.use(vueSmoothScroll);
Vue.use(VueRouter);
Vue.use(Vuetify,
    {
        directives: { VScroll },
        components: vuetifyComponents
    });
Vue.component("scroll-list", scroll);
Vue.component("adsense", vueAdsense);
Vue.component("general-page", GeneralPage);
Vue.component("sb-checkbox", checkBox);

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
Vue.filter('capitalize',
    function (value) {
        if (!value) return '';
        value = value.toString();
        return value.charAt(0).toUpperCase() + value.slice(1);
    });
Vue.filter('ellipsis',
    function (value, characters) {
        if (value && (value.length <= characters))
            return value;
        return value.substr(0, characters) + '...';


    });
const app = new Vue({
    //el: "#app",
    router: router,
    render: h => h(App),
    store
});
//app.$mount("#app");
//This is for cdn fallback do not touch
global.mainCdn = true;
export { app, router };

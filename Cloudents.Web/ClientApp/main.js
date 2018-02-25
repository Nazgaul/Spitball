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
//import Smartbanner from "smartbanner";

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
    VSnackbar
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
    VSnackbar

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
        disableScriptLoader: true,
        router,
        autoTracking: {
            pageviewTemplate(route) {
                // let title=route.name.charAt(0).toUpperCase() + route.name.slice(1);
                return {
                    page: route.path,
                    title: route.name ? route.name.charAt(0).toUpperCase() + route.name.slice(1) : '',
                    location: window.location.href
                }
            },
            exception: true
        }
    });
//#region yifat
Vue.filter('capitalize',
    function (value) {
        if (!value) return '';
        value = value.toString();
        var values = value.split("/");
        for (var v in values) {
            var tempVal = values[v];
            values[v] = tempVal.charAt(0).toUpperCase() + tempVal.slice(1);
        }
        return values.join(" ")
        //return value.charAt(0).toUpperCase() + value.slice(1);
    });
//#endregion
Vue.filter('ellipsis',
    function (value, characters) {
        value = value || '';
        if (value.length <= characters)
            return value;
        return value.substr(0, characters) + '...';


    });
const app = new Vue({
    //el: "#app",
    router: router,
    render: h => h(App),
    store
});
router.onReady(() => {
    intercom(router.currentRoute)
    router.beforeEach((to, from, next) => {
        intercom(to)
        next();
    });

    function intercom(to) {
        if (window.innerWidth < 600) {
            let hideLauncher = true
            if (to.name === "home") {
                hideLauncher = false;
            }

            intercomSettings.hide_default_launcher = hideLauncher;
        }
        Intercom("update");
    }
});
//app.$mount("#app");
//This is for cdn fallback do not touch
global.mainCdn = true;
export { app, router };

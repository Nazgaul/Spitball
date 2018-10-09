﻿
import Vue from "vue";
import App from "./components/app/app.vue";
import store from "./store";
import { Language } from "./services/language/langDirective";
import initSignalRService from './services/signalR/signalrEventService'

// clip board copy text
import VueClipboard from 'vue-clipboard2'
import Scroll from "vuetify/es5/directives/scroll";
const scrollComponent = () => import("./components/helpers/infinateScroll.vue");


const GeneralPage = () =>
    import("./components/helpers/generalPage.vue");
import VueRouter from "vue-router";
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
    VSelect,
    VBtn,
    VBtnToggle,
    VTooltip,
    VMenu,
    VSwitch,
    VTabs,
    VIcon,
    VSnackbar,
    VNavigationDrawer,
    VAvatar,
    VPagination,
    VDataTable,
};
import {
    Vuetify,
    VApp,
    VGrid,
    VChip,
    VToolbar,
    VList,
    VTextField,
    VSelect,
    VExpansionPanel,
    VCard,
    VCarousel,
    VProgressCircular,
    VProgressLinear,
    VSubheader,
    VDivider,
    VDialog,
    VBtn,
    VBtnToggle,
    VTooltip,
    VMenu,
    VSwitch,
    VTabs,
    VIcon,
    VSnackbar,
    VNavigationDrawer,
    VAvatar,
    VPagination,
    VDataTable,


} from "vuetify";
import * as route from "./routes";

import { constants } from "./utilities/constants";

//TODO: server side fix
WebFont.load({
    google: {
        families: ["Open+Sans:300,400,600,700", "Fira+Sans:300,400,600,700"]
    }
});

//Vue.use(VueLazyload, {
//    lazyComponent: true,
//    preLoad: 1.8,
//    attempt: 1
//});
//Vue.use(vueSmoothScroll);

Vue.use(VueRouter);
Vue.use(Vuetify, {
    directives: {
        Scroll,
    },
    components: vuetifyComponents
});
Vue.component("scroll-list", scrollComponent);
//Vue.component("adsense", vueAdsense);
Vue.component("general-page", GeneralPage);

const router = new VueRouter({
    mode: "history",
    routes: route.routes,
    scrollBehavior(to, from, savedPosition) {
        return new Promise((resolve, reject) => {
                if(savedPosition){
                    resolve({ x: savedPosition.x, y: savedPosition.y });
                }else{
                    resolve({ x: 0, y: 0 });
                }
          });
    }
});

Vue.use(VueClipboard)

Vue.use(VueAnalytics, {
    id: 'UA-100723645-2',
    disableScriptLoader: true,
    router,
    autoTracking: {
        pageviewOnLoad: false,
        //ignoreRoutes: ['result'],
        shouldRouterUpdate(to, from) {
            return to.path != "/result";
        },
        pageviewTemplate(route) {
            return {
                page: route.path,
                title: route.name ? route.name.charAt(0).toUpperCase() + route.name.slice(1) : '',
                location: window.location.href
            };
        },
        exception: true
    }
});

Vue.directive('language', Language);


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
        return values.join(" ");
        //return value.charAt(0).toUpperCase() + value.slice(1);
    });
//#endregion

//check if firefox for ellipsis, if yes use allipsis filter if false css multiline ellipsis
global.isFirefox = global.navigator.userAgent.toLowerCase().indexOf('firefox') > -1;
Vue.filter('ellipsis',
    function (value, characters, datailedView) {
        value = value || '';
        if (value.length <= characters || datailedView || !global.isFirefox){
            return value;
        }else{
            return value.substr(0, characters) + '...';

        }
    });
Vue.filter('bolder',
  function (value, query) {
    if(query.length) {
        query.map((item) => {
            value = value.replace(item, '<span class="bolder">' + item + '</span>')
        });
    }
    return value
});

Vue.filter('fixedPoints', function (value) {
    if (!value) return 0;
    if (value.toString().indexOf('.') === -1) return value;
    return parseFloat(value).toFixed(2);
});

Vue.filter('dollarVal', function (value) {
    if (!value) return 0;
    return parseFloat(value / 40).toFixed(2);
});

Vue.filter('dateFromISO', function (value) {
    let d = new Date(value);
    //return load if no data
    if (!value) {
        return 'Loading..';
    }
    return `${d.getUTCMonth() + 1}/${d.getUTCDate()}/${d.getUTCFullYear()}`;
});
// filter for numbers, format numbers to local formats. Read more: 'toLocaleString'
Vue.filter('currencyLocalyFilter', function (value) {
    let amount = Number(value);
    return amount && amount.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 }) || '0';
});

Vue.filter('commasFilter', function (value) {

    return value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
});


router.beforeEach((to, from, next) => {
    if (!!to.query && Object.keys(to.query).length > 0) {
        for (let prop in to.query) {
            if (constants.regExXSSCheck.test(to.query[prop])) {
                to.query[prop] = "";
            }
        }
    }
    if (global.innerWidth < 600) {
        intercomSettings.hide_default_launcher = true;
    }
    else {
        intercomSettings.hide_default_launcher = false;
    }
    store.dispatch('changeLastActiveRoute', from);
    checkUserStatus(to, next);
    
});
const app = new Vue({
    //el: "#app",
    router: router,
    render: h => h(App),
    store,

});


function checkUserStatus(to, next) {

    store.dispatch('userStatus', {
        isRequire: to.meta.requiresAuth,
        to
    }).then(() => {
        if (!store.getters.loginStatus && to.meta && to.meta.requiresAuth) {
            next("/signin");
        } else {
            next();
        }
    }).catch(error => {
        console.error(error);
        next("/signin");
    });
}

initSignalRService();

//app.$mount("#app");
//This is for cdn fallback do not touch
global.mainCdn = true;
export {
    app,
    router
};

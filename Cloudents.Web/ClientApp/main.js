import Vue from "vue";
import App from "./components/app/app.vue";
import { sync } from 'vuex-router-sync'
import store from "./store";
import { Language } from "./services/language/langDirective";
import { LanguageService } from './services/language/languageService';
//import initSignalRService from './services/signalR/signalrEventService'; only logged in users will connect to the signalR
// clip board copy text
import VueClipboard from 'vue-clipboard2';
import lineClamp from 'vue-line-clamp';
import Scroll from "vuetify/es5/directives/scroll";
import scrollComponent from './components/helpers/infinateScroll.vue';
import GeneralPage from './components/helpers/generalPage.vue';
import VueRouter from "vue-router";
import VueAnalytics from "vue-analytics";
import WebFont from "webfontloader";
import CloudentsTour from 'cloudents-tour';
require('cloudents-tour/dist/cloudents-tour.css');
import VueYouTubeEmbed from 'vue-youtube-embed'; //https://github.com/kaorun343/vue-youtube-embed
import LoadScript from 'vue-plugin-load-script';

import VueNumeric from 'vue-numeric'



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
    VStepper,
    VCombobox,
    VCheckbox,
    VParallax,
    VBottomNav,
    VTextarea,
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
    VStepper,
    VCombobox,
    VCheckbox,
    VParallax,
    VBottomNav,
    VTextarea
} from "vuetify";
import * as route from "./routes";

const ilFonts = [
    "Assistant:400",
    ]
const usFonts = [
    "Open+Sans:400",
    "Fira+Sans:400",
    ]

let usedFonts = global.lang.toLowerCase() === 'he' ? ilFonts : usFonts;

//TODO: server side fix
WebFont.load({
    google: {
        families: usedFonts
    }
});

//Vue.use(VueLazyload, {
//    lazyComponent: true,
//    preLoad: 1.8,
//    attempt: 1
//});


Vue.use(CloudentsTour);
Vue.use(VueRouter);
Vue.use(LoadScript);
Vue.use(Vuetify, {
    directives: {
        Scroll,
    },
    components: vuetifyComponents
});


Vue.use(VueYouTubeEmbed, { global: true });
Vue.component("scroll-list", scrollComponent);
//Vue.component("adsense", vueAdsense);
Vue.component("general-page", GeneralPage);

const router = new VueRouter({
    mode: "history",
    routes: route.routes,
    scrollBehavior(to, from, savedPosition) {
        return new Promise((resolve, reject) => {
            if (savedPosition) {
                resolve({x: savedPosition.x, y: savedPosition.y});
            } else {
                resolve({x: 0, y: 0});
            }
        });
    }
});

Vue.use(VueClipboard);
Vue.use(lineClamp, {

});


Vue.use(VueNumeric);

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
// Register a global custom directive called `v-focus`

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
//is rtl
global.isRtl = document.getElementsByTagName("html")[0].getAttribute("dir") === "rtl";
//check if firefox for ellipsis, if yes use allipsis filter if false css multiline ellipsis
global.isFirefox = global.navigator.userAgent.toLowerCase().indexOf('firefox') > -1;
//is country Israel
global.isIsrael = global.country.toLowerCase() === "il";
//check if Edge (using to fix position sticky bugs)
global.isEdgeRtl = false;
global.isEdge = false;
if (document.documentMode || /Edge/.test(navigator.userAgent)) {
    global.isEdge = true;
    if(global.isRtl){
        global.isEdgeRtl = true;
    }

}

Vue.filter('ellipsis',
    function (value, characters, datailedView) {
        value = value || '';
        if (value.length <= characters || datailedView || !global.isFirefox) {
            return value;
        } else {
            return value.substr(0, characters) + '...';

        }
    });
Vue.filter('bolder',
    function (value, query) {
        if (query.length) {
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
    return parseFloat(value / 100).toFixed(2);
});


// 10/12/2018
Vue.filter('dateFromISO', function (value) {
    let d = new Date(value);
    //return load if no data
    if (!value) {
        return LanguageService.getValueByKey('wallet_Loading');
    }
    return `${d.getUTCMonth() + 1}/${d.getUTCDate()}/${d.getUTCFullYear()}`;
});

// Nov 14, 2018 :: MDN Global_Objects/Date/toLocaleDateString
Vue.filter('fullMonthDate', function (value) {
    let date = new Date(value);
    //return load if no data
    if (!value) {
        return ''
    }
// request a weekday along with a long month
    let options = {  year: 'numeric', month: 'short', day: '2-digit' };
    let languageDate = global.lang.toLowerCase() === 'he' ? 'he-IL' : 'en-US';
    return date.toLocaleDateString(languageDate, options);
});



Vue.prototype.$Ph = function (key, placeholders) {
    let rawKey = LanguageService.getValueByKey(key);
    //if no placeholders return the Key without the replace
    if (!placeholders) {
        //console.error(`${key} have no placeholders to replace`)
        return rawKey
    }

    let argumentsToSend = [];
    //placeholders must be an array
    if (Array.isArray(placeholders)) {
        argumentsToSend = placeholders;
    } else {
        argumentsToSend = [placeholders];
    }
    return LanguageService.changePlaceHolders(rawKey, argumentsToSend)
}

// filter for numbers, format numbers to local formats. Read more: 'toLocaleString'
Vue.filter('currencyLocalyFilter', function (value) {
    let amount = Number(value);
    let sblCurrency = LanguageService.getValueByKey('wallet_SBL');
    let result = amount && amount.toLocaleString(undefined, {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    }) || '0';
    return result + " " + sblCurrency;
});

Vue.filter('commasFilter', function (value) {

    return value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
});


router.beforeEach((to, from, next) => {
    if(!to.query || !to.query.university){
        if(!!from.query && !!from.query.university){
            store.dispatch('closeSelectUniFromNav')
        }
    }
    
    // if (!!to.query && Object.keys(to.query).length > 0) {
    //     for (let prop in to.query) {
    //         if (constants.regExXSSCheck.test(to.query[prop])) {
    //             to.query[prop] = "";
    //         }
    //     }
    // }
    if (global.innerWidth < 600) {
        intercomSettings.hide_default_launcher = true;
    }
    else {
        intercomSettings.hide_default_launcher = false;
    }
    //case 10995
    if (global.appInsights) {
        appInsights.trackPageView(to.fullPath);
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

global.isMobileAgent = function() {
    let check = false;
    (function(a){
        if(/(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino/i.test(a)||/1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-/i.test(a.substr(0,4))){
             check = true; 
        }
     })(navigator.userAgent||navigator.vendor||window.opera);
    return check;
  };
  
//initSignalRService();

//app.$mount("#app");
//This is for cdn fallback do not touch

//injects the route to the store via the rootState.route
sync(store, router);

export {
    app,
    router
};

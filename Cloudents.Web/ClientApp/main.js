import Vue from "vue";
import App from "./components/app/app.vue";
import { sync } from 'vuex-router-sync';
import store from "./store";
import { Language } from "./services/language/langDirective";
import { LanguageService } from './services/language/languageService';
//import initSignalRService from './services/signalR/signalrEventService'; only logged in users will connect to the signalR
// clip board copy text
import VueClipboard from 'vue-clipboard2';
import Scroll from "vuetify/es5/directives/scroll";
import Touch from "vuetify/es5/directives/touch";
import scrollComponent from './components/helpers/infinateScroll.vue';
import GeneralPage from './components/helpers/generalPage.vue';
import VueRouter from "vue-router";
import VueAnalytics from "vue-analytics";
import LoadScript from 'vue-plugin-load-script';

// Filters
import './filters/filters';

import VueNumeric from 'vue-numeric';
import VueMathjax from 'vue-mathjax';
import utilitiesService from './services/utilities/utilitiesService';
import VueAppInsights from 'vue-application-insights';
import { VLazyImagePlugin } from "v-lazy-image";

import intercomSettings from './services/intercomService';
import VueFlicking from "@egjs/vue-flicking";

Vue.use(VueFlicking);

// import VueCodemirror from 'vue-codemirror'
// import 'codemirror/lib/codemirror.css'
// import 'code'

// Vue.use(VueCodemirror);

import {
    VApp,
    VAvatar,
    VBottomNav,
    VBtn,
    VBtnToggle,
    VCard,
    VCalendar,
    VCarousel,
    VCheckbox,
    VChip,
    VCombobox,
    VDataTable,
    VDialog,
    VDivider,
    VExpansionPanel,
    VGrid,
    VIcon,
    VList,
    VMenu,
    VNavigationDrawer,
    VPagination,
    VProgressCircular,
    VProgressLinear,
    VSelect,
    VSnackbar,
    VStepper,
    VSubheader,
    VSwitch,
    VTabs,
    VTextarea,
    VTextField,
    VToolbar,
    VTooltip,
    VRating,
    VForm,
    VAutocomplete,
    VSheet,
    VSystemBar,
    Vuetify
} from "vuetify";
import * as route from "./routes";



//NOTE: put changes in here in webpack vendor as well
const vuetifyComponents = {
    VApp,
    VCalendar,
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
    VBottomNav,
    VTextarea,
    VRating,
    VForm,
    VAutocomplete,
    VSheet,
    VSystemBar,
};


Vue.use(VueMathjax);
Vue.use(VueRouter);
Vue.use(LoadScript);

Vue.use(Vuetify, {
    directives: {
        Scroll,
        Touch
    },
    components: vuetifyComponents
});

Vue.component("scroll-list", scrollComponent);
Vue.component("general-page", GeneralPage);

const router = new VueRouter({
    mode: "history",
    routes: route.routes,
    scrollBehavior(to, from, savedPosition) {
        return new Promise((resolve, reject) => {
            if(to.hash){
                resolve({selector: to.hash});
            }
            if (savedPosition) {
                //firefox fix
                if(global.navigator.userAgent.toLowerCase().indexOf('firefox') > -1){
                    setTimeout(()=>{
                        global.scrollTo(savedPosition.x,savedPosition.y);
                    });
                }
                resolve({x: savedPosition.x, y: savedPosition.y});
            } else {
                resolve({x: 0, y: 0});
            }
        });
    }
});

Vue.use(VueClipboard);
// Vue.use(lineClamp, {});
Vue.use(VueNumeric);
Vue.use(VLazyImagePlugin);
Vue.use(VueAnalytics, {
    id: 'UA-100723645-2',
    disableScriptLoader: true,
    router,
    autoTracking: {
        pageviewOnLoad: false,
        //ignoreRoutes: ['result'],
        // shouldRouterUpdate(to, from) {
        //     return to.path != "/result";
        // },
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

//is rtl
global.isRtl = document.getElementsByTagName("html")[0].getAttribute("dir") === "rtl";
//check if firefox for ellipsis, if yes use allipsis filter if false css multiline ellipsis
// global.isFirefox = global.navigator.userAgent.toLowerCase().indexOf('firefox') > -1;
//is country Israel
global.isIsrael = global.country.toLowerCase() === "il";
//check if Edge (using to fix position sticky bugs)
global.isEdgeRtl = false;
global.isEdge = false;
if (document.documentMode || /Edge/.test(navigator.userAgent)) {
    global.isEdge = true;
    if (global.isRtl) {
        global.isEdgeRtl = true;
    }

}

Vue.prototype.$loadStyle = function(url,id){
    return new Promise((resolve, reject) => {
        if (document.querySelector(id)) return resolve()
        let linkTag = document.createElement('link')
        linkTag.id = id
        linkTag.rel = 'stylesheet'
        linkTag.href = url
        document.head.insertBefore(linkTag, document.head.firstChild)
        return resolve()
    })
}

Vue.prototype.$proccessImageUrl = function(url, width, height, mode){
    let usedMode = mode ? mode : 'crop';
    if(url){
        let returnedUrl = `${url}?&width=${width}&height=${height}&mode=${usedMode}`;
        return returnedUrl;
    }else{
        return '';
    }
  };

Vue.prototype.$Ph = function (key, placeholders) {
    let rawKey = LanguageService.getValueByKey(key);
    //if no placeholders return the Key without the replace
    if (!placeholders) {
        //console.error(`${key} have no placeholders to replace`)
        return rawKey;
    }

    let argumentsToSend = [];
    //placeholders must be an array
    if (Array.isArray(placeholders)) {
        argumentsToSend = placeholders;
    } else {
        argumentsToSend = [placeholders];
    }
    return LanguageService.changePlaceHolders(rawKey, argumentsToSend);
};

Vue.prototype.$chatMessage = function (message) {
    if(message.type === 'text'){
        //text and convert links to url's
        let linkTest = /(ftp:\/\/|www\.|https?:\/\/){1}[a-zA-Z0-9u00a1-\\uffff0-]{2,}\.[a-zA-Z0-9u00a1-\\uffff0-]{2,}(\S*)/g;
        let modifiedText = message.text;
        let matchedResults = modifiedText.match(linkTest);

        if(!!matchedResults){
            matchedResults.forEach(result=>{
                let prefix = result.toLowerCase().indexOf('http') === -1 &&
                result.toLowerCase().indexOf('ftp') === -1 ? '//' : '';
                modifiedText = modifiedText.replace(result, `<a href="${prefix}${result}" target="_blank">${result}</a>`);
            });
        } 
        return modifiedText;
    }else{
        let src = utilitiesService.proccessImageURL(message.src, 190, 140, 'crop');
        return `<a href="${message.href}" target="_blank"><img src="${src}"/></a>`;
    }
};

router.beforeEach((to, from, next) => {
    store.dispatch('setRouteStack', to.name);
    if (!to.query || !to.query.university) {
        if (!!from.query && !!from.query.university) {
            store.dispatch('closeSelectUniFromNav');
        }
    } 

    store.dispatch('sendQueryToAnalytic', to);
    intercomSettings.IntercomSettings.set({hideLauncher:true});

    if (global.innerWidth < 600) {
        intercomSettings.IntercomSettings.set({hideLauncher:true});
    }
    else {
        intercomSettings.IntercomSettings.set({hideLauncher:false});
    }
    //if tutoring disable intercom
    if (global.location.href.indexOf("studyroom") > -1) {
        intercomSettings.IntercomSettings.set({hideLauncher:true});
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
    store.dispatch('userStatus', {isRequire: to.meta.requiresAuth, to});
    if (!store.getters.loginStatus && to.meta && to.meta.requiresAuth) {
        next("/signin");
    } else {
        next();
    }
}

global.isMobileAgent = function () {
    let check = false;
    (function (a) {
        if (/(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino/i.test(a) || /1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-/i.test(a.substr(0, 4))) {
            check = true;
        }
    })(navigator.userAgent || navigator.vendor || window.opera);
    return check;
};

let touchSupported = (('ontouchstart' in window) || (navigator.msMaxTouchPoints > 0));

if(touchSupported){
    global.addEventListener('touchstart', function(){
        store.dispatch('setIsTouchEnd', false);
        store.dispatch('setIsTouchMove', false);
        store.dispatch('setIsTouchStarted', true);
    })
    global.addEventListener('touchmove', function(){
        store.dispatch('setIsTouchMove', true);
    })
    global.addEventListener('touchend', function(){
        store.dispatch('setIsTouchStarted', false);
        store.dispatch('setIsTouchMove', false);
        store.dispatch('setIsTouchEnd', true);
    })
}

//initSignalRService();

//app.$mount("#app");
//This is for cdn fallback do not touch

//injects the route to the store via the rootState.route
sync(store, router);
utilitiesService.init();


Vue.use(VueAppInsights, {
    appInsights: global.appInsights,
    router
});

export {
    app,
    router
};

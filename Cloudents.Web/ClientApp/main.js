import Vue from "vue";
import vuetify from './plugins/vuetify';
import { sync } from 'vuex-router-sync';
import * as route from "./routes";
import store from "./store";
import VueRouter from "vue-router";
import VueAnalytics from "vue-analytics";
import LoadScript from 'vue-plugin-load-script';
import VueClipboard from 'vue-clipboard2';
import VueAppInsights from 'vue-application-insights';
import VueFlicking from "@egjs/vue-flicking";
import {i18n, loadLanguageAsync } from './plugins/t-i18n';

// Global Components
import App from "./components/app/app.vue";
import UserAvatar from './components/helpers/UserAvatar/UserAvatar.vue';

// Global Services
import { Language } from "./services/language/langDirective";
import utilitiesService from './services/utilities/utilitiesService';

// Fonts
import '../ClientApp/myFont.font.js'; 

// Vue Prototypes
import './prototypes/prototypes.js'; 

// Directives
import { openDialog,closeDialog } from './directives/dialog.js';

if(!window.IntersectionObserver){ // Intersection observer support
    import('intersection-observer');
}

const router = new VueRouter({
    mode: "history",
    routes: route.routes,
    scrollBehavior(to, from, savedPosition) {
        return new Promise((resolve) => {
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
//import initSignalRService from './services/signalR/signalrEventService'; only logged in users will connect to the signalR

Vue.use(VueFlicking);
Vue.use(VueRouter);
Vue.use(LoadScript);
Vue.use(VueClipboard);

Vue.component("UserAvatar",UserAvatar);

//this need to be below the configuration of vue router
Vue.use(VueAnalytics, {
    id: 'UA-100723645-2',
    disableScriptLoader: true,
    router,
    autoTracking: {
        pageviewOnLoad: false,
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
Vue.directive('openDialog',openDialog);
Vue.directive('closeDialog',closeDialog);

global.isRtl = document.getElementsByTagName("html")[0].getAttribute("dir") === "rtl";
global.isEdgeRtl = false;
if (document.documentMode || /Edge/.test(navigator.userAgent)) {
    if (global.isRtl) {
        global.isEdgeRtl = true;
    }
}

router.beforeEach((to, from, next) => {
    store.dispatch('setRouteStack', to.name);
    store.dispatch('sendQueryToAnalytic', to);
    let isLogged = store.getters.getUserLoggedInStatus;
    
    if (!isLogged && to.meta && to.meta.requiresAuth) {
        next("/signin");
        return;
    }
    let getAccountUser = store.dispatch('userStatus');
    Promise.all([getAccountUser,loadLanguageAsync()]).then(() => {
       next();
    });
});

sync(store, router);

const app = new Vue({
    //el: "#app",
    router: router,
    store,
    vuetify,
    i18n,
    render: h => h(App),
});

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
    });
    global.addEventListener('touchmove', function(){
        store.dispatch('setIsTouchMove', true);
    });
    global.addEventListener('touchend', function(){
        store.dispatch('setIsTouchStarted', false);
        store.dispatch('setIsTouchMove', false);
        store.dispatch('setIsTouchEnd', true);
    });
}

//initSignalRService();

//app.$mount("#app");
//This is for cdn fallback do not touch

//injects the route to the store via the rootState.route

utilitiesService.init();

Vue.use(VueAppInsights, {
    //appInsights: global.appInsights,
    id : global.applicationId,
    router
});

export {
    app,
    router
};

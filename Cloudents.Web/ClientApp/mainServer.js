import Vue from "vue";
import App from "./components/app/appServer.vue";
import Router from "vue-router";
const Foo= require('./components/Foo.vue').default;
import Bar from './../ClientApp/components/Bar.vue'

Vue.use(Router);
//Vue.use(Vuetify,
//    {
//        directives: { VScroll },
//        components: vuetifyComponents
//    });
//Vue.component("scroll-list", scroll);
//Vue.component("adsense", vueAdsense);
//Vue.component("general-page", GeneralPage);

function createRouter() {
    return new Router({
        mode: "history",
        // routes:route.routes.filter(i=>i.name==='work').concat([{ path: '/foo', component: Foo,alias:['foo'] }])
        routes:  [
            { path: '/foo', component: Foo,alias:['foo'] },
            { path: '/bar', component: Bar,alias:['bar','yifat'] }
        ],
        //scrollBehavior(to, from, savedPosition) {
        //    if (savedPosition) {
        //        return savedPosition;
        //    } else {
        //        return { x: 0, y: 0 }
        //    }
        //}

    });
}

//Vue.use(VueAnalytics,
//    {
//        id: 'UA-100723645-2',
//        disableScriptLoader: true,
//        router,
//        autoTracking: {
//            pageviewTemplate(route) {
//                // let title=route.name.charAt(0).toUpperCase() + route.name.slice(1);
//                return {
//                    page: route.path,
//                    title: route.name ? route.name.charAt(0).toUpperCase() + route.name.slice(1) : '',
//                    location: window.location.href
//                }
//            },
//            exception: true
//        }
//    });
//#region yifat
//Vue.filter('capitalize',
//    function (value) {
//        if (!value) return '';
//        value = value.toString();
//        var values = value.split("/");
//        for (var v in values) {
//            var tempVal = values[v];
//            values[v] = tempVal.charAt(0).toUpperCase() + tempVal.slice(1);
//        }
//        return values.join(" ")
//        //return value.charAt(0).toUpperCase() + value.slice(1);
//    });
////#endregion
//Vue.filter('ellipsis',
//    function (value, characters) {
//        value = value || '';
//        if (value.length <= characters)
//            return value;
//        return value.substr(0, characters) + '...';


//    });

export function createApp() {
    const router = createRouter()
    // const store = createStore()
    // sync(store, router)

    const app = new Vue({
        el: "#app",
        router,
        // store,
        render: h => h(App),
        //store
    });
    return { app, router }
}
//router.onReady(() => {
//    intercom(router.currentRoute)
//    router.beforeEach((to, from, next) => {
//        intercom(to)
//        next();
//    });
//TODO: server side fix
//function intercom(to) {
//    if (window.innerWidth < 600) {
//        let hideLauncher = true
//        if (to.name === "home") {
//            hideLauncher = false;
//        }

//        intercomSettings.hide_default_launcher = hideLauncher;
//    }
//    Intercom("update");
//}
//});
//app.$mount("#app");
//This is for cdn fallback do not touch
// global.mainCdn = true;
//export { app, router };

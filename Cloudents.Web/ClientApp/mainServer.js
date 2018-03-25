import Vue from "vue";
import App from "./components/app/appServer.vue";
import Router from "vue-router";
const Foo= require('./components/Foo.vue').default;
import Bar from './../ClientApp/components/Bar.vue';
import {ssrPages} from "./routes";
import {staticRoutes} from "./components/satellite/satellite-routes";

Vue.use(Router);

let pages=[];
staticRoutes.forEach((item)=>pages.push({
    path: "/"+item.name,
    name: item.name,
    component:item.import,alias:[item.name],
    props:item.params
}));

function createRouter() {
    return new Router({
        mode: "history",
        routes:  [
            ...pages,
            { path: '/foo', component: Foo,alias:['foo'] },
            { path: '/bar', component: Bar,alias:['bar','yifat','/yifat'] }
        ]
    });
}


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


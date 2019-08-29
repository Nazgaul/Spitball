import Vue from "vue";
import VueRouter from "vue-router"
import App from './App.vue'
import { routes } from './routes'
import Toaster from 'v-toaster'
import 'v-toaster/dist/v-toaster.css'
import store from "./store";
import Vuetify from 'vuetify'
import 'vuetify/dist/vuetify.min.css'
import VueClipboard from 'vue-clipboard2'

const VueUploadComponent = require('vue-upload-component');
Vue.component('file-upload', VueUploadComponent);

Vue.config.productionTip = false;
Vue.use(VueRouter);
Vue.use(Toaster);
Vue.use(Vuetify,
    {
        theme: {
            
        }
    });
Vue.use(VueClipboard);
// 10/12/2018
Vue.filter('dateFromISO', function (value) {
    let d = new Date(value);
    //return load if no data
    return `${d.getUTCMonth() + 1}/${d.getUTCDate()}/${d.getUTCFullYear()}`;
});


Vue.use(VueClipboard);
const router = new VueRouter({
    mode: "history",
    routes: routes
});

const app = new Vue({
    router: router,
    render: h => h(App),
    store
});


export {
    app,
    router
};
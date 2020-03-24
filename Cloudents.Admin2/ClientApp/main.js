// ReSharper disable once InconsistentNaming
import Vue from "vue";
// ReSharper disable once InconsistentNaming

import VueRouter from "vue-router"
// ReSharper disable once InconsistentNaming
import App from './app.vue'
import { routes } from './routes'
import toaster from 'v-toaster'
import 'v-toaster/dist/v-toaster.css'
import store from "./store";
import vuetify from 'vuetify'
import 'vuetify/dist/vuetify.min.css'
import vueClipboard from 'vue-clipboard2'

const vueUploadComponent = require('vue-upload-component');
Vue.component('file-upload', vueUploadComponent);

const vueJsonToCsv = require('vue-json-to-csv');
Vue.component('json-to-csv', vueJsonToCsv);

Vue.config.productionTip = false;
Vue.use(VueRouter);
Vue.use(toaster);
Vue.use(vuetify,
    {
        theme: {
            
        }
    });
Vue.use(vueClipboard);

// 10/12/2018
Vue.filter('dateFromISO', function (value) {
    let d = new Date(value);
    //return load if no data
    return `${d.getUTCMonth() + 1}/${d.getUTCDate()}/${d.getUTCFullYear()}`;
});


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
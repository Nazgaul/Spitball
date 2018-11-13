import Vue from "vue";
import VueRouter from "vue-router"
import App from './App.vue'
import { routes } from './routes'
import Toaster from 'v-toaster'
import 'v-toaster/dist/v-toaster.css'
const VueUploadComponent = require('vue-upload-component')
Vue.component('file-upload', VueUploadComponent)

Vue.config.productionTip = false
Vue.use(VueRouter)
Vue.use(Toaster, {timeout: 5000})
console.log(routes);

const router = new VueRouter({
    mode: "history",
    routes: routes,
});

const app = new Vue({
    router: router,
    render: h => h(App),
});

export {
    app,
    router
};
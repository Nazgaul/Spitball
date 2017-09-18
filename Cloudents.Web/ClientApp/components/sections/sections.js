import Vue from 'vue'
//import VueRouter from 'vue-router';
import appAside from './aside/navbar.vue'
import appHeader from './header/header.vue'
//Vue.component('app-aside', {
//    render: h => h(require('./aside/navbar.vue'),
//});
//Vue.component('app-header', {
//    render: h => h(require('./header/header.vue.html')),
//    props: ['isOpen']
//});
const NotFound = { template: '<p>Page not found</p>' }
const Home = { template: '<p>home page</p>' }
const About = { template: '<p>about page</p>' }
//const routes = new VueRouter({
//    routes: [
//        { path:'/ask',component: Home },
//        { path: '/note', component: Home },
//        { path: '/flashcard', component: About },
//        { path: '/tutor', component: About },
//        { path: '/job', component: About },
//        { path: '/purchase', component: NotFound }
//    ]})
export default {
    components: {
        'app-aside':appAside, 'app-header':appHeader
    },
    
    data() {
        return {
            isOpen: "yifat",
            section: ""
        }
    }
}
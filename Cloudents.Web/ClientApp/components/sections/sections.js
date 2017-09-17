import Vue from 'vue'
import appAside from './aside/navbar.vue'
import appHeader from './header/header.vue'
//Vue.component('app-aside', {
//    render: h => h(require('./aside/navbar.vue'),
//});
//Vue.component('app-header', {
//    render: h => h(require('./header/header.vue.html')),
//    props: ['isOpen']
//});
//var routes = [
//    { path: '/ask', component: require('./header/header.vue.html') },
//    { path: '/note', component: require('./header/header.vue.html') },
//    { path: '/flashcard', component: require('./header/header.vue.html') }
//];
export default {
    components: {
        'app-aside':appAside, 'app-header':appHeader
    },
    data() {
        return {
            isOpen: "yifat",
            section:""
        }
    }
}
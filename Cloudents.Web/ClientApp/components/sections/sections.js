import Vue from 'vue'
//import AsidePage from './aside/navbar.vue'
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
window.addEventListener('load', function () {
    var page = new Vue({
        el: "#sections",
        data: {
            isOpen: false
        }
    })
})
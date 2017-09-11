import './css/site.css';
//import 'bootstrap';
import Vue from 'vue';
import VueRouter from 'vue-router';
Vue.use(VueRouter);
import VueResource from 'vue-resource';
Vue.use(VueResource);
import SvgSprite from 'vue-svg-sprite';

Vue.use(SvgSprite, {
    url: './components/sections/Images/icons.svg',
    class: 'icon'
});


//const routes = [
//    { path: '/', component: require('./components/sections/sections.vue.html') },
//    //{ path: '/', component: require('./components/home/home.vue.html') },
//    { path: '/counter', component: require('./components/counter/counter.vue.html') },
//    { path: '/fetchdata', component: require('./components/fetchdata/fetchdata.vue.html') }
//];

//new Vue({
//    el: '#app-root',
//    router: new VueRouter({ mode: 'history', routes: routes }),
//    render: h => h(require('./components/app/app.vue.html'))
//});
Vue.component('app-aside', {

   render: h => h(require('./components/sections/aside/navbar.vue.html')),

    props: ['isOpen']
});
var aa=new Vue({
    el: '#sections',
    render: h => h(require('./components/sections/sections.vue.html'))
});

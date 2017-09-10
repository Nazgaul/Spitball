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


const routes = [
    { path: '/', component: require('./components/sections/sections.vue.html') },
    //{ path: '/', component: require('./components/home/home.vue.html') },
    { path: '/counter', component: require('./components/counter/counter.vue.html') },
    { path: '/fetchdata', component: require('./components/fetchdata/fetchdata.vue.html') }
];

//new Vue({
//    el: '#app-root',
//    router: new VueRouter({ mode: 'history', routes: routes }),
//    render: h => h(require('./components/app/app.vue.html'))
//});
var aa=new Vue({
    el: '#sections',
    created: function () {
        this.fetchProducts();
    },
    methods: {
        fetchProducts() {
            {
                this.$http.get('/search').then((response) => {
                    this.products = response.body;
                    console.log("11");
                });
            }
        }
    },
    data() {
        return {
            sectionOpen: false,
            isOpen: false
        }
    },
    render: h => h(require('./components/sections/sections.vue.html'))
});

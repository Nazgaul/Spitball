import Vue from 'vue';
import { Component } from 'vue-property-decorator';
//import SvgSprite from 'vue-svg-sprite';

//Vue.use(SvgSprite, {
//    url: './Images/icons.svg',
//    class: 'icon'
//});
@Component({
    components: {
        AppAside: require('./aside/navbar.vue.html'),
        AppHeader: require('./header/header.vue.html')
    }
})
export default class AppComponent extends Vue {
    data() {
        return {
            customers: [],
            isOpen: false,
            sectionOpen:false,
            search_form: {
                type: '',
                filter: '',
            },

            unsubscribe_form: {
                email: '',
                errors: [],
            }
        }
    }
}


//@Component({
//    components: {
//        aside: require('/aside/navmenu.vue.html'),
//        header: require('/header/header.vue.html')
//    }
//})
//new Vue({
//    components: {
//        'aside': require('/aside/navmenu.vue.html'),
//        'header': require('/header/header.vue.html')}
//})
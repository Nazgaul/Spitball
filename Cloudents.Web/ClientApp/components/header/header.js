import mainHeader from '../helpers/header.vue'
import verticalsTabs from './verticalsTabs.vue'
export default {
    components: {
        mainHeader,verticalsTabs
    },
    computed:{
        hideHeaderMobile(){
            return false;
            // let path = this.$route.name;
            // let isMobile = this.$vuetify.breakpoint.xsOnly;
            // return path === 'question' && isMobile;
        }
    },
    beforeRouteUpdate(to, from, next) {
        //const toName = to.path.slice(1);
        // let tabs = this.$el.querySelector('.v-tabs__wrapper');
        // let currentItem = this.$el.querySelector(`#${toName}`);
        // if (currentItem && !global.isRtl){
        // tabs.scrollLeft = currentItem.offsetLeft - (tabs.clientWidth / 2);
        // }else{
        // tabs.scrollLeft = 0;
        // }

        next();
    },
    mounted() {
        // let tabs = this.$el.querySelector('.v-tabs__wrapper');
        // let currentItem = this.$el.querySelector(`#${this.currentSelection}`);
        // if (currentItem && !global.isRtl){
        //     tabs.scrollLeft = currentItem.offsetLeft - (tabs.clientWidth / 2);
        // }else{
        //     tabs.scrollLeft = 0;
        // }

    },
    props: {
        currentSelection: {type: String},
        submitRoute: {String},
        userText: {String}
    }
}

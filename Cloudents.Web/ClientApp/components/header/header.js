import mainHeader from '../helpers/header.vue'
export default {
    components: {
        mainHeader
    },
    computed:{
        isMobile(){
            return this.$vuetify.breakpoint.xsOnly;
        },
        hideHeaderMobile(){
            // return false;
            if(this.isMobile){
                let filteredRoutes = ['document'];
                return filteredRoutes.indexOf(this.$route.name) > -1;
            }else{
                return false;
            }            
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

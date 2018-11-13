import mainHeader from '../helpers/header.vue'
import verticalsTabs from './verticalsTabs.vue'
export default {
    components: {
        mainHeader,verticalsTabs
    },
    computed:{
        hideHeaderMobile(){
            let path = this.$route.name;
            let isMobile = this.$vuetify.breakpoint.xsOnly;
            return path === 'question' && isMobile;
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
    methods:{
        correctCurrentSelection(){
            let documentPath = this.currentSelection.toLowerCase() === "document" ? "note" : this.currentSelection;
            return documentPath;
        }
    },
    mounted() {
        let tabs = this.$el.querySelector('.v-tabs__wrapper');
        let currentItem = this.$el.querySelector(`#${this.correctCurrentSelection()}`);
        if (currentItem && !global.isRtl){
            tabs.scrollLeft = currentItem.offsetLeft - (tabs.clientWidth / 2);
        }else{
            tabs.scrollLeft = 0;
        }

    },
    props: {
        currentSelection: {type: String},
        submitRoute: {String},
        userText: {String}
    }
}

import questionCard from "./../../question/helpers/question-card/question-card.vue";

export default {
    components: {questionCard},
    data() {
        return {
            activeTab:1
        }
    },
    methods:{    
        changeActiveTab(tabId){
            this.activeTab = tabId;
        }  
    },
    computed:{        
        isMobile(){return this.$vuetify.breakpoint.xsOnly;}
    },
    created(){

    }
}
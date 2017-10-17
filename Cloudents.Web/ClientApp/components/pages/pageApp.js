import appAside from './../navbar/navbar.vue'
import appHeader from './../header/header.vue'
import pageResult from './results/Result.vue'

export default {
    components: {
        'app-aside': appAside, 'app-header': appHeader, pageResult
    },
    
    computed: {
        isLoading: function () { return this.$store.getters.loading }
    },
    methods: {
        begone: function() {
            console.log("hasdasd");
            this.isOverlay = false;
        }

    },
    data: 
        function() {
            return {
                isOverlay: !this.$store.getters.userText
            }
        }
}
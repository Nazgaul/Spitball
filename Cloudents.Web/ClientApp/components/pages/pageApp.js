import Vue from 'vue'
import appAside from './../navbar/navbar.vue'
import appHeader from './../header/header.vue'
import pageResult from './results/Result.vue'

export default {
    components: {
        'app-aside': appAside, 'app-header': appHeader, pageResult
    },
    
    data() {
        return {
            isOpen: "yifat",
            section: ""
        }
    },
    computed: {
        isLoading: function () { return this.$store.getters.loading }
    }
}
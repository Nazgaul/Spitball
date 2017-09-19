import Vue from 'vue'
import appAside from './aside/navbar.vue'
import appHeader from './header/header.vue'
export default {
    components: {
        'app-aside':appAside, 'app-header':appHeader
    },
    
    data() {
        return {
            isOpen: "yifat",
            section: ""
        }
    },
    beforeRouteUpdate(to, from, next) {
        console.log('Reusing this component.')
        console.log(to);
        this.$refs.header.$refs.search.focus()
        //console.log(this.$refs.header.search.focus());
        //this.$refs.search.focus();
        next()
    }
}
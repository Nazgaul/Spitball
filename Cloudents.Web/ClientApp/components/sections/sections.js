import Vue from 'vue'
import appAside from './aside/navbar.vue'
import appHeader from './header/header.vue'
//const appHeader = import('./header/header.vue')
const search=import('./../../api/search')
export default {
    components: {
        'app-aside':appAside, 'app-header':appHeader
    },
    
    data() {
        return {
            isOpen: "yifat",
            section: "",
            result:null
        }
    },
    beforeRouteUpdate(to, from, next) {
        console.log('Reusing this component.')
        console.log(to);
        this.$refs.header.$refs.search.focus()
        //console.log(this.$refs.header.search.focus());
        //this.$refs.search.focus();
        search.getDocument(null, (response) => {
                    console.log(response);
                })
        next()
    }
}
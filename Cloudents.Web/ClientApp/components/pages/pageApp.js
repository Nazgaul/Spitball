﻿import Vue from 'vue'
import appAside from './../navbar/navbar.vue'
import appHeader from './../header/header.vue'

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
    watch: {
        // call again the method if the route changes
        '$route': 'fetchData'
    },
    methods: {
        fetchData() {console.log('route change') }
    },
    beforeRouteUpdate(to, from, next) {
        console.log('Reusing this component.')
        console.log(to);
        this.$refs.header.$refs.search.focus()
        this.$store.dispatch('fetchingData', to);
        //console.log(this.$refs.header.search.focus());
        //this.$refs.search.focus();
        //search.getDocument(null, (response) => {
        //            console.log(response);
        //        })
        next()
    },
    beforeRouteEnter(to, from, next) {
        console.log('before enter');
        //this.$store.dispatch('fetchingData', to);
        next(vm => {
            console.log('boooo')
            vm.$store.dispatch('fetchingData', vm.$route);
        });
    }
}
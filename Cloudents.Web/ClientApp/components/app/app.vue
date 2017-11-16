<template>
    <v-app>
        <app-header v-if="$route.meta.showHeader" :userText.sync="userText"></app-header>
        <app-menu v-if="$route.meta.showSidebar" :term="userText"></app-menu>
        <div v-if="isLoading" class="loader">
            <v-progress-circular indeterminate v-bind:size="50" color="amber"></v-progress-circular>
        </div>
        <router-view v-else></router-view>
    </v-app>
</template>
<script>
    import AppMenu from '../navbar/TheNavbar.vue'
    import AppHeader from '../header/header.vue'
    import {mapGetters} from 'vuex'
    export default {
        data() {
            return { userText: this.$route.query.q }
        },
        computed:{
          ...mapGetters(['isLoading'])
        },
        components: { AppHeader, AppMenu }
    }
</script>


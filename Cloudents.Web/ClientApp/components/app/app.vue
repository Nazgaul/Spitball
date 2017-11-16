<template>
    <v-app>
        <app-header v-if="$route.meta.showHeader" :userText.sync="userText"></app-header>
        <app-menu v-if="$route.meta.showSidebar" :term="userText"></app-menu>
        <!--v-show="!$route.meta.isStatic&&loading"-->
        <div  class="loader">
            <v-progress-circular indeterminate v-bind:size="50" color="amber"></v-progress-circular>
        </div>
        <router-view v-show="!loading||$route.meta.isStatic"></router-view>
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
          ...mapGetters(['loading'])
        },
        components: { AppHeader, AppMenu }
    }
</script>
<style lang="less">
    @import "../../mixin.less";
    .loader {
        .inTheMiddle();
        width: 100vw;
        height: 100vh;
    }
</style>


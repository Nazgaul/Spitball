<template>
    <v-app>
        <!--TODO v-model is not relevant anymore-->
        <app-header v-if="$route.meta.showHeader" :userText.sync="userText" v-model="showMenu"></app-header>
        <!--<app-menu v-if="$route.meta.showSidebar" :term="userText" :isOpen="showMenu" v-model="showMenu"></app-menu>-->
        <div  class="loader" v-show="!$route.meta.isStatic&&loading">
            <v-progress-circular indeterminate v-bind:size="50" color="amber"></v-progress-circular>
        </div>
        <router-view v-show="!loading||$route.meta.isStatic"></router-view>
    </v-app>
</template>
<script>
   // import AppMenu from '../navbar/TheNavbar.vue'
    import AppHeader from '../header/header.vue'
    import {mapGetters} from 'vuex'
    export default {
        data() {
            return {
                userText: this.$route.query.q,
                showMenu: true
            }
        },
        computed:{
          ...mapGetters(['loading'])
        },
        components: { AppHeader }
    }
</script>
<style lang="less" src="./app.less"></style>


<template>
    <v-app>
        <app-header v-if="$route.meta.showHeader" :userText.sync="userText"></app-header>
        <app-menu v-if="$route.meta.showSidebar" :term="userText"></app-menu>
        <v-content v-if="$route.meta.showHeader">
            <v-container fluid class="pa-0">
                <v-layout>
                    <router-view :key="$route.path"></router-view>
                </v-layout>
            </v-container>
        </v-content>
        <router-view v-else></router-view>
    </v-app>
</template>
<script>
    import AppMenu from '../navbar/TheNavbar.vue'
    import AppHeader from '../header/header.vue'
    export default {
        data() {
            return { userText: this.$route.query.q }
        },

        components: { AppHeader, AppMenu }
    }
</script>


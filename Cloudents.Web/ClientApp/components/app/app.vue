<template>
    <v-app>
        <main>
            <app-header v-if="$route.meta.showHeader" :userText.sync="userText"></app-header>
            <app-menu v-if="$route.meta.showSidebar" :term="userText"></app-menu>
            <main v-if="$route.meta.showHeader">
                <v-content>
                    <v-container fluid>
                        <v-layout>                                                        
                                <router-view :key="$route.path"></router-view>
                        </v-layout>
                    </v-container>
                </v-content>
            </main>
                <router-view v-else></router-view>
        </main>
    </v-app>
</template>
<script>
    import AppMenu from '../navbar/TheNavbar.vue'
    import AppHeader from '../header/header.vue'
    export default {
        data() {
            return {userText:this.$route.query.q}
        },

        components: { AppHeader, AppMenu }
    }
</script>
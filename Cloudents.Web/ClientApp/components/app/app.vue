<template>
    <v-app>
        <main>
            <router-view name="header"></router-view>
            <router-view name="navbar"></router-view>
            <main v-if="hasSidebar">
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
    export default{
        computed: {
            hasSidebar() {
                return this.$route.matched[0] &&
                    this.$route.matched[0].components &&
                    !!this.$route.matched[0].components.navbar
            }
        }
    }
</script>
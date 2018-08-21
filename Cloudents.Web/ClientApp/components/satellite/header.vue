<template>
    <main-header ref="mainHeader"
                 :toolbarHeight="$vuetify.breakpoint.smAndUp ? 108 : 104">
        <v-flex class="line" slot="extraHeader">
            <v-layout row>
            <v-tabs :scrollable="false" centered>
                    <v-tab router v-for="tab in verticals" :key="tab.name" :ripple="false" :href="tab.name === 'blog'" v-if="tab.display" class="vertical">
                        <a v-if="tab.name == 'blog'" href="https://medium.com/@spitballstudy" target="_blank">{{tab.display}}</a>
                        <router-link  v-else :to="tab.name">{{tab.display}}</router-link>
                    </v-tab>
                    <v-tabs-slider color="color-dark-blue"></v-tabs-slider>
            </v-tabs>
            </v-layout>
        </v-flex>
    </main-header>
</template>

<style src="./header.less" lang="less"></style>
<script>
    import logo from '../../../wwwroot/Images/logo-spitball.svg';
    import mainHeader from "../helpers/header.vue";
    import { staticRoutes } from "./satellite-routes"
    export default {
        components: {
            logo, mainHeader
        },
        data() {
            return {
                verticals: staticRoutes,
            }
        },
        methods:{

        }
    }
</script>
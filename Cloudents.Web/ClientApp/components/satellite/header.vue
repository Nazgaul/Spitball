<template>
    <main-header ref="mainHeader"
                 :toolbarHeight="$vuetify.breakpoint.smAndUp ? 108 : 104">
        <!--<v-flex slot="extraHeader" fill-height align-end d-flex class="hidden-sm-and-down">-->
            <!--<div  class="gap"></div>-->
        <v-flex class="line verticals" slot="extraHeader" v-if="$vuetify.breakpoint.mdAndUp">
            <v-layout row>
                <!--<div class="gap ma-0" v-if="$vuetify.breakpoint.mdAndUp"></div>-->
            <v-tabs class="verticals-bar" :scrollable="false" centered>
                <v-tabs-bar>
                    <v-tabs-item router v-for="tab in verticals" :key="tab.name" :to="tab.name"
                                 class="vertical">
                        {{tab.display}}
                    </v-tabs-item>
                    <v-tabs-slider color="color-dark-blue"></v-tabs-slider>
                </v-tabs-bar>
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
                verticals: staticRoutes
            }
        }
    }
</script>
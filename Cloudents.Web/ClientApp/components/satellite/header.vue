<template>
    <v-toolbar :height="$vuetify.breakpoint.smAndDown? 128 : 150" class="sat-header" app fixed>
        <v-layout column justify-end fill-height class="pt-3 mx-0">
            <v-flex>
                <v-layout row fill-height>
                    <v-toolbar-title class="ml-2">
                        <router-link :to="{name:'home'}">
                            <logo class="logo"></logo>
                        </router-link>
                    </v-toolbar-title>
                    <sb-search class="search-bar hidden-md-and-down"></sb-search>
                    <v-spacer></v-spacer>
                    <v-flex class="h-menu hidden-md-and-up">
                        <v-menu bottom left>
                            <v-btn flat color="color-blue" icon slot="activator" class="ma-0">
                                <v-icon>sbf-3-dot</v-icon>
                            </v-btn>
                            <v-list>
                                <v-list-tile :to="tab.name" v-for="tab in verticals" :key="tab.name">
                                    {{ tab.display }}
                                </v-list-tile>
                            </v-list>
                        </v-menu>
                    </v-flex>

                </v-layout>
            </v-flex>
            <v-flex fill-height align-end d-flex class="hidden-md-and-down">
                <v-tabs class="verticals-bar" centered :scrollable="false">
                    <v-tabs-bar>
                        <v-tabs-item router v-for="tab in verticals" :key="tab.name" :to="tab.name"
                                     class="vertical">
                            {{tab.display}}
                        </v-tabs-item>
                        <v-tabs-slider color="color-blue"></v-tabs-slider>
                    </v-tabs-bar>
                </v-tabs>
            </v-flex>
            <v-flex class="hidden-md-and-up">
                <sb-search class="search-bar"></sb-search>
            </v-flex>
        </v-layout>
    </v-toolbar>
</template>

<style src="./header.less" lang="less"></style>
<script>
    import logo from '../../../wwwroot/Images/logo-spitball.svg';
    import sbSearch from "../home/search.vue";
    import { staticRoutes } from "./satellite-routes"
    export default {
        components: {
            logo, sbSearch
        },
        data() {
            return {
                verticals: staticRoutes
            }
        }
    }
</script>
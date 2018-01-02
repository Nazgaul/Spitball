<template>
    <v-toolbar :height="$vuetify.breakpoint.smAndDown? 64 : 150" flat class="h-p-header" app fixed>
        <v-toolbar-title class="ml-2">
            <logo class="logo"></logo>
        </v-toolbar-title>
        <v-toolbar-items>
            <sb-search></sb-search>
        </v-toolbar-items>
        <v-tabs class="verticals-bar" :value="currentSelection" >
            <v-tabs-bar>
                <v-tabs-item router v-for="tab in verticals" :key="tab.name" :to="tab.name" 
                             class="mr-4 vertical">
                    {{tab.display}}
                </v-tabs-item>
                <v-tabs-slider color="color-blue"></v-tabs-slider>
            </v-tabs-bar>
        </v-tabs>


    </v-toolbar>
</template>

<script>
    import logo from '../../../wwwroot/Images/logo-spitball.svg';
    import sbSearch from "../home/search.vue";
    import { staticRoutes } from "../../routeTypes"
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
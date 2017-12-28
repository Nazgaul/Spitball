<template>
    <v-toolbar :height="$vuetify.breakpoint.smAndDown? 64 : 150" flat class="h-p-header" :class="{scroll: scrollTop}" :extended="$vuetify.breakpoint.smAndDown && showText" app fixed v-scroll="onScroll">
        <v-toolbar-title class="ml-2">
            <logo class="logo"></logo>
        </v-toolbar-title>
        <v-toolbar-items>
            <sb-search></sb-search>
        </v-toolbar-items>
        <v-tabs class="verticals-bar" :value="currentSelection" :scrollable="false">
            <v-tabs-bar>
                <v-tabs-item v-for="tab in verticals" :key="tab.id" :href="tab.id" :id="tab.id"
                             class="mr-4 vertical">
                    {{tab.name}}
                </v-tabs-item>
                <v-tabs-slider :color="color-blue"></v-tabs-slider>
            </v-tabs-bar>
        </v-tabs>


    </v-toolbar>
</template>

<script>
    import logo from '../../../wwwroot/Images/logo-spitball.svg';
    import sbSearch from "./search.vue";
    export default {
        components: {
            logo, sbSearch
        },
        data() {
            return {
                verticals: [
                    {
                        id: "A",
                        name: "Privacy"
                    },
                    {
                        id: "A",
                        name: "Terms"
                    }
                ]
            }
        }
    }
</script>
<template>
    <v-toolbar :height="$vuetify.breakpoint.smAndDown? 64 : 110" flat class="h-p-header" :class="{scroll: scrollTop}" :extended="$vuetify.breakpoint.smAndDown && showText" app fixed v-scroll="onScroll">
        <v-toolbar-title>
            <logo class="logo mr-5"></logo>
        </v-toolbar-title>
        <v-toolbar-items class="hidden-sm-and-down" v-if="!showText" v-for="action in links" :key="action.name">
            <v-btn class="link-menu" flat href="#">{{ action.name }}</v-btn>
        </v-toolbar-items>
        <v-spacer v-if="$vuetify.breakpoint.smAndDown"></v-spacer>
        <v-menu bottom left v-if="$vuetify.breakpoint.smAndDown">
            <v-btn icon slot="activator">
                <v-icon>sbf-3-dot</v-icon>
            </v-btn>
            <v-list>
                <v-list-tile href="#" v-for="action in links" :key="action.name">
                    {{ action.name }}
                </v-list-tile>
            </v-list>
        </v-menu>
        <!--<transition name="fade">-->
            <sb-search :header-menu="true" v-if="showText" :slot="$vuetify.breakpoint.smAndDown? 'extension' : 'default'"></sb-search>
        <!--</transition>-->
    </v-toolbar>


</template>

<script>
    import logo from '../../../wwwroot/Images/logo-spitball.svg';
    import sbSearch from "./search.vue";
    export default {
        components: {
            logo, sbSearch
        },
        computed: {
            showText: function () {
                return this.scrollTop > 365
            }
        },
        data() {
            return {
                scrollTop: 0,
                links: [
                    {
                        name: "Spitball Guide",
                        link: "#"
                    },
                    {
                        name: "Key Features",
                        link: "#"
                    },
                    //{
                    //    name: "Shared Documents",
                    //    link: "#"
                    //},
                    {
                        name: "Mobile App",
                        link: "#"
                    }
                ]
            }
        },
        methods: {
            onScroll(e) {
                this.scrollTop = window.pageYOffset || document.documentElement.scrollTop;
            }
        }
    }
</script>
<style src="./header.less" lang="less"></style>
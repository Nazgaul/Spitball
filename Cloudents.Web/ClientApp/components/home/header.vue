<template>
    <v-toolbar :height="$vuetify.breakpoint.smAndDown? 64 : 110" flat class="h-p-header" :class="{scroll: scrollTop}" :extended="$vuetify.breakpoint.smAndDown && showText" app :fixed="$vuetify.breakpoint.mdAndUp" v-scroll="onScroll">
        <v-toolbar-title class="ml-2">
            <logo class="logo"></logo>
        </v-toolbar-title>
        <!--<v-toolbar-items class="hidden-sm-and-down" v-if="!showText" v-for="action in links" :key="action.name">-->
            <!--<v-btn class="link-menu" flat href="#">{{ action.name }}</v-btn>-->
        <!--</v-toolbar-items>-->
        <!--<v-spacer v-if="$vuetify.breakpoint.smAndDown"></v-spacer>
        <v-menu bottom left v-if="$vuetify.breakpoint.smAndDown">
            <v-btn flat color="color-blue" icon slot="activator" class="ma-0">
                <v-icon>sbf-3-dot</v-icon>
            </v-btn>
            <v-list>
                <v-list-tile href="#" v-for="action in links" :key="action.name">
                    {{ action.name }}
                </v-list-tile>
            </v-list>
        </v-menu>-->
        <div class="spacing hidden-sm-and-down" v-if="showText"></div>
        <sb-search v-if="$vuetify.breakpoint.smAndDown || showText" :slot="$vuetify.breakpoint.smAndDown? 'extension' : 'default'"></sb-search>
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
                return screen.height > 768 ? this.scrollTop > 275 : this.scrollTop > 215
            }
        },
        data() {
            return {
                scrollTop: 0,
                //links: [
                //    {
                //        name: "Spitball Guide",
                //        link: "#"
                //    },
                //    {
                //        name: "Key Features",
                //        link: "#"
                //    },
                //    //{
                //    //    name: "Shared Documents",
                //    //    link: "#"
                //    //},
                //    {
                //        name: "Mobile App",
                //        link: "#"
                //    }
                //]
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
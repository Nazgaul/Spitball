<template>
    <v-toolbar height="110" flat class="h-p-header" :class="{scroll: scrollTop}" app fixed slot="header" v-scroll="onScroll">
        <v-toolbar-title>
            <logo class="logo mr-3"></logo>
        </v-toolbar-title>
        <v-toolbar-items class="hidden-sm-and-down" v-if="!scrollTop" v-for="action in links" :key="action.name">
            <v-btn class="link-menu" flat href="#">{{ action.name }}</v-btn>
            <!--<a href="#">{{ action.name }}</a>-->
        </v-toolbar-items>
        <sb-search v-if="scrollTop"></sb-search>
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
                scrollTop: false,
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
                this.scrollTop = window.pageYOffset || document.documentElement.scrollTop > 100;
            }
        }
    }
</script>
<style src="./header.less" lang="less"></style>
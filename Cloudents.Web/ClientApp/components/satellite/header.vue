<template>
<div>
    <main-header class="satellite-header" ref="mainHeader" :userText="userText" :submitRoute="submitRoute" :currentSelection="currentSelection"></main-header>
    <v-flex class="line sat-header-container" slot="extraHeader">
            <v-layout row wrap justify-center>
            <!--RTL bug fix in hebrew mobile-->
            <v-tabs class="satellite-header-tabs" :scrollable="false" centered :dir="isRtl && $vuetify.breakpoint.xsOnly ? `ltr` : ''">
                <v-tab router :to="verticals[0].name" :ripple="false" class="vertical sattelite-vertical">{{verticals[0].display}}</v-tab>
                <v-tab router :to="verticals[1].name" :ripple="false" class="vertical sattelite-vertical">{{verticals[1].display}}</v-tab>
                <!--special handler for blog open in new browser window-->
                <v-tab :ripple="false" class="vertical sattelite-vertical" @click="openBlog()">{{verticals[2].display}}</v-tab>
                <v-tab router :to="verticals[3].name" :ripple="false" class="vertical sattelite-vertical">{{verticals[3].display}}</v-tab>
                <v-tab router :to="verticals[4].name" :ripple="false" class="vertical sattelite-vertical">{{verticals[4].display}}</v-tab>
                <v-tab router :to="verticals[5].name" :ripple="false" class="vertical sattelite-vertical">{{verticals[5].display}}</v-tab>
                <v-tab router :to="verticals[6].name" :ripple="false" class="vertical sattelite-vertical">{{verticals[6].display}}</v-tab>
                <v-tab router :to="verticals[7].name" :ripple="false" class="vertical sattelite-vertical">{{verticals[7].display}}</v-tab>
                <v-tabs-slider color="color-dark-blue"></v-tabs-slider>
            </v-tabs>
        </v-layout>
    </v-flex>
</div>
    
</template>

<style src="./header.less" lang="less"></style>
<script>
    import logo from '../../../wwwroot/Images/logo-spitball.svg';
    import mainHeader from "../helpers/header.vue";
    import { staticRoutes } from "./satellite-routes"
    export default {
        props: {
            currentSelection: {type: String},
            submitRoute: {String},
            userText: {String}
        },
        components: {
            logo, mainHeader
        },
        data() {
            return {
                verticals: staticRoutes,
                isRtl: global.isRtl
            }
        },
        methods:{
            openBlog(){
                let currentRoute = this.$route.path;
                let Rout1 = '/about';
                let Rout2 = '/contact';
                if(currentRoute !== Rout1){
                    this.$router.push({path: Rout1});
                }else{
                    this.$router.push({path: Rout2});
                }
                setTimeout(()=>{
                    this.$router.push({path: currentRoute});

                }, 200);
                window.open('https://medium.com/@spitballstudy', '_blank');
            }
        }
    }
</script>
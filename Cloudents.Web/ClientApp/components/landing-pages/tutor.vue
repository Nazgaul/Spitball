<template>
    <div class="landing-page-wrapper" :style="'background-image: url('+ require(`./img/${contentObj.background}`)+')'">
        <div class="landing-page">

            <router-link class="logo-link" :to="{name:'home'}">
                <app-logo class="logo"></app-logo>
            </router-link>
            <div class="page-data">
                <h1 v-html="contentObj.titleHtml"></h1>
                <div class="content-wrapper">
                    <h2 v-html="contentObj.bodyHtml"></h2>
                    <p class="hidden-xs-only">Fill out the field and start spitballing!</p>
                    <div class="search-wrapper">
                        <v-text-field class="search-b" type="search" solo
                                      @keyup.enter="search" autocomplete="off"
                                      required name="q"
                                      v-model.trim="msg" placeholder="What do you need help with?"></v-text-field>
                        <button @click="search">Start Spitballing</button>
                    </div>
                </div>
            </div>
        </div>

    </div>
</template>

<script>
    import AppLogo from "../../../wwwroot/Images/logo-spitball.svg";
    import {tutorV1} from "./consts.js";
    import {tutorV2} from "./consts.js";

    export default {
        data: () => ({
            msg: '',
            contentObj: null
        }),
        components: {
            AppLogo
        },
        methods: {
            search() {
                this.$router.push({path: "/tutor", query: {q: this.msg}});
            }
        },
        created() {
            this.contentObj = this.$route.name === 'landingTutorV1' ? tutorV1 : tutorV2;
        }
    }
</script>
<style src="./tutor.less" lang="less"></style>


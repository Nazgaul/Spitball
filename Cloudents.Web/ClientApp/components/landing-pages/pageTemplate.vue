<template>
    <div class="landing-page-wrapper" :class="[contentObj.wrappingClass, {collapse: collapsePage}]"
         :style="'background-image: url('+ require(`./img/${contentObj.background}`)+')'">
        <div class="landing-page">

            <router-link class="logo-link collapsible" :to="{name:'home'}">
                <app-logo class="logo"></app-logo>
            </router-link>
            <div class="page-data">
                <h1 v-html="contentObj.titleHtml" class="collapsible"></h1>
                <div class="content-wrapper">
                    <h2 v-html="contentObj.bodyHtml" class="collapsible"></h2>
                    <p class="hidden-xs-only collapsible">Fill out the field and start spitballing!</p>
                    <div class="search-wrapper">
                        <form action="." method="get" @submit.prevent="search">
                            <search-input @openedSuggestions="toggleCollapse" class="term-field mb-3" :placeholder="contentObj.placeholders.term" v-model="msg" :search-on-selection="contentObj.name !=='note'"></search-input>
                            <search-input @openedSuggestions="toggleCollapse" class="uni-field mb-3" :disabled="!msg.length" :placeholder="contentObj.placeholders.uni"
                                          v-if="contentObj.name ==='note'" search-type="uni"
                                          v-model="uni" :search-on-selection="true"></search-input>
                            <button type="submit" class="ma-0">Start Spitballing</button>
                        </form>
                    </div>
                </div>
            </div>
        </div>

    </div>
</template>

<script>
    import AppLogo from "../../../wwwroot/Images/logo-spitball.svg";
    import {landingPagesData} from "./consts.js";
    import SearchInput from '../helpers/searchInput.vue';

    export default {
        data: () => ({
            msg: '',
            uni: '',
            contentObj: null,
            uniDisabled: true,
            collapsePage: false
        }),
        components: {
            AppLogo, SearchInput
        },
        methods: {
            search() {
                this.contentObj.submissionCallback(this.msg);
            },
            toggleCollapse(val){
                this.collapsePage = val;
            }
        },
        created() {
            this.contentObj = landingPagesData[this.$route.name];
        },
    }
</script>
<style src="./style.less" lang="less"></style>


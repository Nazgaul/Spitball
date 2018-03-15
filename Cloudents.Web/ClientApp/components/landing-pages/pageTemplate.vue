<template>
    <div class="landing-page-wrapper" :class="contentObj.wrappingClass"
         :style="'background-image: url('+ require(`./img/${contentObj.background}`)+')'">
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
                        <search-input :placeholder="contentObj.placeholder" search-type="uni" v-model="msg"></search-input>

                        <form action="." method="get" @submit.prevent="search">
                            <search-input :placeholder="contentObj.placeholder" v-model="msg"></search-input>
                            <button type="submit">
                                <v-icon class="hidden-md-and-up">sbf-search</v-icon>
                                <span class="hidden-sm-and-down">Start Spitballing</span>
                            </button>
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
            contentObj: null
        }),
        components: {
            AppLogo, SearchInput
        },
        methods: {
            search() {
                this.contentObj.submissionCallback(this.msg);
            }
        },
        created() {
            this.contentObj = landingPagesData[this.$route.name];
        }
    }
</script>
<style src="./style.less" lang="less"></style>


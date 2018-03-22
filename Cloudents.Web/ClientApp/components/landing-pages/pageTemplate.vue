<template>
    <div class="landing-page-wrapper" :class="[contentObj.wrappingClass, {collapse: collapsePage}, {'no-bg':!contentObj.background}]">
        <div class="top" :style="contentObj.background ? 'background-image: url('+ require(`./img/${contentObj.background}`)+')': ''">
        <router-link class="logo-link collapsible" :to="{name:'home'}">
            <app-logo class="logo"></app-logo>
        </router-link>
        <div class="landing-page">

            <div class="page-data">
                <h1 v-html="contentObj.titleHtml" class="collapsible"></h1>
                <div class="search-wrapper">
                    <form action="." method="get" @submit.prevent="search">
                        <search-input @openedSuggestions="toggleCollapse" class="term-field mr-3"
                                      :placeholder="contentObj.placeholder" v-model="msg"
                                      :search-on-selection="contentObj.name !=='note'"></search-input>
                        <button type="submit" class="ma-0">{{contentObj.submitButtonText}}</button>
                    </form>
                </div>
                <div class="content-wrapper">
                    <h2 v-html="contentObj.bodyHtml" class="collapsible"></h2>
                </div>
            </div>
        </div>
        </div>
        <footer>
            <div class="partners">
                <img :src="contentObj.background ? require('./img/partners.png'): require('./img/partners2.png')"/>
                <div class="text">Our Partners</div>
                <div class="logos">
                    <img v-for="image in contentObj.partnersImages" :src="image"/>
                </div>
            </div>
            <div class="subfooter">
                <span class="copyright">Copyright</span><span class="c-icon">©</span><span>Cloudents 2018</span>
                <router-link to="/work">How Spitball Works</router-link>
                <router-link to="/privacy">Privacy Policy</router-link>
                <router-link to="/terms">Terms of Service</router-link>
                <router-link to="/faq">FAQ</router-link>

            </div>
        </footer>
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
                debugger;
                this.contentObj.submissionCallback(this.msg);
            },
            toggleCollapse(val) {
                this.collapsePage = val;
            }
        },
        created() {
            this.contentObj = landingPagesData[this.$route.name];
        },
    }
</script>
<style src="./style.less" lang="less"></style>


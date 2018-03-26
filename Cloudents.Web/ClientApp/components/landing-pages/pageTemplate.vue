<template>
    <div class="landing-page-wrapper"
         :class="['landing-'+contentObj.type, contentObj.wrappingClass, {collapse: collapsePage}, {'no-bg':!contentObj.background}]">
        <div class="top"
             :style="contentObj.background ? 'background-image: url('+ require(`./img/${contentObj.background}`)+')': ''">
            <div class="limited-width logo-wrap">
                <router-link class="logo-link collapsible" :to="{name:'home'}">
                    <app-logo class="logo"></app-logo>
                </router-link>
            </div>
            <div class="limited-width landing-page">

                <div class="page-data">
                    <h1 v-html="contentObj.titleHtml" class="collapsible"></h1>
                    <div class="search-wrapper">
                        <form action="." method="get" @submit.prevent="search">
                            <search-input v-if="contentObj.type !== 'notes'" class="term-field"
                                          :placeholder="contentObj.placeholder" v-model="msg"
                                          :search-on-selection="contentObj.name !=='note'"
                                          :searchVertical="contentObj.type" :submitRoute="'/'+contentObj.name"></search-input>
                            <search-input v-else class="uni-field" :placeholder="contentObj.placeholder"
                                          search-type="uni"
                                          v-model="uni" submitRoute="/note"></search-input>
                            <button type="submit" class="ma-0">
                                <v-icon class="hidden-md-and-up">sbf-search</v-icon>
                                <span class="hidden-sm-and-down">{{contentObj.submitButtonText}}</span>
                            </button>
                        </form>
                    </div>
                    <div class="content-wrapper">
                        <h2 v-html="contentObj.bodyHtml" class="collapsible"></h2>
                    </div>
                </div>
            </div>
            <img class="bottom-image" :src="require(`./img/${contentObj.bottomImage}`)"/>
        </div>
        <footer>
            <div class="partners">
                <div class="text">Our Partners</div>
                <div class="logos">
                    <img v-for="image in contentObj.partnersImages" :class="image.name" :src="image.source"/>
                </div>
            </div>
            <div class="subfooter">
                <span class="copyright">Copyright <span class="c-icon">©</span> Cloudents 2018</span>
                <div class="links">
                    <router-link to="/work">How Spitball Works</router-link>
                    <router-link to="/privacy">Privacy Policy</router-link>
                    <router-link to="/terms">Terms of Service</router-link>
                    <router-link to="/faq">FAQ</router-link>
                </div>
            </div>
        </footer>
    </div>

</template>

<script>
    import AppLogo from "../../../wwwroot/Images/logo-spitball.svg";
    import {landingPagesData} from "./consts.js";
    import SearchInput from '../helpers/searchInput.vue';
    import { router } from "../../main";

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
                if (this.msg) {
                    router.push({path: "/"+this.contentObj.name, query: {q: this.msg}});
                }
            }
        },
        created() {
            this.contentObj = landingPagesData[this.$route.name];
        },
    }
</script>
<style src="./style.less" lang="less"></style>


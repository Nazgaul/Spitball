<template>
    <div class="landing-page-wrapper"
         :class="['landing-'+contentObj.name, contentObj.wrappingClass, {'no-bg':!contentObj.background}]">
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
                        <search-input v-if="contentObj.name !== 'note'" class="term-field"
                                      :placeholder="contentObj.placeholder" v-model="msg"
                                      :search-on-selection="contentObj.name !=='note'"
                                      :searchVertical="contentObj.name"
                                      :submitRoute="'/'+contentObj.name">
                            <button slot="searchBtn" slot-scope="props" @click="props.search">
                                <v-icon class="hidden-md-and-up">sbf-search</v-icon>
                                <span class="hidden-sm-and-down">{{contentObj.submitButtonText}}</span>
                            </button>
                        </search-input>
                        <uni-search-input v-else class="uni-field" submitRoute="/note" :placeholder="contentObj.placeholder" v-model="msg">
                            <button slot="searchBtn" slot-scope="props" @click="props.search">
                                <v-icon class="hidden-md-and-up">sbf-search</v-icon>
                                <span class="hidden-sm-and-down">{{contentObj.submitButtonText}}</span>
                            </button>
                        </uni-search-input>
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
    import UniSearchInput from '../helpers/uniSearchInput.vue';
    import {router} from "../../main";

    export default {
        data: () => ({
            contentObj: null,
            msg: ''
        }),
        components: {
            AppLogo, SearchInput, UniSearchInput
        },
        methods: {
            search() {
                if (this.msg) {
                    if (this.contentObj.name === 'note') {
                        router.push({path: "/" + this.contentObj.name, query: ''});
                    }
                    else {
                        router.push({path: "/" + this.contentObj.name, query: {q: this.msg}});
                    }
                }
            }
        },
        created() {
            this.contentObj = landingPagesData[this.$route.name];
        },
    }
</script>
<style src="./style.less" lang="less"></style>


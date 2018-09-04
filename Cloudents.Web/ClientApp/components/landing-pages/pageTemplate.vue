<template>
    <div class="landing-page-wrapper"
         :class="['landing-'+pageData.name, pageData.wrappingClass, {'no-bg':!pageData.background}]">
        <div class="top"
             :style="pageData.background ? 'background-image: url('+ require(`./img/${pageData.background}`)+')': ''">
            <div class="limited-width logo-wrap">
                <router-link class="logo-link collapsible" :to="{name:'home'}">
                    <app-logo class="logo"></app-logo>
                </router-link>
            </div>
            <div class="limited-width landing-page">

                <div class="page-data">
                    <h1 v-html="pageData.titleHtml" class="collapsible"></h1>
                    <div class="search-wrapper">
                        <search-input v-if="pageData.name !== 'note'" class="term-field"
                                      :placeholder="pageData.placeholder" v-model="msg"
                                      :submitRoute="'/'+pageData.name" :suggestion-vertical="pageData.name">
                            <button slot="searchBtn" slot-scope="props" @click="props.search">
                                <v-icon class="hidden-md-and-up">sbf-search</v-icon>
                                <span class="hidden-sm-and-down">{{pageData.submitButtonText}}</span>
                            </button>
                        </search-input>
                        <uni-search-input v-else class="uni-field" submitRoute="/note" :placeholder="pageData.placeholder" v-model="msg">
                            <button slot="searchBtn" slot-scope="props" @click="props.search">
                                <v-icon class="hidden-sm-and-up">sbf-search</v-icon>
                                <span class="hidden-xs-only">{{pageData.submitButtonText}}</span>
                            </button>
                        </uni-search-input>
                    </div>
                    <div class="content-wrapper">
                        <h2 v-html="pageData.bodyHtml" class="collapsible"></h2>
                    </div>
                </div>
            </div>
            <img class="bottom-image" :src="require(`./img/${pageData.bottomImage}`)"/>
        </div>
        <footer>
            <div class="partners">
                <div class="text" v-language:inner>pageTemple_our_partners</div>
                <div class="logos">
                    <img v-for="image in pageData.partnersImages" :class="image.name" :src="image.source"/>
                </div>
            </div>
            <div class="subfooter">
                <span class="copyright"><span v-language:inner>pageTemplate_copy_righy</span>  <span class="c-icon">©</span> <span v-language:inner>pageTemplate_cloudents_2018</span> </span>
                <div class="links">
                    <router-link to="/work" v-language:inner>ageTemplate_How_spitball_works</router-link>
                    <router-link to="/privacy" v-language:inner>pageTemplate_privacy_policy</router-link>
                    <router-link to="/terms" v-language:inner>Terms of Service</router-link>
                    <router-link to="/faq" v-language:inner>FAQ</router-link>
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
    // import {router} from "../../main";

    export default {
        data: () => ({
            msg: ''
        }),
        components: {
            AppLogo, SearchInput, UniSearchInput
        },
        // props:{name:{String}},
        methods: {
            search() {
                if (this.msg) {
                    if (this.pageData.name === 'note') {
                        this.$router.push({path: "/" + this.pageData.name, query: ''});
                    }
                    else {
                        this.$router.push({path: "/" + this.pageData.name, query: {q: this.msg}});
                    }
                }
            }
        },
        computed: {
            pageData(){return landingPagesData[this.$route.name];}
        },
    }
</script>
<style src="./style.less" lang="less"></style>


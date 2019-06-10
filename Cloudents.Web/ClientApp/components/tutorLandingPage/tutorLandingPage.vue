<template>
    
    <v-container class="tutor-landing-page-container">
        <top-nav></top-nav>
        <v-layout :class="`${isMobile ? 'pt-2 pb-5' : 'pt-1 pb-3'}`" px-4 class="tutor-landing-page-header" align-center justify-center column>
            <v-flex pt-4 pb-3>
                <h1 v-language:inner="'tutorListLanding_header_get_lesson'"></h1>
            </v-flex>
            <v-flex pb-4>
                <h2 v-language:inner="'tutorListLanding_header_find_tutors'"></h2>
            </v-flex>
            <v-flex :class="{'pb-4': !isMobile}">
                <h3><span v-language:inner="'tutorListLanding_rates'"></span> <v-icon v-for="n in 5" :key="n" class="tutor-landing-page-star">sbf-star-rating-full</v-icon> <span v-language:inner="'tutorListLanding_reviews'"></span></h3>
            </v-flex>
            <div class="tutor-search-container">
                <tutor-search-component></tutor-search-component>
            </div>
        </v-layout>
        <v-layout class="tutor-landing-page-body" column>
            <v-flex class="tutor-landing-page-empty-state">
                <empty-state-card v-if="items.length === 0 && query.term" style="margin: 0 auto;" :userText="query.term"></empty-state-card>
            </v-flex>
            <v-flex class="tutor-landing-card-container" v-for="(item, index) in items" :key="index">
                <tutor-result-card class="mb-3 hidden-xs-only" :fromLandingPage="true" :tutorData="item"></tutor-result-card>
                <tutor-result-card-mobile class="mb-2 hidden-sm-and-up" :fromLandingPage="true" :tutorData="item"></tutor-result-card-mobile>
            </v-flex>            
        </v-layout>
        <v-layout align-center py-5 justify-space-around class="tutor-landing-status-row">
            <span class="hidden-xs-only"><span v-language:inner="'tutorListLanding_rates'"></span> <v-icon v-for="n in 5" :key="n" class="tutor-landing-page-star">sbf-star-rating-full</v-icon> <span v-language:inner="'tutorListLanding_reviews'"></span></span>
            <span class="hidden-xs-only" v-language:inner="'tutorListLanding_courses'"></span>
            <span v-language:inner="'tutorListLanding_tutors'"></span>
        </v-layout>
        <v-layout>

        </v-layout>
        <Footer></Footer>
    </v-container>
</template>

<script>
import tutorResultCard from '../results/tutorCards/tutorResultCard/tutorResultCard.vue';
import tutorResultCardMobile from '../results/tutorCards/tutorResultCardMobile/tutorResultCardMobile.vue';
import topNav from '../landingPageTools/TopNav.vue'
import Footer from '../landingPageTools/Footer.vue'
import tutorSearchComponent from './components/tutorSearchInput/tutorSearchInput.vue'
import tutorLandingPageService from './tutorLandingPageService'
import emptyStateCard from '../results/emptyStateCard/emptyStateCard.vue'

export default {
    components:{
        tutorResultCard,
        tutorResultCardMobile,
        topNav,
        Footer,
        tutorSearchComponent,
        emptyStateCard
    },
    data(){
        return {
            items: [],
            query: {
                term: ''
            },
        }
    },
    computed:{
        isMobile(){
            return this.$vuetify.breakpoint.xsOnly;
        }
    },
    watch:{
        '$route'(val){
            // console.log(val.query.term)
            this.query.term = val.query.term;
            this.updateList();
        }
    },
    methods:{
        updateList(){
                tutorLandingPageService.getTutorList(this.query).then(data=>{
                this.items = data;
            })
        }
    },
    created(){
        this.query.term = !!this.$route.query && !!this.$route.query.term ? this.$route.query.term : '';
        this.updateList();
    }
}
</script>

<style lang="less">
@import "../../styles/mixin.less";
.tutor-landing-page-container{
    max-width: 100%;
    padding: 0;
    margin: 0;
    .tutor-landing-page-star{
        color:#ffca54;
        font-size: 20px;
    }
    .tutor-landing-page-header{
        position: relative;
        background-color: #1b2441;
        h1{
            color: #3dc2ba;
            font-size: 35px;
            font-weight: bold;
            @media (max-width: @screen-xs) {
                font-size: 32px;
            }
        }
        h2{
            font-size: 25px;
            font-weight: bold;
            color: #ffffff;
            @media (max-width: @screen-xs) {
                font-size: 16px;
            }
        }
        h3{
            font-size: 18px;
            font-weight: 600;
            color: rgba(255, 255, 255, 0.87);
            @media (max-width: @screen-xs) {
                font-size: 16px;
            }
        }
        .tutor-search-container{
            width: 740px;
            position: absolute;
            bottom: -26px;
            box-shadow: 0 7px 13px 0 rgba(0, 0, 0, 0.28);
            border-radius: 4px;
            @media (max-width: @screen-xs) {
                width: 90%;
            }
        }
    }
    .tutor-landing-page-body{
        margin-top: 15px;
        .tutor-landing-page-empty-state{
            margin: 35px 0;
            @media (max-width: @screen-xs) {
                margin: 45px 6px 25px;
            }
        }
        .tutor-landing-card-container{
            margin: 0 auto;
            max-width: 900px;
            @media (max-width: @screen-xs) {
                margin: 0 8px;
            }
        }
    }
    .tutor-landing-status-row{
        background-color:#FFF;
        padding: 0 290px;
        @media (max-width: @screen-md) {
            padding: 0;
        }
        
        span{
            font-size: 22px;
            font-weight: 600;
            color: rgba(0, 0, 0, 0.87);
        }
    }
}
</style>

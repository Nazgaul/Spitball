<template>
    <v-container class="tutor-landing-page-container">
        <v-layout :class="`${isMobile ? 'pt-2 pb-5' : 'pt-1 pb-4'}`" px-4 class="tutor-landing-page-header" align-center justify-center column>
            <v-flex pt-4 pb-4>
                <h1 v-language:inner="'tutorListLanding_header_get_lesson'"></h1>
            </v-flex>
            <v-flex pb-4>
                <h2 v-language:inner="'tutorListLanding_header_find_tutors'"></h2>
            </v-flex>
            <!-- <v-flex class="pb-6">
                <h3><span v-language:inner="'tutorListLanding_rates'"></span>&nbsp; <v-icon v-for="n in 5" :key="n" class="tutor-landing-page-star">sbf-star-rating-full</v-icon>&nbsp; <span v-language:inner="'tutorListLanding_reviews'"></span></h3> -->
            <v-flex :class="{'pb-6': !isMobile}">
                <h3><span>95%</span>&nbsp; <v-icon v-for="n in 5" :key="n" class="tutor-landing-page-star">sbf-star-rating-full</v-icon>&nbsp; <span v-language:inner="'tutorListLanding_reviews'"></span></h3>
            </v-flex>
        </v-layout>
        <v-layout class="tutor-landing-page-search" :class="{'sticky-active': activateSticky}" align-center justify-center>
            <div class="tutor-search-container" :class="{'sticky-active': activateSticky, 'strech': activateStickyMobile}">
                <tutor-search-component></tutor-search-component>
            </div>
        </v-layout>

        <v-layout column wrap class="tutor-landing-page-body">
            <v-flex class="tutor-landing-page-empty-state">
                <suggest-card v-if="items.length === 0 && query.term && showEmptyState" 
                @click.native="openRequestTutor()" :name="'tutor-list'"></suggest-card>  
            </v-flex>
            <v-flex class="tutor-landing-card-container" v-for="(item, index) in items" :key="index">
                <tutor-result-card v-if="!isMobile" class="mb-4 " :fromLandingPage="true" :tutorData="item"></tutor-result-card>
                <tutor-result-card-mobile v-else class="mb-2 " :fromLandingPage="true" :tutorData="item"/>
            </v-flex>   
        </v-layout>
        <div class="tutorLandingPage_pagination" v-if="items.length && pagination.length > 1">
            <v-pagination
                    total-visible=7 
                    v-model="pagination.current" 
                    :length="pagination.length"
                    @next="goNext"
                    @input="goSelected"
                    @previous="goPrevious"
                    :next-icon="`sbf-arrow-right-carousel`"
                    :prev-icon="`sbf-arrow-left-carousel`"/>
        </div>
        <!-- <v-layout align-center py-12 justify-space-around class="tutor-landing-status-row">
            <span class="hidden-xs-only"><span v-language:inner="'tutorListLanding_rates'"></span>&nbsp; <v-icon v-for="n in 5" :key="n" class="tutor-landing-page-star">sbf-star-rating-full</v-icon>&nbsp; <span v-language:inner="'tutorListLanding_reviews'"></span></span> -->
        <v-layout align-center py-12 justify-space-around class="tutor-landing-status-row">
            <span class="hidden-xs-only"><span>95%</span>&nbsp; <v-icon v-for="n in 5" :key="n" class="tutor-landing-page-star">sbf-star-rating-full</v-icon>&nbsp; <span v-language:inner="'tutorListLanding_reviews'"></span></span>
            <span class="hidden-xs-only" v-language:inner="'tutorListLanding_courses'"></span>
            <span v-language:inner="'tutorListLanding_tutors'"></span>
        </v-layout>
        <v-layout class="tutor-landing-card-bottom" v-if="getHPReviews.length">
            <div class="testimonialCarousel-tutorList" :style="{'pointer-events':$vuetify.breakpoint.smAndDown?'':'none'}">
                <sbCarousel :slideStep="1" :overflow="true" :arrows="!isMobile">
                    <testimonialCard v-for="(item, index) in getHPReviews" :item="item" :key="index"/>
                </sbCarousel>
            </div>
        </v-layout>
    </v-container>
</template>

<script>
import tutorResultCard from '../results/tutorCards/tutorResultCard/tutorResultCard.vue';
import tutorResultCardMobile from '../results/tutorCards/tutorResultCardMobile/tutorResultCardMobile.vue';
import tutorSearchComponent from './components/tutorSearchInput/tutorSearchInput.vue';
import tutorLandingPageService from './tutorLandingPageService';
// import emptyStateCard from '../results/emptyStateCard/emptyStateCard.vue';
import SuggestCard from '../results/suggestCard.vue';
import analyticsService from '../../services/analytics.service.js';

import sbCarousel from '../sbCarousel/sbCarousel.vue';
import testimonialCard from '../carouselCards/testimonialCard.vue';

import { mapActions,mapGetters } from 'vuex'
export default {
    components:{
        tutorResultCard,
        tutorResultCardMobile,
        tutorSearchComponent,
        // emptyStateCard,
        SuggestCard,
        sbCarousel,
        testimonialCard
    },
    data(){
        return {
            pagination:{
                length:0,
                current:1
            },
            items: [],
            query: {
                term: '',
                page: 0,
                pageSize: 10,
            },
            showEmptyState: false,
            topOffset: 0
        }
    },
    computed:{
        ...mapGetters(['getHPReviews']),
        isMobile(){
            return this.$vuetify.breakpoint.xsOnly;
        },
        activateSticky(){
            if(!this.isMobile){
                return this.topOffset > 240;
            }else{
                return false
            }
        },
        activateStickyMobile(){
            if(this.isMobile){
                return this.topOffset > 280;
            }else{
                return false
            }
        }
    },
    watch:{
        '$route'(val){
            if(!!val.query && !!val.query.size){
            this.query.pageSize = val.query.size;
            }
            if(!!val.params && !!val.params.course){
                this.query.term = (!!val.params && !!val.params.course) ? val.params.course : '';
            }
            if(!!val.query){
                this.query.page = val.query.page || 0;
                this.pagination.current = +val.query.page+1 || 1
                this.query.term = (!!val.params && !!val.params.course) ? val.params.course : '';
            }
            this.updateList();
        }
    },
    methods:{
        ...mapActions(['setTutorRequestAnalyticsOpenedFrom','updateRequestDialog','updateHPReviews']),
        updateList(){            
            this.showEmptyState = false;
            let self = this;
            tutorLandingPageService.getTutorList(this.query).then(data=>{
                self.items = data.result;
                self.pagination.length = Math.ceil(data.count / self.query.pageSize)
                self.showEmptyState = true;
            })
        },
        goNext(){
            this.showEmptyState = false;
            this.$router.push({
                path: `/tutor-list/${this.query.term}`,
                query: {
                    page: this.pagination.current -1,
                },
                params:{
                    course: this.query.term
                }
            })
        },
        goPrevious(){
            this.showEmptyState = false;
             this.$router.push({
                path: `/tutor-list/${this.query.term}`,
                query: {
                    page: this.pagination.current -1,
                },
                params:{
                    course: this.query.term
                }
            })
        },
        goSelected(){
            this.showEmptyState = false;
            this.$router.push({
                path: `/tutor-list/${this.query.term}`,
                query: {
                    page: this.pagination.current -1,
                },
                params:{
                    course: this.query.term
                }
            })
        },
        openRequestTutor() {
            analyticsService.sb_unitedEvent('Tutor_Engagement', 'request_box');
            this.setTutorRequestAnalyticsOpenedFrom({
                component: 'suggestCard',
                path: this.$route.path
            });
            this.updateRequestDialog(true);
        }
    },
    mounted() {
        if(!!this.$route.query && !!this.$route.query.size){
            this.query.pageSize = this.$route.query.size;
        }
    },
    created(){
        if(!!this.$route.params && !!this.$route.params.course){
            this.query.term = (!!this.$route.params && !!this.$route.params.course) ? this.$route.params.course : '';
        }
        if(!!this.$route.query){
            this.query.page = this.$route.query.page || 0;
            this.pagination.current = +this.$route.query.page+1 || 1
        }
        this.updateList();
        this.updateHPReviews()
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
            text-align: center;
            color: #5158af;
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
    }
    .tutor-landing-page-search{
        position: -webkit-sticky;
        position: -moz-sticky;
        position: -ms-sticky;
        position: -o-sticky;
        position: sticky;
        // z-index: 240;
        @media (max-width: @screen-xs) {
            z-index: unset;
        }

        // top:30px;
        // top:30px;
        // z-index: 99;
        &.sticky-active{
            position: fixed;
            top:70px;
            background: #fff;
            width: 100%;
            height: 70px;
            box-shadow: 0 7px 13px 0 rgba(0, 0, 0, 0.28);
        }
        .tutor-search-container{
            width: 90%;
            max-width: 740px;
            position: absolute;
            bottom: -26px;
            box-shadow: 0 7px 13px 0 rgba(0, 0, 0, 0.28);
            border-radius: 4px;
            &.sticky-active{
                bottom: 6px;
                box-shadow: unset;
                border: 1px solid #b4b4b4;
            }
            // @media (max-width: @screen-xs) {
            //     &.strech{
            //     //    width: 100%;
            //     }
            // }
        }
    }
    .tutorLandingPage_pagination{
        padding: 20px 0;
        text-align: center;
        button{
            outline: none;
        }
        .v-pagination__item{
            background-color: initial !important;
            box-shadow: none !important;
            &.v-pagination__item--active{
                color: initial !important;
                background-color: initial !important;
                border: none !important;
                border: 1px solid rgb(68, 82, 252) !important;
            }
        }

        .v-pagination__navigation{
            background-color: initial !important;
            box-shadow: none !important;
            i{
                transform: scaleX(1)/*rtl:scaleX(-1)*/; 
                color: rgb(68, 82, 252) !important;
                font-size: 16px;
            }
        }
    }
    .tutor-landing-page-body{
        .responsive-property(margin-top, 15px, null, 0px);

        .tutor-landing-page-empty-state{
            max-width: 920px; 
            width:100%;
            margin: 35px auto;
            @media (max-width: @screen-xs) {
                margin: 25px 16px;
                max-width: 94%;
            }
        }
        .tutor-landing-card-container{
            margin: 0 auto;
            width: 100%;
            max-width: 920px;
            @media (max-width: @screen-xs) {
                padding: 0;
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
    .tutor-landing-card-bottom {
        overflow: -webkit-paged-y;
        height: 528px;
        padding: 0 385px;
        display: flex;
        align-items: center;
        background-color: #fbfbfb;
        @media (max-width: @screen-lg) {
            padding: 0 150px;
            height: auto;
        }
        @media (max-width: @screen-md) {
            padding: 0 70px;
            height: auto;
        }
        @media (max-width: @screen-sm) {
            padding: 0 30px;
            height: auto;
        }
        @media (max-width: @screen-xs) {
            padding: 0 20px;
            height: auto;
        }
        .testimonialCarousel-tutorList{
            .responsiveLandingPage(928px,20px);
            width: 100%;
            padding: 40px 0;
        }
    }
}
</style>

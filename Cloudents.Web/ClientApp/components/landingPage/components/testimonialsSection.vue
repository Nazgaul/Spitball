<template>
    <div class="testimonialsSection">
        <h1 class="tss-subtitle" v-language:inner="'homePage_tss_subtitle'"/>
        <h1 class="tss-title" v-language:inner="'homePage_tss_title'"/>
        <div class="tss-dvider"/>
        <div class="testimonialsCont" :style="{'pointer-events':$vuetify.breakpoint.smAndDown?'':'none'}">
            <sbCarousel :slideStep="1" :overflow="true" :arrows="false">
                <testimonialCard v-for="(item, index) in reviewsList" :item="item" :key="index"/>
            </sbCarousel>
        </div>
    </div>
</template>

<script>
import sbCarousel from '../../sbCarousel/sbCarousel.vue';
import { LanguageService } from "../../../services/language/languageService.js";
import testimonialCard from '../../carouselCards/testimonialCard.vue'
import { mapGetters, mapActions } from 'vuex';

export default {
    components:{sbCarousel,testimonialCard},
    computed: {
        ...mapGetters(['getHPReviews']),
        reviewsList(){
            return this.getHPReviews;
        },
        isMobile(){
            return this.$vuetify.breakpoint.xsOnly;
        }
    },
    methods: {
        ...mapActions(['updateHPReviews']),
    },
    created() {
        this.updateHPReviews()
    }
}
</script>

<style lang="less">
@import "../../../styles/mixin.less";
    .testimonialsSection{
        overflow: -webkit-paged-y;
        width: 100%;
        height: 620px;
        background-color: #f9f9fa;
        .tss-subtitle{
            padding-top: 34px;
            font-size: 16px;
            font-weight: normal;
            font-stretch: normal;
            font-style: normal;
            line-height: normal;
            letter-spacing: normal;
            text-align: center;
            color: #43425d;
        }
        .tss-title{
            @media (max-width: @screen-xs) {
                font-size: 24px;
            }
            font-size: 30px;
            font-weight: bold;
            font-stretch: normal;
            font-style: normal;
            line-height: normal;
            letter-spacing: normal;
            text-align: center;
            color: #43425d;
        }
        .tss-dvider{
            margin: 0 auto;
            margin-top: 16px;
            margin-bottom: 58px;
            width: 111px;
            height: 6px;
            background-color: #4c59ff;
        }
        .testimonialsCont{
            .responsiveLandingPage(928px,80px);
            display: flex;
            justify-content: center;
        }
    }
</style>
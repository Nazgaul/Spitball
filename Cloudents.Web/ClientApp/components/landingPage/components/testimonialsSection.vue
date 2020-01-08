<template>
    <div class="testimonialsSection">
        <h1 class="tss-subtitle" v-language:inner="'homePage_tss_subtitle'"/>
        <h1 class="tss-title" v-language:inner="'homePage_tss_title'"/>
        <div class="tss-dvider"/>
        <div class="testimonialsCont" ref="testimonialsCont" v-resize="setItemsToShow">
            <sbCarousel :itemsToShow="slideItems" :slideStep="1" :items="reviewsList" :overflow="true" :arrows="false">
                <template v-slot:slide="{item}">
                    <testimonialCard draggable="false" :item="item"/>
                </template>
            </sbCarousel>
        </div>
    </div>
</template>

<script>
import sbCarousel from '../../sbCarousel/sbCarousel.vue';
import testimonialCard from '../../carouselCards/testimonialCard.vue'
import { mapGetters, mapActions } from 'vuex';
import sbCarouselService from '../../sbCarousel/sbCarouselService';

export default {
    components:{sbCarousel,testimonialCard},
    data(){
        return {
            itemsToShow:3,
            maxItems:3,
            itemWidth: 285
        }
    },
    computed: {
        ...mapGetters(['getHPReviews']),
        reviewsList(){
            return this.getHPReviews;
        },
        slideItems(){
            return this.itemsToShow
        }
    },
    methods: {
        ...mapActions(['updateHPReviews']),
        setItemsToShow(){
            let carouselContainer = this.$refs.testimonialsCont;
            let offset = 10;
            this.itemsToShow = sbCarouselService.calculateItemsToShow(carouselContainer, this.itemWidth, offset, this.maxItems);
        }
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
        height: 580px;
        background-color: #f9f9fa;
        .tss-subtitle{
            padding-top: 20px;
            font-size: 16px;
            font-weight: normal;
            font-stretch: normal;
            font-style: normal;
            line-height: normal;
            letter-spacing: normal;
            text-align: center;
            color: #43425d;
            padding-bottom: 5px;
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
            margin-top: 10px;
            margin-bottom: 58px;
            width: 111px;
            height: 6px;
            background-color: #4c59ff;
            @media (max-width: @screen-xs) {
            margin-bottom: 40px;
            }
        }
        .testimonialsCont{
            .responsiveLandingPage(928px,80px);
            display: flex;
            justify-content: center;
        }
    }
</style>
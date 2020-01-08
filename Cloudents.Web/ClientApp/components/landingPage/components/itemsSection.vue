<template>
    <div class="itemsSection">
        <h1 class="is-title" v-language:inner="'homePage_is_title'"/>
        <div class="itemsCarousel" ref="itemsCarousel" v-resize="setItemsToShow">
            <sbCarousel :itemsToShow="slideItems" :itemsToSlide="slideItems" :items="itemList" v-if="itemList.length" :arrows="!$vuetify.breakpoint.xsOnly">
                <template v-slot:slide="{item, isDragging}">
                    <itemCard draggable="false" :fromCarousel="true" :isDragging="isDragging" :item="item" />
                </template>
            </sbCarousel>
        </div>
    </div>
</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import sbCarousel from '../../sbCarousel/sbCarousel.vue';
import itemCard from '../../carouselCards/itemCard.vue';

import carouselService from '../../sbCarousel/sbCarouselService';

export default {
    components:{sbCarousel,itemCard},
    data(){
        return{
            maxItemToShow: 5,
            itemsToShow: 5,
            cardWidth: 242
        }
    },
    computed: {
        ...mapGetters(['getHPItems']),
        itemList(){
            return this.getHPItems
        },
        slideItems(){
            return this.itemsToShow; 
        },
        
    },
    methods: {
        ...mapActions(['updateHPItems']),        
        setItemsToShow(){
            let carouselContainer = this.$refs.itemsCarousel;
            let offset = 10;
            this.itemsToShow = carouselService.calculateItemsToShow(carouselContainer, this.cardWidth, offset, this.maxItemToShow)
        }
    },
    created() {
        this.updateHPItems()
    },
}
</script>
<style lang="less">
@import "../../../styles/mixin.less";
.itemsSection{
    .responsiveLandingPage(1354px,80px);
    @media (max-width: @screen-xs) {
        width: calc(~"100% - 22px");
      margin-bottom: 40px;
        
    }
    margin-bottom: 80px;
    .is-title{
        @media (max-width: @screen-xs) {
            font-size: 18px;
            margin-bottom: 14px;
        }
        font-size: 24px;
        font-weight: bold;
        font-stretch: normal;
        font-style: normal;
        line-height: normal;
        letter-spacing: normal;
        margin-bottom: 26px;
        color: #43425d;
    }
    .itemsCarousel{
        width: 100%;
        .sbCarousel_btn {
            i {
                font-size: 18px;
            }
        }
    }

}

</style>
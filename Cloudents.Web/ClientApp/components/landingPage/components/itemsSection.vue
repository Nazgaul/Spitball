<template>
    <div class="itemsSection"  v-if="itemList.length">
        <h1 class="is-title" v-language:inner="'homePage_is_title'"/>
        <div class="itemsCarousel">
            <sbCarousel @select="enterItemCard" :arrows="!$vuetify.breakpoint.xsOnly">
                <itemCard :fromCarousel="true" v-for="(item, index) in itemList" :item="item" :key="index"/>
            </sbCarousel>
        </div>
    </div>
</template>

<script>
import { mapActions, mapGetters } from 'vuex';
const sbCarousel = () => import(/* webpackChunkName: "sbCarousel" */'../../sbCarousel/sbCarousel.vue');
//const itemCard = () => import(/* webpackChunkName: "itemCard" */ '../../carouselCards/itemCard.vue');
import itemCard from '../../carouselCards/itemCard.vue';

export default {
    components:{sbCarousel,itemCard},
    computed: {
        ...mapGetters(['getHPItems']),
        itemList(){
            return this.getHPItems
        }
    },
    methods: {
        ...mapActions(['updateHPItems']),
        enterItemCard(vueElm){
            if(vueElm.enterItemPage){
                vueElm.enterItemPage();
            }else{
                vueElm.$parent.enterItemPage();
            }
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
                font-size: 18px !important;
            }
        }
    }

}

</style>
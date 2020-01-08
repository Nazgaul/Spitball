<template>
  <div class="tutorsSections">
    <h1 class="ts-title" v-language:inner="'homePage_ts_title'" />
    <h2 class="ts-subtitle" v-language:inner="'homePage_ts_subtitle'" />
    <div class="tutorsCarousel" ref="tutorsCarousel" v-resize="setItemsToShow">
      <sbCarousel :itemsToShow="slideItems" :items="tutorList" :itemsToSlide="slideItems" v-if="tutorList.length" :arrows="!$vuetify.breakpoint.xsOnly">
        <template v-slot:slide="{item, isDragging}">
          <tutorCard draggable="false" :isDragging="isDragging" :fromCarousel="true" :tutor="item" />
        </template>
      </sbCarousel>
    </div>
  </div>
</template>

<script>
import sbCarousel from "../../sbCarousel/sbCarousel.vue";
import tutorCard from "../../carouselCards/tutorCard.vue"
import { mapActions, mapGetters } from "vuex";
import sbCarouselService from '../../sbCarousel/sbCarouselService';

export default {
  components: {sbCarousel,tutorCard},
  data(){
    return {
      tutorCardWidth: 242,
      itemsToShow: 5,
      maxItemsToShow: 5,
    }
  },
  computed: {
    ...mapGetters(["getHPTutors"]),
    tutorList() {
      return this.getHPTutors;
    },
    slideItems(){
      return this.itemsToShow;
    }
  },
  methods: {
    ...mapActions(["updateHPTutors"]),
    enterTutorCard(isDragging){
      if(!isDragging){
        this.$el.enterProfilePage();
      }
    },
    setItemsToShow(){
      let containerElm = this.$refs.tutorsCarousel;
      let offset = 10;
      this.itemsToShow = sbCarouselService.calculateItemsToShow(containerElm, this.tutorCardWidth, offset, this.maxItemsToShow)
    }
  },
  created() {
    this.updateHPTutors();
  }
};
</script>

<style lang="less">
@import "../../../styles/mixin.less";

.tutorsSections {
  .responsiveLandingPage(1354px, 80px);
  @media (max-width: @screen-xs) {
    width: calc(~"100% - 22px");
    margin-bottom: 50px;
  }
  margin-bottom: 70px;
  .ts-title {
    @media (max-width: @screen-xs) {
      font-size: 18px;
    }
    font-size: 24px;
    font-weight: bold;
    font-stretch: normal;
    font-style: normal;
    line-height: normal;
    letter-spacing: normal;
    color: #43425d;
  }
  .ts-subtitle {
    @media (max-width: @screen-xs) {
      margin: 8px 0 14px 0;
      font-size: 14px;
    }
    margin: 10px 0 15px 0;
    font-size: 16px;
    font-weight: normal;
    font-stretch: normal;
    font-style: normal;
    line-height: normal;
    letter-spacing: normal;
    color: #43425d;
  }
  .tutorsCarousel {
    width: 100%;

    .sbCarousel_btn {
      i {
        // transform: scaleX(1)/*rtl:scaleX(-1)*/; 
        color: rgb(68, 82, 252) !important;
        font-size: 18px;
      }
    }
  }
}
</style>
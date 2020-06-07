<template>
  <div class="tutorsSections">
    <h1 class="ts-title" v-language:inner="'homePage_ts_title'" />
    <h2 class="ts-subtitle" v-language:inner="'homePage_ts_subtitle'" />
    <div class="tutorsCarousel">
      <sbCarousel :slideStep="5" @select="enterTutorCard" v-if="tutorList.length" :arrows="!$vuetify.breakpoint.xsOnly">
        <tutorCard :fromCarousel="true" v-for="(tutor, index) in tutorList" :tutor="tutor" :key="index"/>
      </sbCarousel>
    </div>
  </div>
</template>

<script>
const sbCarousel = () => import(/* webpackChunkName: "sbCarousel" */"../../sbCarousel/sbCarousel.vue");
import tutorCard from "../../carouselCards/tutorCard.vue"
import { mapActions, mapGetters } from "vuex";
export default {
  components: {sbCarousel,tutorCard},
  computed: {
    ...mapGetters(["getHPTutors"]),
    tutorList() {
      return this.getHPTutors;
    }
  },
  methods: {
    ...mapActions(["updateHPTutors"]),
    enterTutorCard(vueElm){
      if(vueElm.enterProfilePage){
        vueElm.enterProfilePage();
      }else{
        vueElm.$parent.enterProfilePage();
      }
      
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
        font-size: 18px !important;
      }
    }
  }
}
</style>
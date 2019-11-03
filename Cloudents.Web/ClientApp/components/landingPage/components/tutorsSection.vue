<template>
  <div class="tutorsSections">
    <h1 class="ts-title" v-language:inner="'homePage_ts_title'" />
    <h2 class="ts-subtitle" v-language:inner="'homePage_ts_subtitle'" />
    <div class="tutorsCarousel">
      <sbCarousel @select="enterTutorCard" v-if="tutorList.length" :arrows="!$vuetify.breakpoint.xsOnly">
        <tutorCard :fromCarousel="true" v-for="(tutor, index) in tutorList" :tutor="tutor" :key="index"/>
      </sbCarousel>
    </div>
  </div>
</template>

<script>
import sbCarousel from "../../sbCarousel/sbCarousel.vue";
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
      vueElm.enterProfilePage();
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
    width: calc(~"100% - 20px");
    margin-bottom: 50px;
  }
  margin-bottom: 80px;
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
      margin: 6px 0;
      font-size: 14px;
    }
    margin: 10px 0;
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
  }
}
</style>
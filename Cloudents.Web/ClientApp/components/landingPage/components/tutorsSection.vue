<template>
  <div class="tutorsSections">
    <h1 class="ts-title" v-t="'homePage_ts_title'"></h1>
    <h2 class="ts-subtitle" v-t="'homePage_ts_subtitle'"></h2>
    <v-slide-group
      class="tutorSlider"
      :prev-icon="$vuetify.icons.prev"
      :next-icon="$vuetify.icons.next"
      :show-arrows="!$vuetify.breakpoint.xsOnly"
    >
      <v-slide-item v-for="(tutor, index) in tutorList" :key="index">
          <tutorCard :tutor="tutor" :key="index" />
      </v-slide-item>
    </v-slide-group>
  </div>
</template>

<script>
import tutorCard from "../../carouselCards/tutorCard.vue"

export default {
  components: { tutorCard },
  data() {
    return {
      isRtl: global.isRtl
    }
  },
  computed: {
    tutorList() {
      return this.$store.getters.getHPTutors;
    }
  },
  created() {
    this.$store.dispatch('updateHPTutors');
  }
}
</script>

<style lang="less">
@import "../../../styles/mixin.less";
@import "../../../styles/colors.less";

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
    color: @global-purple;
  }
  .ts-subtitle {
    @media (max-width: @screen-xs) {
      margin: 8px 0 14px 0;
      font-size: 14px;
    }
    margin: 10px 0 15px 0;
    font-size: 16px;
    font-weight: normal;
    color: @global-purple;
  }
}
.tutorSlider {
  .v-slide-group__next {
    right: -20px;
  }
  .v-slide-group__prev {
    left: -10px;
  }
  .v-slide-group__prev, .v-slide-group__next {
    height: 50px !important;
    width: 50px !important;
    background: #fff;
    position: absolute;
    top: calc(50% - 20px);
    z-index: 11;
    border-radius: 50%;
    box-shadow: 0px 3px 5px -1px rgba(0, 0, 0, 0.2), 0px 6px 10px 0px rgba(0, 0, 0, 0.14), 0px 1px 18px 0px rgba(0, 0, 0, 0.12);
  
    i {
      transform: scaleX(1)/*rtl:scaleX(-1)*/; 
      font-size: 18px !important;
      color: #4c59ff !important;
    }
  }
}
</style>
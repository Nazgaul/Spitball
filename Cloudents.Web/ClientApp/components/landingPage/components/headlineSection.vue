<template>
  <div class="headlineSection" :style="{'backgroundImage': `${isMobile ? '' : `url(${backgroundImg}`}`}">
    <components :is="`headlineSearch${isMobile?'Mobile':'Desktop'}`"/>
    <headlineStatus></headlineStatus>
  </div>
</template>

<script>
import headlineSearchDesktop from './headlineSection/headlineSearchDesktop.vue';
import headlineSearchMobile from './headlineSection/headlineSearchMobile.vue';
import headlineStatus from "./headlineStatus.vue";
import { LanguageService } from "../../../services/language/languageService.js";

export default {
  components: { headlineStatus, headlineSearchDesktop, headlineSearchMobile },
  data() {
    return {
      phSearch: LanguageService.getValueByKey("homePage_hd_search_ph")
    };
  },
  computed: {
    isMobile() {
      return this.$vuetify.breakpoint.xsOnly;
    },
    backgroundImg(){
      return global.isRtl? require('../images/bg_he.png') : require('../images/bg_en.jpg')
    },
    
  }
};
</script>

<style lang="less">
@import "../../../styles/mixin.less";

.headlineSection {
  width: 100%;
  height: 562px;
  @media (max-width: @sbScreen-smallDesktop) {
    height: 550px;
  }
  @media (max-width: @screen-xs) {
    height: unset;
  }
  background-repeat: no-repeat;
  background-size: cover;
  background-position: center;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  position: relative;
}
</style>
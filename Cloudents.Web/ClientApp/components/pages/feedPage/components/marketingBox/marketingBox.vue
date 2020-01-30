<template>
   <div class="marketing-box-component">
      <div class="heading" v-if="$vuetify.breakpoint.smAndDown">
         <span class="heading-text" v-language:inner>marketingBox_title</span>
      </div>
      <router-link :to="{name: 'tutorLandingPage'}" class="main-marketing-content transparent" @click.native="promotionOpen">
         <img :src="imgBySiteType" alt="Private lessons">
      </router-link>
   </div>
</template>

<script>
import { mapGetters } from 'vuex';
import analyticsService from '../../../../../services/analytics.service';

export default {
   name: "marketingBox",
   computed: {
      ...mapGetters(['accountUser']),
      isLogedIn() {
            return (this.accountUser != null)
      },
      imgBySiteType(){
            if(global.lang.toLowerCase() === 'he'){
               return require('./images/Banner_Sept_he.jpg');
            }else{
               return global.siteName === 'frymo' ? require('./images/Frymo_Promotion.jpg') : require('./images/Banner_Sept_en.jpg');
            }
      }
   },
   methods: {
      promotionOpen() {
            if (this.isLogedIn) {
               analyticsService.sb_unitedEvent('MARKETING_BOX', 'REGISTERED OPEN_TUTOR');
            } else {
               analyticsService.sb_unitedEvent('MARKETING_BOX', 'NOT REGISTERED OPEN_TUTOR');
            }
      },
   },
}
</script>

<style lang="less">
@import "../../../../../styles/mixin.less";

.marketing-box-component {
  display: flex;
  flex-direction: column;
  justify-content: center;
  height: 100%;
  width: 100%;
  max-width: 304px;
  // margin: 0 auto;
  // margin-bottom: 32px;
  @media (max-width: @screen-xs) {
    max-width: unset;
    height: ~"calc(100vh - 48px)";
  }
}

.heading {
  height: 56px;
  display: flex;
  flex-direction: row;
  align-items: center;
  .heading-text {
    font-size: 18px;
    font-weight: bold;
    letter-spacing: -0.2px;
    color: @textColor;
    padding: 21px 16px 16px 16px;
  }
}

.main-marketing-content {
  height: 100%;
  min-height: 164px;
  padding: 0;
  display: flex;
  flex-direction: row;
  align-items: center;
  justify-content: center;
  box-shadow: none;
  background-size: cover;
  background-position: center;
  background-repeat: no-repeat;
  cursor: pointer;
  @media (max-width: @screen-xs) {
    height:  ~"calc(100% - 56px)";
    min-height: unset;
    box-shadow: 0 1px 7px 0 rgba(0, 0, 0, 0.22);

  }
}
</style>
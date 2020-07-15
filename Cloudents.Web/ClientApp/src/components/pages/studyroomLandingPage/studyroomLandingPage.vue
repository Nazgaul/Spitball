<template>
   <div class="studyroomLandingPage d-flex" :class="[{'window2':onboarding === 1}]">
      <div class="pageWrapper px-0 px-sm-5 px-md-5 px-lg-0">
         <v-window v-model="onboarding">
            <v-window-item>
               <logo v-if="!isMobile" class="pageLogo"/>
               <roomInfo @enrolled="onboarding = 1"/>
               <sessionInfo/>
               <hostInfo/>
            </v-window-item>
            <v-window-item>
               
               <roomThankYou/>
            </v-window-item>
         </v-window>
      </div>
   </div>
</template>

<script>
import roomInfo from './roomInfo.vue';
import sessionInfo from './sessionInfo.vue';
import hostInfo from './hostInfo.vue';
import logo from '../../app/logo/logo.vue';
import roomThankYou from './roomThankYou.vue'
export default {
   data() {
      return {
         onboarding: 0
      }
   },
   components:{
      roomInfo,
      sessionInfo,
      hostInfo,
      logo,
      roomThankYou
   },
   computed: {
      isMobile(){
         return this.$vuetify.breakpoint.xsOnly;
      }
   },
}
</script>

<style lang="less">
   @import '../../../styles/mixin.less';
   .studyroomLandingPage{
      width: 100%;
      height: 100%;
      background: white;  
      position: relative;
      &.window2{
         &::before{
            content: '';
            height: auto;
            max-height: initial;
         }
      }
      &::before{
         content: '';
         position: absolute;
         background-repeat: no-repeat;
         background-size: cover;
         background-position: center;
         background-image: url('./images/Landing-page_small.jpg');
         width: 100%;
         max-height: 714px;
         top: 0;
         left: 0;
         right: 0;
         bottom: 0;
         z-index: 1;
         @media(max-width: @screen-sm) {
            max-height: 653px;
         }
         @media(max-width: @screen-xs) {
            background-image: none;
         }
      }
      .pageWrapper{
         width: 100%;
         z-index: 2;
         margin: 0 auto;
         max-width: 1100px;
         .pageLogo{
            margin: 16px 0;
            .logo {
               fill: #fff !important;
               width: 120px;
            }
         }
      }

   }
</style>
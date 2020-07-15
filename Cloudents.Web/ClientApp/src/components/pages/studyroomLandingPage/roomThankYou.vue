<template>
   <div class="roomThankYou d-flex">
      <logo v-if="!isMobile" class="logoThankYou"/>
      <div class="thankYouWrapper px-4">
         <logo v-if="isMobile" :menuList="true" class="logoThankYouMobile"/>
         <img class="mt-5 mt-sm-8" src="./images/circleCheck.png" width="50px" height="50px" alt="">
         <div class="thankTitle pt-2 pt-sm-0" v-t="'seat_saved'"/>
         <div class="thankSubTitle" v-t="'we_will_email'"/>
         <div class="thankBox">
            <img width="100%" src="./images/live-banner-copy.jpg">
            <div class="pt-3">{{$t('starts_on',[date])}}</div>
            <sessionStartCounter class="thankYouCounter" :dateProp="'2020-07-30T21:00:00Z'"/>
         </div>

      </div>
   </div>
</template>

<script>
import logo from '../../app/logo/logo.vue';
import sessionStartCounter from '../../studyroom/tutorHelpers/sessionStartCounter/sessionStartCounter.vue'

export default {
   components:{logo,sessionStartCounter},
   data() {
      return {
         date: `July 19th, 7:00 PM`
      }
   },
   computed: {
      isMobile(){
         return this.$vuetify.breakpoint.xsOnly;
      },
   },

}
</script>

<style lang="less">
   @import '../../../styles/mixin.less';

   .roomThankYou{
      @media(max-width: @screen-xs) {
         margin-top: 0;
         height: 100%;
         margin-bottom: 0;
         padding-bottom: 0;
      }
      width: 100%;
      margin-bottom: 38px;
      padding-bottom: 64px;
      background: white;
      margin-top: 38px;
      position: relative;
      color: #ffffff;
      &::before{
         content: '';
         position: absolute;
         background: #1b2441;
         width: 100%;
         top: 0;
         left: 0;
         right: 0;
         height: 50%;
         bottom: 0;
         @media(max-width: @screen-xs) {
            height: 524px;
         }

      }
      .logoThankYou{
         position: absolute;
         top: 14px;
         left: 14px;
         opacity: 0.4;
            .logo {
               fill: #fff !important;
               width: 120px;
            }
      }
      .thankYouWrapper{
         z-index: 1;
         width: 100%;
         text-align: center;
         .thankTitle{
            font-size: 50px;
            font-weight: 600;
            padding-bottom: 8px;
            @media(max-width: @screen-xs) {
               line-height: 1.2; 
               padding: 0 16px 20px;
            }

         }
         .thankSubTitle{
            margin: 0 auto;
            font-size: 18px;
            line-height: 1.5;
            max-width: 550px;
            padding-bottom: 24px;
            @media(max-width: @screen-xs) {
               font-size: 20px;
               line-height: 1.5;
               padding-bottom: 30px;
            }
         }
         .logoThankYouMobile{
            margin: 0;
            margin-top: 10px;
            opacity: 0.4;
            text-align: initial;
            .logo {
               margin: 0;
               fill: #fff !important;
               width: 100px;
               height: auto;
            }
         }
         .thankBox{
            margin: 0 auto;
            max-width: 428px;
            // height: 427px;
            border-radius: 6px;
            border: solid 1px #dddddd;
            background-color: #ffffff;
            padding: 12px;
            font-size: 20px;
            font-weight: 600;
            color: #43425d;
            @media(max-width: @screen-xs) {
               max-width: initial;
               width: 100%;
               padding: 0;
               border: none;
               border-radius: initial;
               height: auto;
               padding-bottom: 50px;
            }

            .thankYouCounter{
               .counterDots{
                  padding: 0 10px;

               }
               margin-top: 10px;
               text-align: center;
               font-size: 40px;
               font-weight: 600;
               color: #1b2441;
            }
         }
      }
   }
</style>
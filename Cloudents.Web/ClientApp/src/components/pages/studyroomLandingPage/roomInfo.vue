<template>
   <div class="roomInfoContainer d-flex flex-column">
      <logo v-if="isMobile" :menuList="true" class="logoRoom"></logo>
      <div class="roomInfoTop d-flex">
         <div class="rightSide px-2 pt-10 pb-5 pb-sm-0 pt-sm-0">
            <div class="classTitle">{{$t('live_with',[tutorName])}}</div>
            <div class="classSubject" v-text="classSubject"/>
            <div>
               <div class="pb-2">10 live sessions - every Tue and Thu</div>
               <div>{{$t('starts_on',[date])}}</div>
            </div>
            <div v-if="!isMobile">Only $39.99 for the entire class</div>
            <img class="triangle" src="./images/triangle.png">
         </div>
         <div v-if="!isMobile" class="leftSide">
            <img src="../../new_profile/components/profileLiveClasses/live-banner-desktop.png">
         </div>
      </div>
      <div class="roomInfoBottom d-flex flex-wrap justify-center">
         <div class="bottomRight text-center px-4">
            <div v-if="isMobile" class="pt-7 sessionPrice">Only $39.99 for the entire class</div>
            <v-btn class="saveBtn" v-t="'save_spot'" depressed :height="btnHeight" color="#1b2441"/>
            <div class="couponText" v-t="'apply_coupon_code'"/>
         </div>
         <div class="bottomLeft">
            <sessionStartCounter class="pageCounter" :dateProp="'2020-07-30T21:00:00Z'"/>
            <!-- @updateCounterFinish="waitingForTutor = true"  -->
         </div>
      </div>
      <div v-if="isMobile" class="mobileImg">
         <img src="../../new_profile/components/profileLiveClasses/live-banner-desktop.png">
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
         tutorName:'Ariel Feinsilberg',
         classSubject: 'Just Say Hello-Find an Online Teaching Job',
         date: `July 19th, 7:00 PM`
      }
   },
   computed: {
      isMobile(){
         return this.$vuetify.breakpoint.xsOnly;
      },
      btnHeight(){
         if(this.isMobile){
            return 70;
         }
         return this.$vuetify.breakpoint.smAndDown? 74 : 82;
      }
   },
}
</script>

<style lang="less">
   @import '../../../styles/mixin.less';
   .roomInfoContainer{
      width: 100%;
      @media(max-width: @screen-sm) {
         // background-color: #1b2441;
         position: relative;
      }
         .logoRoom{
            position: absolute;
            // top: 0;
            z-index: 5;
            margin-top: 16px;
            .logo {
               margin: 0;
               opacity: 0.4;
               fill: #fff !important;
               width: 120px;
            }
         }
      .roomInfoTop{
         width: 100%;
         height: 357px;
         @media(max-width: @screen-sm) {
            height: 300px;
         }
         @media(max-width: @screen-xs) {
            height: 290px;
         }
         .rightSide{
            color: white;
            width: 52%;
            background-color: #1b2441;
            text-align: center;
            position: relative;
            display: flex;
            flex-direction: column;
            justify-content: space-evenly;
            font-size: 20px;
            @media(max-width: @screen-sm) {
               font-size: 18px;
            }
            @media(max-width: @screen-xs) {
               width: 100%;
            }
            .classTitle{
               font-size: 22px;
               @media(max-width: @screen-sm) {
                  font-size: 18px;
               }
               @media(max-width: @screen-xs) {
                  font-size: 16px;
               }
               color: #41c4bc;
            }
            .classSubject{
               margin: 0 auto;
               font-size: 42px;
               @media(max-width: @screen-sm) {
                  max-width: 380px;
                  font-size: 30px;
               }
            }
            .triangle{
               position: absolute;
               bottom: -21px;
               left: calc(~"50% - 32px");
            }
         }
         .leftSide{
            width: 48%;
            img{
               width: 100%;
               height: 100%;
               object-fit: cover;
            }
         }
      }
      .mobileImg{
         width: 100%;
         img{
            width: 100%;
            height: 100%;
            object-fit: cover;
         }
      }
      .roomInfoBottom{
         width: 100%;
         padding-bottom: 16px;
         @media(max-width: @screen-sm) {
            padding-bottom: 12px;
         }
         background-color: #41c4bc;
         .bottomRight{
            width: 52%;
            @media(max-width: @screen-xs) {
               width: 100%;
            }
            .saveBtn{
               width: 100%;
               border-radius: 8px;
               max-width: 380px;
               margin-top: 34px;
               font-size: 22px;
               font-weight: 600;
               color: white;
               @media(max-width: @screen-xs) {
                  margin-top: 10px;
                  font-size: 24px;
               }
            }
            .couponText{
               font-size: 20px;
               color: #1b2441;
               text-decoration: underline;
               padding-top: 10px;
               @media(max-width: @screen-sm) {
                  font-size: 18px;
               }
               @media(max-width: @screen-xs) {
                  font-size: 16px;
               }
            }
            .sessionPrice{
               font-size: 18px;
               font-weight: 600;
               color: #1b2441;
            }
         }
         .bottomLeft{
            width: 48%;
            @media(max-width: @screen-xs) {
               width: 100%;
            }
            .pageCounter{
               span{
                  padding-right: 4px;
                  padding-left: 4px;
               }
               @media(max-width: @screen-xs) {
                  margin-top: 14px;
                  font-size: 30px;
               }

               margin-top: 34px;
               text-align: center;
               font-size: 52px;
               font-weight: 600;
               color: #1b2441;
            }
         }
      }
   }
</style>
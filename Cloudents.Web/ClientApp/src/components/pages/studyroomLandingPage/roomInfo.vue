<template>
   <div class="roomInfoContainer d-flex flex-column">
      <div style="width: fit-content" class="cursor-pointer" v-if="isMobile" @click="$router.push('/')">
         <logo :menuList="true" class="logoRoom"></logo>
      </div>
      <div class="roomInfoTop d-flex">
         <div class="rightSide px-2 pt-10 pb-5 pb-sm-0 pt-sm-0">
            <div class="classTitle">{{$t('live_with',[tutorName])}}</div>
            <div class="classSubject" v-text="courseName"/>
            <div v-if="courseSessions.length">
               <div class="pb-2">{{$tc('live_times',courseSessions.length)}}</div>

               <div>{{$t('starts_on',[$moment(courseDate).format('MMMM Do, h:mm a')])}}</div>
            </div>
            <div v-else class="pb-2">
               {{$moment(courseDate).format('MMMM Do, h:mm a')}}
            </div>
            <div v-if="!isMobile && coursePrice && coursePrice.amount">
               {{$t("room_price",[$price(coursePrice.amount, coursePrice.currency, true)])}}
            </div>
            <img class="triangle" src="./images/triangle.png">
         </div>
         <div v-if="!isMobile" class="leftSide">
            <v-skeleton-loader v-if="!imgLoaded" width="100%" height="100%" type="image" class="skelLoader">
            </v-skeleton-loader>
            <img v-show="imgLoaded" @load="()=>imgLoaded = true" :src="courseImage">
         </div>
      </div>
      <div class="roomInfoBottom d-flex flex-wrap justify-center">
         <div class="bottomRight text-center px-6 px-sm-4">

            <div v-if="isMobile && coursePrice && coursePrice.amount" class="pt-7 sessionPrice">
               {{$t("room_price",[$price(coursePrice.amount, coursePrice.currency, true)])}}
             </div>
            <v-btn v-if="isCourseTutor" @click="enterStudyRoom" :class="{'mt-7': isMobile && coursePrice && !coursePrice.amount}" class="saveBtn" depressed :height="btnHeight" color="#1b2441">
               {{$t('enter_room')}}
            </v-btn>
            <v-btn v-else :disabled="isRoomFull" :loading="loadingBtn" :class="{'mt-7': isMobile && coursePrice && !coursePrice.amount}" @click="enrollSession" class="saveBtn" depressed :height="btnHeight" color="#1b2441">
               {{isRoomFull? $t('room_full') : $t('save_spot') }}
            </v-btn>
            <v-btn v-if="coursePrice && coursePrice.amount" block :disabled="isCourseTutor || isRoomFull" @click="applyCoupon" class="couponText" tile text>{{$t('apply_coupon_code')}}</v-btn>
         </div>
         <div class="bottomLeft">
            <sessionStartCounter v-show="!isSessionNow" class="pageCounter" @updateCounterFinish="isSessionNow = true"/>
         </div>
      </div>
      <div v-if="isMobile" class="mobileImg">
         <v-skeleton-loader v-if="!imgLoaded" height="100%" width="100%"  type="image" class="skelLoader">
         </v-skeleton-loader>
         <img v-show="imgLoaded" @load="()=>imgLoaded = true" :src="courseImage">
      </div>
        <stripe ref="stripe"></stripe>
   </div>
</template>

<script>
import stripe from "../global/stripe.vue";

import logo from '../../app/logo/logo.vue';
import sessionStartCounter from '../../studyroom/tutorHelpers/sessionStartCounter/sessionStartCounter.vue'
import * as componentConsts from '../global/toasterInjection/componentConsts.js';
import * as routeNames from '../../../routes/routeNames';
import EventBus from '../../../eventBus.js';

export default {
   components:{logo,sessionStartCounter,stripe},
   data() {
      return {
         isSessionNow:false,
         loadingBtn:false,
         imgLoaded:false,
      }
   },
   methods: {
      applyCoupon(){
         if(!this.isLogged) {
            this.$store.commit('setComponent', 'register')
            return
         }
         if(this.loadingBtn) return;
         this.$store.commit('setComponent', 'applyCoupon');
      },
      enterStudyRoom(){
         let id = this.$route.params?.id;
         let routeData = this.$router.resolve({
            name: routeNames.StudyRoom,
            params: { id }
         });
         global.open(routeData.href, "_self");
      },
      async goStripe() {
         let session = {
          //  userId: this.$store.getters.accountUser?.id,
            studyRoomId: this.$route.params?.id
         };
         let x = await this.$store.dispatch('updateStudyroomLiveSessionsWithPrice', session);
         this.$refs.stripe.redirectToStripe(x);
      },
      async goPayme() {
         let session = {
          //  userId: this.$store.getters.accountUser?.id,
            studyRoomId: this.$route.params?.id
         };
         let x = await this.$store.dispatch('updateStudyroomLiveSessionsWithPricePayMe',session);
         location.href = x;
      },

      async enrollSession(){
         if(!this.isLogged) {
            this.$store.commit('setComponent', 'register')
            return
         }
         if(this.loadingBtn) return;
         this.loadingBtn = true;

         let userId = this.$store.getters.accountUser?.id;
         let studyRoomId = this.$route.params?.id;
         let session = {
            userId,
            studyRoomId
         }
         if (this.coursePrice.amount) {
            if (this.tutorCountry !== 'IL'){
               this.goStripe()
               return;
            }
            this.goPayme();
            return;
         }
         let self = this
         this.$store.dispatch('updateStudyroomLiveSessions', session)
            .then(() => {
               self.$store.commit('setCourseEnrolled',true);
            }).catch(ex => {
               self.$store.commit('setComponent',componentConsts.ENROLLED_ERROR);
               self.$appInsights.trackException(ex);
            }).finally(()=>{
               self.loadingBtn = false;
            })
      }
   },
   computed: {
      courseDetails(){
         return this.$store.getters.getCourseDetails;
      },
      courseSessions(){
         return this.$store.getters.getCourseSessions
      },
      courseImage(){
         return this.$proccessImageUrl(this.courseDetails?.image, 528, 357)
      },
      courseName(){
         return this.courseDetails?.name;
      },
      isCourseTutor(){
         return this.$store.getters.getIsCourseTutor
      },
      coursePrice(){
         return this.$store.getters.getCoursePrice;
      },
      tutorName(){
         return this.courseDetails?.tutorName;
      },
      tutorCountry(){
         return this.courseDetails?.tutorCountry
      },
      isMobile(){
         return this.$vuetify.breakpoint.xsOnly;
      },
      btnHeight(){
         if(this.isMobile){
            return 70;
         }
         return this.$vuetify.breakpoint.smAndDown? 74 : 82;
      },
      isLogged() {
         return this.$store.getters.getUserLoggedInStatus
      },
      courseDate(){
         return this.courseDetails?.startTime;
      },
      isRoomFull(){
         return this.courseDetails?.full;
      },
      // recurringDetails(){
      //    return this.$store.getters.getSessionRecurring(this.courseDetails?.nextEvents)
      // },
   },
   mounted() {
      EventBus.$on('applyCouponDone',()=>{
         if (this.coursePrice?.amount && this.tutorCountry !== 'IL') {
            this.goStripe()
         }else{
            this.enrollSession()
         }
      });
   },
   beforeDestroy() {
      EventBus.$off('applyCouponDone', ()=>{})
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
               @media(max-width: @screen-sm) {
                  font-size: 28px;
               }
            }
            .triangle{
               position: absolute;
               bottom: -21px;
               left: calc(~"50% - 32px");
            }
         }
         .leftSide{
            .skelLoader{
               background: white;
               .v-skeleton-loader__image{
                  height: inherit;
               }
            }
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
               text-transform:initial;
               padding-top: 12px;
               @media(max-width: @screen-sm) {
                  font-size: 18px;
               }
               @media(max-width: @screen-xs) {
                  font-size: 16px;
                  font-weight: 600;
                  padding-top: 14px;

               }
            }
            .sessionPrice{
               font-size: 18px;
               font-weight: bold;
               color: #1b2441;
            }
         }
         .bottomLeft{
            width: 48%;
            @media(max-width: @screen-xs) {
               width: 100%;
            }
            .pageCounter{
               .counterDots{
                  padding: 0 10px;
                  @media(max-width: @screen-sm) {
                     padding: 0 4px;
                  }
               }
               @media(max-width: @screen-sm) {
                  font-size: 44px;
                  margin-top: 40px;
               }
               @media(max-width: @screen-xs) {
                  margin-top: 16px;
                  font-size: 36px;
               }
               margin-top: 36px;
               text-align: center;
               font-size: 54px;
               font-weight: 600;
               color: #1b2441;
            }
         }
      }
   }
</style>
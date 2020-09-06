<template>
   <div id="courseInfoSection" class="roomInfoContainer d-flex flex-column">
      <div style="width:-moz-fit-content;width: fit-content" class="cursor-pointer" v-if="isMobile" @click="$router.push('/')">
         <logo :menuList="true" class="logoRoom"></logo>
      </div>

      <div class="roomInfoTop d-flex">
         <div class="rightSide px-2 pt-10 pb-5 pb-sm-0 pt-sm-0">
            <div>
               <div class="classTitle">{{$t('live_with')}}</div>
               <div class="classSubject pt-4" v-text="courseName"/>
            </div>
            <div v-if="courseSessions.length">
               <div class="pb-2">{{$tc('live_times',courseSessions.length)}}</div>

               <div>{{$t('starts_on',[$moment(courseDate).format('MMMM Do, HH:mm')])}}</div>
            </div>
            <template v-if="!isMobile">
               <div v-if="coursePrice && coursePrice.amount">
                  {{$t("room_price",[$price(coursePrice.amount, coursePrice.currency, true)])}}
               </div>
               <div v-else>{{$t('course_free')}}</div>
            </template>
            <img class="triangle" src="./images/triangle.png">
         </div>
         <div v-if="!isMobile" class="leftSide">
            <v-skeleton-loader v-if="!imgLoaded" width="100%" height="100%" type="image" class="skelLoader">
            </v-skeleton-loader>
            <img v-show="imgLoaded" @load="()=>imgLoaded = true" :src="courseImage">
         </div>
      </div>
      <div v-if="!isCourseTutor || isCourseTutor " class="roomInfoBottom d-flex flex-wrap justify-center">
         <div class="bottomRight text-center px-6 px-sm-4">
            <template v-if="isMobile">
               <div v-if="coursePrice && coursePrice.amount" class="pt-7 sessionPrice">
                  {{$t("room_price",[$price(coursePrice.amount, coursePrice.currency, true)])}}
               </div>
               <div v-else class="pt-7 sessionPrice">{{$t('course_free')}}</div>
            </template>

            <v-btn class="saveBtn" :loading="loadingBtn" @click="enrollSession" :disabled="isCourseFull" depressed :height="btnHeight" color="#1b2441">
               {{enrollBtnText}}
            </v-btn>
            <v-btn v-if="coursePrice && coursePrice.amount" block :disabled="isCourseFull" @click="applyCoupon" class="couponText" tile text>{{$t('apply_coupon_code')}}</v-btn>
        
         </div>
         <div class="bottomLeft" v-if="courseDetails">
            <sessionStartCounter v-show="!isSessionNow" class="pageCounter" @updateCounterFinish="isSessionNow = true"/>
         </div>
      </div>

      <div v-if="isMobile" class="mobileImg">
         <v-skeleton-loader v-if="!imgLoaded" height="100%" width="100%"  type="image" class="skelLoader">
         </v-skeleton-loader>
         <img v-show="imgLoaded" @load="()=>imgLoaded = true" :src="courseImage">
      </div>

      <v-snackbar v-model="tutorSnackbar" top :timeout="3000">
         <div class="text-center">{{snackbarText}}</div>
      </v-snackbar>
   </div>
</template>

<script>

import logo from '../../app/logo/logo.vue';
import sessionStartCounter from '../../studyroom/tutorHelpers/sessionStartCounter/sessionStartCounter.vue'
import * as routeNames from '../../../routes/routeNames';
import EventBus from '../../../eventBus.js';

export default {
   components:{logo,sessionStartCounter},
   data() {
      return {
         isSessionNow:false,
         loadingBtn:false,
         imgLoaded:false,
         tutorSnackbar:false,
         snackbarText:''
      }
   },
   methods: {
      applyCoupon(){
         if(!this.isLogged) {
            this.$store.commit('setComponent', 'register')
            return
         }
         if(this.isCourseTutor){
            this.snackbarText = this.$t('coupon_tutor')
            this.tutorSnackbar = true
            return;
         }

         if(this.loadingBtn) return;
         this.$store.commit('setComponent', 'applyCoupon');
      },
      enterStudyRoom(){
         let id = this.courseNextSession?.id;
         let routeData = this.$router.resolve({
            name: routeNames.StudyRoom,
            params: { id }
         });
         global.open(routeData.href, "_self");
      },
      async enrollSession(){
         if(!this.isLogged) {
            this.$store.commit('setComponent', 'register')
            return
         }
         if(this.loadingBtn) return;
         if(this.isCourseTutor){
            if(this.courseNextSession?.id){
               this.enterStudyRoom()
            }else{
               this.snackbarText = this.$t('enroll_tutor')
               this.tutorSnackbar = true
               return;
            }
         }
         this.loadingBtn = true;
         let courseId = this.$route.params?.id;
         let self = this
         this.$store.dispatch('updateEnrollCourse', courseId)
            .finally(()=>{
               self.loadingBtn = false;
            })
      }
   },
   computed: {
      courseName(){
         return this.$store.getters.getCourseNamePreview;
      },
      courseNextSession(){
         return this.$store.getters.getNextCourseSession;
      },
      courseImage(){
         let img = this.$store.getters.getCourseImagePreview;
         if(img && img.includes('blob')){
            return img;
         }else{
            return this.$proccessImageUrl(img, {width:528, height:357})
         }
      },
      courseSessions(){
         return this.$store.getters.getCourseSessionsPreview
      },
      isCourseTutor(){
         return this.$store.getters.getIsCourseTutor
      },
      coursePrice(){
         return this.$store.getters.getCoursePrice;
      },
      courseDetails(){
         return this.$store.getters.getCourseDetails;
      },
      enrollBtnText(){
         if(!this.courseDetails){
            return ''
         }
         if(!this.courseNextSession?.id && this.$store.getters.getCourseItems?.length == 0){
            return this.$t('expired');
         }
         if(this.isCourseFull){
            return this.$t('room_full');
         }else{
            if(this.$store.getters.getCourseButtonPreview) return this.$store.getters.getCourseButtonPreview;
            else{
               return this.coursePrice?.amount? this.$t('save_spot') : this.$t('free_enroll')
            }
         }
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
      isCourseFull(){
         return this.$store.getters.getCourseIsFull
      },
   },
   mounted() {
      EventBus.$on('applyCouponDone',()=>{
         this.enrollSession()
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
               word-break: break-all;
               word-break: break-word;
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
               min-width: -moz-fit-content;
               min-width: fit-content !important;
               margin: 0 auto;
               font-size: 20px;
               color: #1b2441;
               text-transform:initial;
               margin-top: 4px;
               @media(max-width: @screen-sm) {
                  font-size: 18px;
               }
               @media(max-width: @screen-xs) {
                  font-size: 16px;
                  font-weight: 600;
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
    .courseDrawer ~ .v-main {
       min-width: 1264px;
       overflow-x: auto;
       background: #fff;
         ::-webkit-scrollbar-track {
            background: #f5f5f5; 
         }
         ::-webkit-scrollbar {
               width: 22px;
         }
         ::-webkit-scrollbar-thumb {
               background: #b5b8d9 !important;
               border-radius: 4px !important;
         }
         .studyroomLandingPage{
            min-width: 1264px + 338px;
         }
   }
</style>
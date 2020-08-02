<template>
   <div class="roomThankYou d-flex">
      <logo :menuList="true" class="logoThankYou cursor-pointer" v-if="!isMobile" @click.native="$router.push('/')"></logo>
      <div class="thankYouWrapper px-4">
         <div style="width: fit-content" class="cursor-pointer" v-if="isMobile" @click="$router.push('/')">
            <logo :menuList="true" class="logoThankYouMobile"></logo>
         </div>
         <img class="mt-5 mt-sm-16" src="./images/circleCheck.png" width="50px" height="50px" alt="">
         <div class="thankTitle pt-2 pt-sm-0" v-t="'seat_saved'"/>
         <div class="thankSubTitle" v-t="'we_will_email'"/>
         <div class="thankBox">
            <v-skeleton-loader v-if="!imgLoaded" width="100%" height="100%" type="image">
            </v-skeleton-loader>
            <img v-show="imgLoaded" @load="()=>imgLoaded = true" width="100%" :src="courseImage">
            <template v-if="isSessions">
               <div class="pt-3">{{$t('starts_on',[$moment(courseDate).format('MMMM Do, h:mm a')])}}</div>
               <sessionStartCounter v-show="!isTimmerFinished" class="thankYouCounter" @updateCounterMinsLeft="isRoomReady = true" @updateCounterFinish="isTimmerFinished = true"/>
               <v-btn :disabled="isButtonDisabled" @click="enterStudyRoom" class="saveBtn" depressed :height="btnHeight" color="#1b2441">
                  {{$t('enter_room')}}
               </v-btn>
            </template>
         </div>

      </div>
   </div>
</template>

<script>
import logo from '../../app/logo/logo.vue';
import sessionStartCounter from '../../studyroom/tutorHelpers/sessionStartCounter/sessionStartCounter.vue'
import * as routeNames from '../../../routes/routeNames';

export default {
   components:{logo,sessionStartCounter},
   data() {
      return {
         isTimmerFinished:false,
         isRoomReady: false,
         imgLoaded:false,
      }
   },
   computed: {
      courseDate(){
         return this.$store.getters.getCourseDetails?.startTime;
      },
      courseImage(){
         return this.$proccessImageUrl(this.$store.getters.getCourseDetails?.image, 402, 268)
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
      isButtonDisabled(){
         if(this.$store.getters.getJwtToken || this.$store.getters.getCourseDetails?.sessionStarted) return false;
         if(this.$store.getters.getCourseSessions?.length === 0) return true;
         else return !this.isRoomReady
      },
      isSessions(){
         return this.$store.getters.getCourseSessions?.length
      }
      
   },
   methods: {
      enterStudyRoom(){
         let id = this.$route.params?.id;
         let routeData = this.$router.resolve({
            name: routeNames.StudyRoom,
            params: { id }
         });
         global.open(routeData.href, "_self");
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
     // margin-top: 38px;
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
         height: 450px;
         // height: 75%;
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
         z-index: 2;
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
            margin: 10px 0 0;
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
                border: none;
               border-radius: initial;
               height: auto;
                padding: 0 0 50px;
            }
            .saveBtn{
               width: 100%;
               border-radius: 8px;
               max-width: 380px;
               margin-top: 14px;
               font-size: 22px;
               font-weight: 600;
               color: white;
               @media(max-width: @screen-xs) {
                  margin-top: 10px;
                  font-size: 24px;
               }
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
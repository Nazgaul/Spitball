<template>
   <div class="studyroomLandingPage d-flex" :class="[{'window2':isRoomEnrolled}]">
      <div class="pageWrapper px-0 px-sm-5 px-md-5 px-lg-0">
         <v-window :value="isRoomEnrolled? 1 : 0"> 
            <v-window-item>
               <div class="roominfoHeader">
                  <div class="cursor-pointer content" v-if="!isMobile" @click="$router.push('/')">
                     <logo class="pageLogo"/>
                  </div>
                  <roomInfo class="content"/>
               </div>
               <sessionInfo  class="content"/>
               <courseSessions class="content" />
               <courseItems  class="content"/>
               <hostInfo  class="content"/>
            </v-window-item>
            <v-window-item>
               <roomThankYou />
               <courseSessions class="content" />
               <courseItems  class="content"/>
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
const roomThankYou = () => import('./roomThankYou.vue');
import courseItems from './courseItems.vue';
import courseSessions from './courseSessions.vue';

export default {
   components:{
      roomInfo,
      sessionInfo,
      hostInfo,
      logo,
      roomThankYou,
      courseItems,
      courseSessions
   },
   computed: {
      isMobile(){
         return this.$vuetify.breakpoint.xsOnly;
      },
      isRoomEnrolled(){
         let isRoomTutor = this.$store.getters.accountUser?.id == this.$store.getters.getCourseDetails?.tutorId;
         if(isRoomTutor) return false;
         else{
            return this.$store.getters.getCourseDetails?.enrolled;
         }
      },
   },
   beforeRouteLeave (to, from, next) {
      this.$store.dispatch('updateCourseDetails',null)
      next()
   }
}
</script>

<style lang="less">
   @import '../../../styles/mixin.less';
    .studyroomLandingPage{
   //    width: 100%;
   //    height: 100%;
       background: white;  
   //    position: relative;
   //    &.window2{
   //       &::before{
   //          content: '';
   //          height: auto;
   //          max-height: initial;
   //       }
   //    }
   //    &::before{
   //       content: '';
   //       position: absolute;
   //       background-repeat: no-repeat;
   //       background-size: cover;
   //       background-position: center;
   //       background-image: url('./images/Landing-page_small.jpg');
   //       width: 100%;
   //       max-height: 647px;
   //       top: 0;
   //       left: 0;
   //       right: 0;
   //       bottom: 0;
   //       z-index: 1;
   //       @media(max-width: @screen-sm) {
   //          max-height: 653px;
   //       }
   //       @media(max-width: @screen-xs) {
   //          background-image: none;
   //       }
   //    }
      .roominfoHeader {
        // background: red;
          background-image: url('./images/Landing-page_small.jpg');
          padding:16px;
          padding-bottom: 60px;
      }
       .pageWrapper{
          width: 100%;
   //       z-index: 2;
          
   //       max-width: 1100px;
         .content {
            max-width: 1100px;
            margin: 0 auto;
         }
          .pageLogo{
             margin-bottom: 10px;
            // margin: 16px 0;
             .logo {
                fill: #fff !important;
                width: 120px;
             }
          }
       }

    }
</style>
<template>
   <v-app>
      <router-view name="banner"></router-view>
      <router-view v-if="showHeader" name="header"></router-view>

      <router-view name="sideMenu" v-if="isDrawer"></router-view>

      <v-content :class="[{'site-content': $route.path !== '/'},{'hidden-sideMenu': drawerPlaceholder}]">
         <router-view class="main-container"></router-view>
      </v-content>
      <mobile-footer v-if="showMobileFooter" />
      <router-view name="footer"></router-view>

      <dialogInjection class="dialogInjection" />
      <componentInjection class="componentInjection" />


      <!-- HAVE TO CLEAN: -->
        <sb-dialog
          :isPersistent="true"
          :showDialog="newQuestionDialogSate"
          :popUpType="'newQuestion'"
          :max-width="'510px'"
          :content-class="'question-request-dialog'"
        >
          <AddQuestion v-if="newQuestionDialogSate"></AddQuestion>
        </sb-dialog>

        <sb-dialog
          :isPersistent="true"
          :showDialog="getRequestTutorDialog"
          :popUpType="'tutorRequestDialog'"
          :max-width="'510px'"
          :content-class="'tutor-request-dialog'"
        >
          <tutor-request v-if="getRequestTutorDialog"></tutor-request>
        </sb-dialog>

        <sb-dialog
          v-if="!!accountUser"
          :showDialog="getReferralDialog"
          :popUpType="'referralPop'"
          :onclosefn="closeReferralDialog"
          :content-class="'login-popup'"
        >
          <referral-dialog v-if="getReferralDialog"
            :isTransparent="true"
            :onclosefn="closeReferralDialog"
            :showDialog="getReferralDialog"
            :popUpType="'referralPop'"
          ></referral-dialog>
        </sb-dialog>

    <v-snackbar
      absolute
      top
      :timeout="getToasterTimeout"
      :class="getShowToasterType"
      :value="getShowToaster"
    >
      <div class="text-wrap" v-html="getToasterText"></div>
    </v-snackbar>
   </v-app>
</template>

<script>
import { mapGetters, mapActions } from "vuex";
import * as routeNames from '../../routes/routeNames.js';

const sbDialog = () => import("../wrappers/sb-dialog/sb-dialog.vue");
const AddQuestion = () => import("../question/askQuestion/askQuestion.vue");
const mobileFooter = () => import("../pages/layouts/mobileFooter/mobileFooter.vue");
const tutorRequest = () => import("../tutorRequestNEW/tutorRequest.vue");
const referralDialog = () => import("../question/helpers/referralDialog/referral-dialog.vue");

//This should not be async since we can loose events if the components not loading in time
import dialogInjection from '../pages/global/dialogInjection/dialogInjection.vue';
import componentInjection from '../pages/global/toasterInjection/componentInjection.vue';

export default {
   components:{
      dialogInjection,
      componentInjection,

      referralDialog,
      AddQuestion,
      sbDialog,
      mobileFooter,
      tutorRequest,
   },
   data() {
      return {
         hideFooter: false,
         toasterTimeoutObj: null
      };
   },
   computed: {
      ...mapGetters([
         "getReferralDialog",
         "accountUser",
         "newQuestionDialogSate",
         "getShowToaster",
         "getShowToasterType",
         "getToasterTimeout",
         "getToasterText",
         "getMobileFooterState",
         "getRequestTutorDialog",
         'getUserLoggedInStatus',
         'getIsTeacher',
      ]),
      isDrawer() {
         let isLogged = this.getUserLoggedInStatus
         let isTeacher = this.getIsTeacher
         let isMobile = this.$vuetify.breakpoint.xsOnly
         return isLogged && isTeacher && !isMobile
      },
      drawerPlaceholder() {
         // need to think of better way to check if placeholder
         let isRoutes = [
         'feed',
         'question',
         'myFollowers',
         'mySales',
         'myContent',
         'myPurchases',
         'myStudyRooms',
         'myCalendar',
         'addCourse',
         'editCourse'].some(route => this.$route.name === route)
         return isRoutes
      },
      isMobile() {
         return this.$vuetify.breakpoint.smAndDown;
      },
      showMobileFooter() {
         return this.$vuetify.breakpoint.xsOnly && 
         this.getMobileFooterState && 
         !this.hideFooter && 
         this.$route.name !== 'tutorLandingPage';
      },
      showHeader(){
         if(this.$route.name == routeNames.MessageCenter){
         return !this.$vuetify.breakpoint.xsOnly 
         }else{
         return true;
         }
      }
   },
   methods: {
      ...mapActions([
         "updateReferralDialog",
         "updateToasterParams",
         "updateRequestDialog",
         "setTutorRequestAnalyticsOpenedFrom",
         "fireOptimizeActivate",
         "updateBannerStatus" 
      ]),
      closeReferralDialog() {
         this.updateReferralDialog(false);
      },
   },
   watch: {
      getShowToaster: function(val) {
         let self = this;
         if (val) {
            this.toasterTimeoutObj = setTimeout(() => {
               if (val) {
                  self.updateToasterParams({
                     showToaster: false
                  });
               }
            }, this.getToasterTimeout);
         } else {
            global.clearTimeout(this.toasterTimeoutObj);
            self.updateToasterParams({
               showToaster: false
            });
         }
      },
      getToasterTimeout: function() {
         let self = this;
         global.clearTimeout(this.toasterTimeoutObj);
         this.toasterTimeoutObj = setTimeout(() => {
            self.updateToasterParams({
               showToaster: false
            });
         }, this.getToasterTimeout);
      },
      $route() {
         this.$nextTick(() => {
            this.fireOptimizeActivate();
         });
      },
   },
   updated: function() {
      this.$nextTick(function() {
         if (!!global.dataLayer) {
            this.fireOptimizeActivate();
         }
         // Code that will run only after the
         // entire question-details has been re-rendered
      });
   },
   mounted: function() {
      if (!!global.dataLayer) {
         this.$nextTick(function() {
         this.fireOptimizeActivate();
         });
      }
   },
   created() {
      if (!!this.$route.query && this.$route.query.requesttutor) {
         if (this.$route.query.requesttutor.toLowerCase() === "open") {
            setTimeout(() => {
               this.setTutorRequestAnalyticsOpenedFrom({
                  component: "query",
                  path: this.$route.path
               });
               this.updateRequestDialog(true);
            }, 170);
         }
      }
      this.$store.dispatch('updateBannerStatus',true);

      if (global.isMobileAgent) {
         global.addEventListener("resize", () => {
            if (
               (document && document.activeElement.tagName == "INPUT") ||
               document.activeElement.tagName == "TEXTAREA"
            ) {
               this.hideFooter = true;
            } else {
               this.hideFooter = false;
            }
         });
      }
      global.addEventListener("error", event => {
         event.stopImmediatePropagation();
         event.stopPropagation();
         event.preventDefault();
      });
   }
}
</script>
<style lang="less">
@import '../../styles/mixin.less';
html {
  overflow-y: auto;
}
.site-content {
   transition: none !important;
   background-color: #f5f5f5;
   @media (max-width: @screen-xs) {
      background-color: #E6E6E6;
   }
   &.hidden-sideMenu{
      padding-left: 220px !important;
      @media (max-width: @screen-md) {
         padding-left: 0px !important;
      }
   }
}
</style>

<style lang="less" src="./app.less"></style>
<style lang="less" src="./main.less"></style>